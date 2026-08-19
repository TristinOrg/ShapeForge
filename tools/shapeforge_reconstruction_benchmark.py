"""Run a curated ShapeForge reconstruction corpus and aggregate measurable failures."""

from __future__ import annotations

import json
from pathlib import Path
from typing import Any, Callable

try:
    from tools.shapeforge_reconstruct_images import cli_invoke, reconstruct_images
except ModuleNotFoundError:
    from shapeforge_reconstruct_images import cli_invoke, reconstruct_images


Reconstruct = Callable[..., dict[str, Any]]


def run_benchmark(
    manifest_path: Path,
    output_path: Path,
    work_path: Path,
    reconstruct: Reconstruct = reconstruct_images,
) -> dict[str, Any]:
    """Execute every corpus case and write a deterministic aggregate report."""
    manifest_path = manifest_path.resolve()
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    if manifest.get("schema") != "shapeforge.reconstruction-corpus/1.0":
        raise ValueError("Unsupported reconstruction corpus schema")
    cases = manifest.get("cases")
    if not isinstance(cases, list) or not cases:
        raise ValueError("Reconstruction corpus requires at least one case")

    defaults = manifest.get("defaults") or {}
    base = manifest_path.parent
    identifiers: set[str] = set()
    for case in cases:
        identifier = case.get("id")
        if not isinstance(identifier, str) or not identifier or identifier in identifiers:
            raise ValueError("Each reconstruction case requires a unique non-empty id")
        identifiers.add(identifier)

    results: list[dict[str, Any]] = []
    for case in cases:
        identifier = case["id"]
        case_work = work_path.resolve() / identifier
        best_model = case_work / "best-model.json"
        maximum_iterations = int(case.get("maxIterations", defaults.get("maxIterations", 8)))
        target_score = float(case.get("targetScore", defaults.get("targetScore", 0.9)))
        minimum_score = float(case.get("minimumScore", defaults.get("minimumScore", target_score)))
        minimum_improvement = float(case.get("minimumImprovement", defaults.get("minimumImprovement", 0.005)))
        report = reconstruct(
            _resolve(base, case, "source"), _resolve(base, case, "reference"),
            _resolve(base, case, "capture"), best_model, case_work,
            maximum_iterations, target_score, minimum_improvement, cli_invoke,
        )
        score = float(report["bestScore"])
        results.append({
            "id": identifier,
            "passed": score >= minimum_score,
            "bestScore": score,
            "minimumScore": minimum_score,
            "status": report["status"],
            "iterations": len(report.get("iterations", [])),
            "report": str((case_work / "report.json").resolve()),
        })

    failure_modes: dict[str, int] = {}
    for case in results:
        if not case["passed"]:
            failure_modes[case["status"]] = failure_modes.get(case["status"], 0) + 1
    scores = [case["bestScore"] for case in results]
    result = {
        "schema": "shapeforge.reconstruction-benchmark-report/1.0",
        "corpus": manifest.get("id", manifest_path.stem),
        "passed": not failure_modes,
        "summary": {
            "total": len(results),
            "passed": sum(case["passed"] for case in results),
            "failed": sum(not case["passed"] for case in results),
            "meanScore": round(sum(scores) / len(scores), 6),
            "minimumScore": min(scores),
            "failureModes": failure_modes,
        },
        "cases": results,
    }
    output_path = output_path.resolve()
    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(json.dumps(result, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    return result


def _resolve(base: Path, case: dict[str, Any], field: str) -> Path:
    value = case.get(field)
    if not isinstance(value, str) or not value:
        raise ValueError(f"Reconstruction case requires {field}")
    path = Path(value)
    return (path if path.is_absolute() else base / path).resolve()
