"""Bounded render/compare/patch reconstruction loop for ShapeForge."""

from __future__ import annotations

import json
import math
import shutil
import subprocess
import sys
from pathlib import Path
from typing import Any, Callable

try:
    from tools.shapeforge_image_compare import compare_manifests, measure_manifest_aspects
except ModuleNotFoundError:
    from shapeforge_image_compare import compare_manifests, measure_manifest_aspects


Invoke = Callable[[list[str]], None]


def reconstruct_images(
    model_path: Path,
    reference_path: Path,
    capture_path: Path,
    output_path: Path,
    work_path: Path,
    maximum_iterations: int,
    target_score: float,
    minimum_improvement: float,
    invoke: Invoke,
) -> dict[str, Any]:
    """Run a bounded deterministic reconstruction loop and preserve the best candidate."""
    if maximum_iterations < 1 or maximum_iterations > 50:
        raise ValueError("Maximum iterations must be between 1 and 50.")
    if not 0.0 < target_score <= 1.0:
        raise ValueError("Target score must be between zero and one.")
    if not 0.0 <= minimum_improvement < 1.0:
        raise ValueError("Minimum improvement must be between zero and one.")
    model_path = model_path.resolve()
    reference_path = reference_path.resolve()
    capture_template = _read_json(capture_path.resolve())
    work_path.mkdir(parents=True, exist_ok=True)
    output_path.parent.mkdir(parents=True, exist_ok=True)
    current_model = work_path / "candidate-0.json"
    shutil.copy2(model_path, current_model)
    best_model = current_model
    best_score = -1.0
    previous_score = -1.0
    iterations = []
    status = "maximumIterations"

    for iteration in range(maximum_iterations):
        iteration_folder = work_path / f"iteration-{iteration:02d}"
        images_folder = iteration_folder / "images"
        iteration_folder.mkdir(parents=True, exist_ok=True)
        capture = dict(capture_template)
        capture["id"] = f"{capture_template.get('id', 'capture')}/iteration-{iteration}"
        capture["candidateId"] = f"candidate/iteration-{iteration}"
        capture_document = iteration_folder / "capture.json"
        capture_manifest = iteration_folder / "capture-manifest.json"
        comparison_path = iteration_folder / "comparison.json"
        aggregate_path = iteration_folder / "aggregate.json"
        patch_path = iteration_folder / "global-proportion.patch.json"
        _write_json(capture_document, capture)

        invoke(["render", str(current_model), str(capture_document), "--images", str(images_folder),
                "-o", str(capture_manifest)])
        comparison = compare_manifests(reference_path, capture_manifest)
        _write_json(comparison_path, comparison)
        invoke(["compare", str(comparison_path), "-o", str(aggregate_path)])
        aggregate = _read_json(aggregate_path)
        score = float(aggregate["overallScore"])
        record = {
            "iteration": iteration,
            "score": score,
            "model": str(current_model),
            "captureManifest": str(capture_manifest),
            "comparison": str(comparison_path),
            "aggregate": str(aggregate_path),
        }
        iterations.append(record)
        if score > best_score:
            best_score = score
            best_model = current_model
        if score >= target_score:
            status = "targetReached"
            break
        if iteration > 0 and score - previous_score < minimum_improvement:
            status = "stalled"
            break
        previous_score = score
        if iteration == maximum_iterations - 1:
            break

        definition = _read_json(current_model)
        aspects = measure_manifest_aspects(reference_path, capture_manifest)
        patch = plan_global_proportion_patch(definition, aspects)
        if not patch["operations"]:
            status = "needsSemanticPatch"
            break
        _write_json(patch_path, patch)
        next_model = work_path / f"candidate-{iteration + 1}.json"
        invoke(["patch", str(current_model), str(patch_path), "-o", str(next_model)])
        record["patch"] = str(patch_path)
        current_model = next_model

    shutil.copy2(best_model, output_path)
    result = {
        "schema": "shapeforge.image-reconstruction-report/1.0",
        "status": status,
        "bestScore": round(best_score, 6),
        "targetScore": target_score,
        "bestModel": str(output_path.resolve()),
        "iterations": iterations,
    }
    _write_json(work_path / "report.json", result)
    return result


def plan_global_proportion_patch(definition: dict[str, Any], aspects: dict[str, dict[str, float]]) -> dict[str, Any]:
    """Plan one conservative root-scale patch from recognizable orthographic view names."""
    root = definition.get("root") or {}
    transform = root.get("transform") or {}
    scale = dict(transform.get("scale") or {"x": 1.0, "y": 1.0, "z": 1.0})
    logs: dict[str, list[tuple[float, float]]] = {"x": [], "z": []}
    for view_id, values in aspects.items():
        candidate = values["candidate"]
        if candidate <= 0:
            continue
        ratio = values["reference"] / candidate
        name = view_id.lower()
        axis = "z" if any(token in name for token in ("side", "left", "right")) else "x"
        if any(token in name for token in ("top", "bottom")):
            axis = "x"
        logs[axis].append((math.log(max(ratio, 1e-6)), values["weight"]))
    changed = False
    for axis, values in logs.items():
        if not values:
            continue
        total_weight = sum(weight for _, weight in values)
        correction = math.exp(sum(value * weight for value, weight in values) / total_weight * 0.5)
        correction = min(1.25, max(0.8, correction))
        if abs(correction - 1.0) < 0.005:
            continue
        scale[axis] = round(float(scale.get(axis, 1.0)) * correction, 6)
        changed = True
    if not changed:
        return {"schema": "shapeforge.patch/1.0", "operations": []}
    updated_transform = {
        "position": transform.get("position") or {"x": 0.0, "y": 0.0, "z": 0.0},
        "eulerAngles": transform.get("eulerAngles") or {"x": 0.0, "y": 0.0, "z": 0.0},
        "scale": scale,
    }
    return {
        "schema": "shapeforge.patch/1.0",
        "operations": [{
            "kind": "updateNode",
            "nodeId": root["id"],
            "parentId": "",
            "siblingIndex": -1,
            "update": {"transform": updated_transform},
        }],
    }


def cli_invoke(arguments: list[str]) -> None:
    """Invoke the public ShapeForge CLI and preserve failure output."""
    script = Path(__file__).with_name("shapeforge.py")
    process = subprocess.run([sys.executable, str(script), *arguments], check=False)
    if process.returncode != 0:
        raise RuntimeError(f"ShapeForge command failed ({process.returncode}): {' '.join(arguments)}")


def _read_json(path: Path) -> dict[str, Any]:
    return json.loads(path.read_text(encoding="utf-8"))


def _write_json(path: Path, value: dict[str, Any]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
