"""Deterministic, category-neutral reference measurement for ShapeForge."""

from __future__ import annotations

import json
from pathlib import Path
from typing import Any

import numpy as np
from PIL import Image


VIEW_IDS = ("front", "front-three-quarter", "side", "back-three-quarter", "back")


def analyze_reference(source: Path, output_folder: Path) -> dict[str, Any]:
    """Measure one image or split a five-view sheet without guessing asset semantics."""
    image = Image.open(source).convert("RGB")
    rgb   = np.asarray(image, dtype=np.uint8)
    boxes, view_ids = _view_boxes(rgb)
    output_folder.mkdir(parents=True, exist_ok=True)

    views = []
    for view_id, box in zip(view_ids, boxes):
        crop      = image.crop(box)
        crop_path = output_folder / f"{view_id}.png"
        crop.save(crop_path)
        views.append(_measure_view(view_id, crop, crop_path.name))

    supplemental = _supplemental_sheet_evidence(image, output_folder)
    palette      = supplemental.get("palette") or _palette(image.crop(boxes[0]))
    review_queue = [
        {
            "kind": "asset-category",
            "reason": "Visual measurements cannot safely choose the category-specific compiler.",
            "required": True,
        },
        {
            "kind": "semantic-part-labels",
            "reason": "Pixel regions do not uniquely identify architectural, character, vehicle, or prop parts.",
            "required": True,
        },
        {
            "kind": "hidden-geometry",
            "reason": "Occluded topology, depth, and construction are not observable.",
            "required": True,
        },
        {
            "kind": "scale-and-orientation",
            "reason": "Pixels do not provide real-world scale or a guaranteed coordinate frame.",
            "required": True,
        },
    ]
    if supplemental:
        review_queue.append({
            "kind": "printed-annotations",
            "reason": "Printed labels and exact written values require an OCR provider or human transcription.",
            "required": False,
        })
    return {
        "schema": "shapeforge.reference-blueprint/1.0",
        "id": source.stem.lower().replace(" ", "-"),
        "sourceImage": str(source.resolve()),
        "coordinateSystem": "image-normalized/top-left",
        "views": views + supplemental.get("views", []),
        "measurements": _cross_view_measurements(views),
        "palette": palette,
        "evidenceRegions": supplemental.get("regions", []),
        "layoutProfile": supplemental.get("layoutProfile", "single-or-turntable"),
        "classification": {"category": "unresolved", "confidence": 0.0},
        "reviewQueue": review_queue,
    }


def _supplemental_sheet_evidence(image: Image.Image, output_folder: Path) -> dict[str, Any]:
    width, height = image.size
    aspect = width / max(height, 1)
    if not 1.42 <= aspect <= 1.58:
        return {}
    rgb = np.asarray(image, dtype=np.uint8)
    header_y = int(height * 0.60)
    if np.mean(rgb[header_y].mean(axis=1) < 80) < 0.65:
        return {}

    definitions = (
        ("top", "orthographic-view", (0.01, 0.615, 0.182, 0.985)),
        ("bottom", "orthographic-view", (0.191, 0.615, 0.362, 0.985)),
        ("palette", "palette", (0.371, 0.615, 0.553, 0.985)),
        ("proportions", "measurement-diagram", (0.562, 0.615, 0.777, 0.985)),
        ("characteristics", "text-annotations", (0.786, 0.615, 0.99, 0.985)),
        ("head-front-detail", "detail", (0.69, 0.08, 0.84, 0.255)),
        ("head-side-detail", "detail", (0.84, 0.08, 0.99, 0.255)),
        ("head-back-detail", "detail", (0.69, 0.255, 0.84, 0.415)),
        ("torso-detail", "detail", (0.84, 0.255, 0.99, 0.415)),
        ("footwear-detail", "detail", (0.69, 0.415, 0.84, 0.57)),
        ("hand-detail", "detail", (0.84, 0.415, 0.99, 0.57)),
    )
    regions = []
    views   = []
    for region_id, kind, normalized in definitions:
        box       = _pixel_box(normalized, width, height)
        crop      = image.crop(box)
        crop_path = output_folder / f"{region_id}.png"
        crop.save(crop_path)
        regions.append({
            "id": region_id, "kind": kind, "imagePath": crop_path.name,
            "bounds": {"x": normalized[0], "y": normalized[1],
                       "width": normalized[2] - normalized[0], "height": normalized[3] - normalized[1]},
            "confidence": 0.8,
        })
        if region_id in ("top", "bottom"):
            views.append(_measure_view(region_id, crop, crop_path.name))
    return {
        "layoutProfile": "reference-sheet-grid/1.0",
        "regions": regions,
        "views": views,
        "palette": _labeled_palette_samples(image),
    }


def _pixel_box(bounds: tuple[float, float, float, float], width: int,
               height: int) -> tuple[int, int, int, int]:
    return tuple(round(value * size) for value, size in zip(bounds, (width, height, width, height)))


def _labeled_palette_samples(image: Image.Image) -> list[dict[str, Any]]:
    rgb     = np.asarray(image.convert("RGB"), dtype=np.uint8)
    samples = []
    center_x = round(image.width * 0.393)
    for index in range(9):
        center_y = round(image.height * (0.638 + index * 0.04))
        patch    = rgb[center_y - 5:center_y + 6, center_x - 5:center_x + 6]
        color    = np.median(patch.reshape(-1, 3), axis=0).astype(np.uint8)
        samples.append({
            "id": f"swatch-{index + 1}",
            "hex": "#" + "".join(f"{int(channel):02X}" for channel in color),
            "source": "labeled-swatch-sample",
            "confidence": 0.9,
        })
    return samples


def write_blueprint(source: Path, output: Path, crops: Path) -> dict[str, Any]:
    """Analyze and persist one blueprint using UTF-8 JSON."""
    result = analyze_reference(source, crops)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(result, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    return result


def _view_boxes(rgb: np.ndarray) -> tuple[list[tuple[int, int, int, int]], tuple[str, ...]]:
    height, width = rgb.shape[:2]
    if width / max(height, 1) < 1.4:
        return [(0, 0, width, height)], ("source",)
    try:
        return _turntable_boxes(rgb), VIEW_IDS
    except ValueError:
        return [(0, 0, width, height)], ("source",)


def _turntable_boxes(rgb: np.ndarray) -> list[tuple[int, int, int, int]]:
    height, width = rgb.shape[:2]
    x0, x1        = round(width * 0.015), round(width * 0.68)
    y0, y1        = round(height * 0.075), round(height * 0.57)
    region        = rgb[y0:y1, x0:x1].astype(np.int16)
    background    = np.median(region.reshape(-1, 3), axis=0)
    distance      = np.linalg.norm(region - background, axis=2)
    chroma        = region.max(axis=2) - region.min(axis=2)
    mask          = (distance > 36) | (chroma > 28)
    activity      = mask.sum(axis=0).astype(np.float64)
    kernel        = np.ones(max(9, width // 120), dtype=np.float64)
    activity      = np.convolve(activity, kernel / kernel.size, mode="same")
    minimum_gap   = max(40, (x1 - x0) // 8)
    peaks: list[int] = []
    for index in np.argsort(activity)[::-1]:
        if activity[index] < (y1 - y0) * 0.08:
            break
        if all(abs(int(index) - peak) >= minimum_gap for peak in peaks):
            peaks.append(int(index))
        if len(peaks) == 5:
            break
    if len(peaks) != 5:
        raise ValueError("Could not isolate five turntable views; provide a cleaner five-view sheet.")
    peaks.sort()
    boundaries = [0] + [(peaks[i] + peaks[i + 1]) // 2 for i in range(4)] + [x1 - x0]
    padding    = round(width * 0.012)
    return [
        (max(x0, x0 + boundaries[i] - padding), y0,
         min(x1, x0 + boundaries[i + 1] + padding), y1)
        for i in range(5)
    ]


def _measure_view(view_id: str, image: Image.Image, image_path: str) -> dict[str, Any]:
    rgb        = np.asarray(image, dtype=np.uint8)
    background = np.median(rgb.reshape(-1, 3), axis=0)
    distance   = np.linalg.norm(rgb.astype(np.int16) - background, axis=2)
    chroma     = rgb.max(axis=2).astype(np.int16) - rgb.min(axis=2).astype(np.int16)
    mask       = (distance > 34) | (chroma > 26)
    mask[: max(1, image.height // 40)] = False
    mask       = _largest_component(mask)
    ys, xs = np.nonzero(mask)
    if len(xs) == 0:
        raise ValueError(f"View '{view_id}' contains no measurable foreground.")
    x0, x1 = int(xs.min()), int(xs.max()) + 1
    y0, y1 = int(ys.min()), int(ys.max()) + 1
    bounds = {
        "x": round(x0 / image.width, 6), "y": round(y0 / image.height, 6),
        "width": round((x1 - x0) / image.width, 6),
        "height": round((y1 - y0) / image.height, 6),
    }
    silhouette = _sample_silhouette(mask[y0:y1, x0:x1], x0, y0, image.width, image.height)
    return {
        "viewId": view_id,
        "imagePath": image_path,
        "foregroundBounds": bounds,
        "silhouette": silhouette,
        "confidence": 0.78 if view_id in ("front", "side", "back") else 0.68,
    }


def _largest_component(mask: np.ndarray) -> np.ndarray:
    visited = np.zeros(mask.shape, dtype=bool)
    best: list[tuple[int, int]] = []
    height, width = mask.shape
    for start_y, start_x in zip(*np.nonzero(mask)):
        if visited[start_y, start_x]:
            continue
        stack = [(int(start_y), int(start_x))]
        visited[start_y, start_x] = True
        component: list[tuple[int, int]] = []
        touches = set()
        while stack:
            y, x = stack.pop()
            component.append((y, x))
            if y == 0: touches.add("top")
            if y == height - 1: touches.add("bottom")
            if x == 0: touches.add("left")
            if x == width - 1: touches.add("right")
            for next_y, next_x in ((y - 1, x), (y + 1, x), (y, x - 1), (y, x + 1)):
                if 0 <= next_y < height and 0 <= next_x < width and mask[next_y, next_x] \
                        and not visited[next_y, next_x]:
                    visited[next_y, next_x] = True
                    stack.append((next_y, next_x))
        if len(touches) < 2 and len(component) > len(best):
            best = component
    result = np.zeros(mask.shape, dtype=bool)
    for y, x in best:
        result[y, x] = True
    return result


def _sample_silhouette(mask: np.ndarray, offset_x: int, offset_y: int,
                       width: int, height: int) -> list[dict[str, float]]:
    points = []
    rows   = np.linspace(0, mask.shape[0] - 1, 32, dtype=int)
    for row in rows:
        xs = np.nonzero(mask[row])[0]
        if len(xs):
            points.append({"x": round((offset_x + int(xs.min())) / width, 6),
                           "y": round((offset_y + row) / height, 6)})
    for row in rows[::-1]:
        xs = np.nonzero(mask[row])[0]
        if len(xs):
            points.append({"x": round((offset_x + int(xs.max())) / width, 6),
                           "y": round((offset_y + row) / height, 6)})
    return points


def _cross_view_measurements(views: list[dict[str, Any]]) -> dict[str, float]:
    first = views[0]["foregroundBounds"]
    result = {"primaryAspect": round(first["width"] / max(first["height"], 1e-6), 6)}
    indexed = {view["viewId"]: view for view in views}
    if "front" in indexed and "side" in indexed:
        front = indexed["front"]["foregroundBounds"]
        side  = indexed["side"]["foregroundBounds"]
        result["sideToFrontWidth"] = round(side["width"] / max(front["width"], 1e-6), 6)
    return result


def _palette(image: Image.Image) -> list[dict[str, Any]]:
    rgb       = np.asarray(image.convert("RGB"), dtype=np.uint8)
    quantized = (rgb.reshape(-1, 3) // 24) * 24 + 12
    colors, counts = np.unique(quantized, axis=0, return_counts=True)
    order = np.argsort(counts)[::-1]
    result = []
    for index in order:
        color = colors[index]
        if color.max() - color.min() < 12 and 110 < color.mean() < 230:
            continue
        result.append({
            "hex": "#" + "".join(f"{int(channel):02X}" for channel in color),
            "coverage": round(float(counts[index]) / len(quantized), 6),
        })
        if len(result) == 8:
            break
    return result
