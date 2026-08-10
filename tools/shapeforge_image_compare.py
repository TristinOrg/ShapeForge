"""Deterministic reference/candidate image comparison for ShapeForge."""

from __future__ import annotations

import json
from pathlib import Path
from typing import Any

import numpy as np
from PIL import Image


CANVAS_SIZE = 256
PADDING = 12


def compare_manifests(reference_path: Path, candidate_path: Path) -> dict[str, Any]:
    """Compare matching manifest views and return ShapeForge Render Compare JSON."""
    reference = _read_json(reference_path)
    candidate = _read_json(candidate_path)
    if reference.get("schema") != "shapeforge.reference-images/1.0":
        raise ValueError("Unsupported reference-image manifest schema.")
    reference_views = _index_views(reference.get("images"), "reference")
    candidate_views = _index_views(candidate.get("images"), "candidate")
    if set(reference_views) != set(candidate_views):
        missing = sorted(set(reference_views) - set(candidate_views))
        extra = sorted(set(candidate_views) - set(reference_views))
        raise ValueError(f"View mismatch; missing candidate={missing}, extra candidate={extra}.")

    views: list[dict[str, Any]] = []
    discrepancies: list[dict[str, Any]] = []
    for view_id, reference_view in reference_views.items():
        candidate_view = candidate_views[view_id]
        reference_image = _load_view(reference_path.parent, reference_view["imagePath"])
        candidate_image = _load_view(candidate_path.parent, candidate_view["imagePath"])
        scores, observations = compare_images(reference_image, candidate_image)
        views.append({
            "viewId": view_id,
            "weight": float(reference_view.get("weight", 1.0)),
            "confidence": observations["confidence"],
            "scores": scores,
        })
        discrepancies.extend(_discrepancies(view_id, scores, observations))

    return {
        "schema": "shapeforge.render-compare/1.0",
        "referenceId": reference["id"],
        "candidateId": candidate.get("candidateId") or candidate.get("captureId", "candidate"),
        "views": views,
        "discrepancies": discrepancies,
    }


def compare_images(reference: Image.Image, candidate: Image.Image) -> tuple[dict[str, float], dict[str, Any]]:
    """Compare two images after deterministic foreground extraction and normalized alignment."""
    reference_rgb, reference_mask, reference_box = _prepare(reference)
    candidate_rgb, candidate_mask, candidate_box = _prepare(candidate)
    silhouette = _iou(reference_mask, candidate_mask)
    reference_ratio = reference_box[0] / max(reference_box[1], 1)
    candidate_ratio = candidate_box[0] / max(candidate_box[1], 1)
    aspect_score = min(reference_ratio, candidate_ratio) / max(reference_ratio, candidate_ratio)
    reference_fill = float(reference_mask.mean())
    candidate_fill = float(candidate_mask.mean())
    fill_score = min(reference_fill, candidate_fill) / max(reference_fill, candidate_fill, 1e-6)
    proportion = 0.7 * aspect_score + 0.3 * fill_score
    color = _histogram_intersection(reference_rgb, reference_mask, candidate_rgb, candidate_mask)
    detail = _detail_similarity(reference_rgb, reference_mask, candidate_rgb, candidate_mask)
    confidence = min(1.0, max(0.25, min(reference_fill, candidate_fill) * 4.0))
    scores = {
        "silhouette": _score(silhouette),
        "proportion": _score(proportion),
        "color": _score(color),
        "detail": _score(detail),
    }
    observations = {
        "confidence": _score(confidence),
        "referenceAspect": reference_ratio,
        "candidateAspect": candidate_ratio,
    }
    return scores, observations


def measure_manifest_aspects(reference_path: Path, candidate_path: Path) -> dict[str, dict[str, float]]:
    """Return raw foreground aspect ratios for deterministic global proportion patches."""
    reference = _read_json(reference_path)
    candidate = _read_json(candidate_path)
    reference_views = _index_views(reference.get("images"), "reference")
    candidate_views = _index_views(candidate.get("images"), "candidate")
    result = {}
    for view_id, reference_view in reference_views.items():
        if view_id not in candidate_views:
            continue
        reference_image = _load_view(reference_path.parent, reference_view["imagePath"])
        candidate_image = _load_view(candidate_path.parent, candidate_views[view_id]["imagePath"])
        _, _, reference_box = _prepare(reference_image)
        _, _, candidate_box = _prepare(candidate_image)
        result[view_id] = {
            "reference": reference_box[0] / max(reference_box[1], 1),
            "candidate": candidate_box[0] / max(candidate_box[1], 1),
            "weight": float(reference_view.get("weight", 1.0)),
        }
    return result


def _prepare(image: Image.Image) -> tuple[np.ndarray, np.ndarray, tuple[int, int]]:
    rgba = np.asarray(image.convert("RGBA"), dtype=np.uint8)
    mask = _foreground_mask(rgba)
    ys, xs = np.nonzero(mask)
    if len(xs) == 0:
        raise ValueError("Image contains no detectable foreground.")
    x0, x1 = int(xs.min()), int(xs.max()) + 1
    y0, y1 = int(ys.min()), int(ys.max()) + 1
    width, height = x1 - x0, y1 - y0
    rgb_crop = rgba[y0:y1, x0:x1, :3]
    mask_crop = mask[y0:y1, x0:x1]
    available = CANVAS_SIZE - PADDING * 2
    scale = min(available / width, available / height)
    target_size = (max(1, round(width * scale)), max(1, round(height * scale)))
    rgb_scaled = np.asarray(Image.fromarray(rgb_crop).resize(target_size, Image.Resampling.BILINEAR))
    mask_scaled = np.asarray(
        Image.fromarray(mask_crop.astype(np.uint8) * 255).resize(target_size, Image.Resampling.NEAREST)
    ) > 127
    rgb_canvas = np.zeros((CANVAS_SIZE, CANVAS_SIZE, 3), dtype=np.uint8)
    mask_canvas = np.zeros((CANVAS_SIZE, CANVAS_SIZE), dtype=bool)
    left = (CANVAS_SIZE - target_size[0]) // 2
    top = (CANVAS_SIZE - target_size[1]) // 2
    rgb_canvas[top:top + target_size[1], left:left + target_size[0]] = rgb_scaled
    mask_canvas[top:top + target_size[1], left:left + target_size[0]] = mask_scaled
    return rgb_canvas, mask_canvas, (width, height)


def _foreground_mask(rgba: np.ndarray) -> np.ndarray:
    alpha = rgba[..., 3]
    if alpha.min() < 250:
        return alpha >= 16
    rgb = rgba[..., :3].astype(np.int16)
    corners = np.concatenate((rgb[:4, :4].reshape(-1, 3), rgb[:4, -4:].reshape(-1, 3),
                              rgb[-4:, :4].reshape(-1, 3), rgb[-4:, -4:].reshape(-1, 3)))
    background = np.median(corners, axis=0)
    distance = np.sqrt(np.square(rgb - background).sum(axis=2))
    threshold = max(18.0, float(np.percentile(distance, 35)) + 8.0)
    mask = distance > threshold
    return _largest_component(mask)


def _largest_component(mask: np.ndarray) -> np.ndarray:
    """Keep the largest 8-connected component without scipy/OpenCV."""
    height, width = mask.shape
    visited = np.zeros_like(mask, dtype=bool)
    largest: list[tuple[int, int]] = []
    for y, x in zip(*np.nonzero(mask)):
        if visited[y, x]:
            continue
        stack = [(int(y), int(x))]
        visited[y, x] = True
        component: list[tuple[int, int]] = []
        while stack:
            cy, cx = stack.pop()
            component.append((cy, cx))
            for ny in range(max(0, cy - 1), min(height, cy + 2)):
                for nx in range(max(0, cx - 1), min(width, cx + 2)):
                    if mask[ny, nx] and not visited[ny, nx]:
                        visited[ny, nx] = True
                        stack.append((ny, nx))
        if len(component) > len(largest):
            largest = component
    result = np.zeros_like(mask, dtype=bool)
    if largest:
        ys, xs = zip(*largest)
        result[np.asarray(ys), np.asarray(xs)] = True
    return result


def _iou(first: np.ndarray, second: np.ndarray) -> float:
    union = np.logical_or(first, second).sum()
    return 1.0 if union == 0 else float(np.logical_and(first, second).sum() / union)


def _histogram_intersection(
    first_rgb: np.ndarray, first_mask: np.ndarray,
    second_rgb: np.ndarray, second_mask: np.ndarray,
) -> float:
    first_hsv = np.asarray(Image.fromarray(first_rgb).convert("HSV"))
    second_hsv = np.asarray(Image.fromarray(second_rgb).convert("HSV"))
    bins = (12, 4, 4)
    ranges = ((0, 256), (0, 256), (0, 256))
    first_hist, _ = np.histogramdd(first_hsv[first_mask], bins=bins, range=ranges)
    second_hist, _ = np.histogramdd(second_hsv[second_mask], bins=bins, range=ranges)
    first_hist /= max(first_hist.sum(), 1)
    second_hist /= max(second_hist.sum(), 1)
    return float(np.minimum(first_hist, second_hist).sum())


def _detail_similarity(
    first_rgb: np.ndarray, first_mask: np.ndarray,
    second_rgb: np.ndarray, second_mask: np.ndarray,
) -> float:
    first_edges = _edges(first_rgb, first_mask)
    second_edges = _edges(second_rgb, second_mask)
    forward = _edge_recall(first_edges, second_edges, 3)
    backward = _edge_recall(second_edges, first_edges, 3)
    return 0.0 if forward + backward == 0 else 2.0 * forward * backward / (forward + backward)


def _edges(rgb: np.ndarray, mask: np.ndarray) -> np.ndarray:
    gray = rgb.astype(np.float32).mean(axis=2)
    gx = np.zeros_like(gray)
    gy = np.zeros_like(gray)
    gx[:, 1:-1] = np.abs(gray[:, 2:] - gray[:, :-2])
    gy[1:-1, :] = np.abs(gray[2:, :] - gray[:-2, :])
    boundary = mask & ~(_shift(mask, 1, 0) & _shift(mask, -1, 0) &
                       _shift(mask, 0, 1) & _shift(mask, 0, -1))
    return boundary | (mask & ((gx + gy) > 48.0))


def _edge_recall(source: np.ndarray, target: np.ndarray, radius: int) -> float:
    count = int(source.sum())
    if count == 0:
        return 1.0 if not target.any() else 0.0
    nearby = np.zeros_like(target)
    for dy in range(-radius, radius + 1):
        for dx in range(-radius, radius + 1):
            if dx * dx + dy * dy <= radius * radius:
                nearby |= _shift(target, dy, dx)
    return float(np.logical_and(source, nearby).sum() / count)


def _shift(value: np.ndarray, dy: int, dx: int) -> np.ndarray:
    result = np.zeros_like(value)
    source_y = slice(max(0, -dy), min(value.shape[0], value.shape[0] - dy))
    source_x = slice(max(0, -dx), min(value.shape[1], value.shape[1] - dx))
    target_y = slice(max(0, dy), min(value.shape[0], value.shape[0] + dy))
    target_x = slice(max(0, dx), min(value.shape[1], value.shape[1] + dx))
    result[target_y, target_x] = value[source_y, source_x]
    return result


def _discrepancies(view_id: str, scores: dict[str, float], observations: dict[str, Any]) -> list[dict[str, Any]]:
    result = []
    actions = {
        "silhouette": "Adjust the outermost stable nodes visible in this view.",
        "proportion": "Adjust node positions or local scales to match the reference aspect ratio.",
        "color": "Adjust palette roles or explicit appearance colors.",
        "detail": "Add or refine the missing semantic detail nodes and local profiles.",
    }
    for category, score in scores.items():
        if score >= 0.85:
            continue
        severity = "error" if score < 0.55 else "warning"
        message = f"{category.capitalize()} similarity is {score:.3f} in view '{view_id}'."
        if category == "proportion":
            message += (f" Reference aspect={observations['referenceAspect']:.3f},"
                        f" candidate aspect={observations['candidateAspect']:.3f}.")
        result.append({
            "id": f"{view_id}-{category}",
            "category": category,
            "viewId": view_id,
            "nodeId": "",
            "detailId": "",
            "severity": severity,
            "message": message,
            "suggestedAction": actions[category],
        })
    return result


def _index_views(values: Any, label: str) -> dict[str, dict[str, Any]]:
    if not isinstance(values, list) or not values:
        raise ValueError(f"{label.capitalize()} manifest requires images.")
    result = {}
    for value in values:
        view_id = value.get("viewId") if isinstance(value, dict) else None
        image_path = value.get("imagePath") if isinstance(value, dict) else None
        if not view_id or not image_path:
            raise ValueError(f"Every {label} image requires viewId and imagePath.")
        if view_id in result:
            raise ValueError(f"Duplicate {label} view '{view_id}'.")
        result[view_id] = value
    return result


def _load_view(folder: Path, value: str) -> Image.Image:
    path = Path(value)
    path = path if path.is_absolute() else folder / path
    if not path.is_file():
        raise ValueError(f"Image does not exist: {path}")
    with Image.open(path) as image:
        return image.convert("RGBA")


def _read_json(path: Path) -> dict[str, Any]:
    return json.loads(path.read_text(encoding="utf-8"))


def _score(value: float) -> float:
    return round(float(np.clip(value, 0.0, 1.0)), 6)
