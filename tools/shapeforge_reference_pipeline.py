"""Resumable, category-neutral reference preprocessing pipeline."""

from __future__ import annotations

import json
from pathlib import Path
from typing import Any

try:
    from tools.shapeforge_reference_blueprint import write_blueprint
except ModuleNotFoundError:
    from shapeforge_reference_blueprint import write_blueprint


PIPELINE_SCHEMA = "shapeforge.reference-pipeline/1.0"
REVIEW_SCHEMA   = "shapeforge.reference-review/1.0"
ANNOTATION_SCHEMA = "shapeforge.reference-annotations/1.0"


def run_pipeline(source: Path, work: Path, review_path: Path | None = None,
                 annotations_path: Path | None = None) -> dict[str, Any]:
    """Run deterministic stages and stop at the explicit compiler handoff boundary."""
    source = source.resolve()
    work.mkdir(parents=True, exist_ok=True)
    blueprint_path = work / "reference-blueprint.json"
    blueprint      = write_blueprint(source, blueprint_path, work / "views")
    if annotations_path is not None:
        blueprint = _apply_annotations(blueprint, annotations_path)
        _write_json(blueprint_path, blueprint)
    review         = _read_review(review_path, blueprint["id"]) if review_path is not None else None
    review_template = work / "review-template.json"
    _write_json(review_template, _review_template(blueprint))

    status = "awaiting-review"
    reviewed_path: Path | None = None
    remaining = [item["kind"] for item in blueprint["reviewQueue"] if item.get("required", True)]
    if review is not None:
        blueprint, remaining = _apply_review(blueprint, review)
        reviewed_path = work / "reviewed-blueprint.json"
        _write_json(reviewed_path, blueprint)
        status = "ready-for-compiler" if not remaining else "awaiting-review"

    manifest = {
        "schema": PIPELINE_SCHEMA,
        "id": blueprint["id"],
        "status": status,
        "sourceImage": str(source),
        "artifacts": {
            "blueprint": str(blueprint_path.resolve()),
            "views": str((work / "views").resolve()),
            "reviewTemplate": str(review_template.resolve()),
            "reviewedBlueprint": str(reviewed_path.resolve()) if reviewed_path else None,
        },
        "remainingReviewKinds": remaining,
        "nextStage": "category-compiler" if status == "ready-for-compiler" else "human-or-ai-review",
    }
    _write_json(work / "pipeline.json", manifest)
    return manifest


def _apply_annotations(blueprint: dict[str, Any], path: Path) -> dict[str, Any]:
    annotations = json.loads(path.read_text(encoding="utf-8"))
    if annotations.get("schema") != ANNOTATION_SCHEMA:
        raise ValueError("Unsupported reference-annotations schema.")
    if annotations.get("blueprintId") != blueprint["id"]:
        raise ValueError("Annotation blueprintId does not match the measured blueprint.")
    palette = annotations.get("palette", [])
    for sample in palette:
        value = sample.get("hex", "")
        if len(value) != 7 or value[0] != "#" or any(character not in "0123456789abcdefABCDEF" for character in value[1:]):
            raise ValueError("Annotation palette colors require #RRGGBB values.")
    if palette:
        blueprint["palette"] = [{**sample, "source": sample.get("source", "printed-label"),
                                  "confidence": float(sample.get("confidence", 1.0))} for sample in palette]
    blueprint["annotations"] = annotations.get("annotations", [])
    blueprint["measurements"].update(annotations.get("measurements", {}))
    blueprint["reviewQueue"] = [item for item in blueprint["reviewQueue"]
                                if item["kind"] != "printed-annotations"]
    return blueprint


def _review_template(blueprint: dict[str, Any]) -> dict[str, Any]:
    return {
        "schema": REVIEW_SCHEMA,
        "blueprintId": blueprint["id"],
        "decisions": [{"kind": item["kind"], "value": None, "confidence": 0.0,
                       "source": "unresolved"} for item in blueprint["reviewQueue"]],
    }


def _read_review(path: Path, blueprint_id: str) -> dict[str, Any]:
    review = json.loads(path.read_text(encoding="utf-8"))
    if review.get("schema") != REVIEW_SCHEMA:
        raise ValueError("Unsupported reference-review schema.")
    if review.get("blueprintId") != blueprint_id:
        raise ValueError("Review blueprintId does not match the measured blueprint.")
    if not isinstance(review.get("decisions"), list):
        raise ValueError("Reference review requires a decisions array.")
    return review


def _apply_review(blueprint: dict[str, Any], review: dict[str, Any]) -> tuple[dict[str, Any], list[str]]:
    decisions = {item.get("kind"): item for item in review["decisions"] if isinstance(item, dict)}
    unresolved = []
    for item in blueprint["reviewQueue"]:
        decision = decisions.get(item["kind"])
        if decision is None or decision.get("value") in (None, ""):
            unresolved.append(item)
    category = decisions.get("asset-category")
    if category and isinstance(category.get("value"), str) and category["value"].strip():
        blueprint["classification"] = {
            "category": category["value"].strip(),
            "confidence": _confidence(category.get("confidence", 0.0)),
        }
    blueprint["reviewQueue"] = unresolved
    blueprint["reviewDecisions"] = review["decisions"]
    return blueprint, [item["kind"] for item in unresolved if item.get("required", True)]


def _confidence(value: Any) -> float:
    number = float(value)
    if not 0.0 <= number <= 1.0:
        raise ValueError("Review confidence must be between zero and one.")
    return number


def _write_json(path: Path, value: dict[str, Any]) -> None:
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
