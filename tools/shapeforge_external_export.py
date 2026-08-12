#!/usr/bin/env python3
"""Run an explicitly configured external converter against a ShapeForge GLB."""

from __future__ import annotations

import subprocess
from pathlib import Path


SUPPORTED_FORMATS = {"fbx", "usd", "usda", "usdc", "usdz"}


def export_external(source: Path, output: Path, command: list[str], timeout: int = 300) -> dict:
    """Invoke a converter without a shell and verify that it produced the requested asset."""
    source = source.resolve()
    output = output.resolve()
    if not source.is_file() or source.suffix.lower() != ".glb":
        raise ValueError("External export requires an existing ShapeForge .glb source")
    target_format = output.suffix.lower().lstrip(".")
    if target_format not in SUPPORTED_FORMATS:
        raise ValueError(f"Unsupported external export format: {target_format or '(none)'}")
    if not command:
        raise ValueError("External export requires a converter command")

    arguments = [part.replace("{input}", str(source)).replace("{output}", str(output)) for part in command]
    if not any("{input}" in part for part in command) or not any("{output}" in part for part in command):
        raise ValueError("Converter command must contain both {input} and {output} placeholders")
    output.parent.mkdir(parents=True, exist_ok=True)
    output.unlink(missing_ok=True)
    completed = subprocess.run(arguments, capture_output=True, text=True, timeout=timeout, shell=False)
    if completed.returncode != 0:
        detail = (completed.stderr or completed.stdout).strip()
        raise RuntimeError(f"Converter exited with {completed.returncode}: {detail[-1000:]}")
    if not output.is_file() or output.stat().st_size == 0:
        raise RuntimeError("Converter reported success but did not create a non-empty output asset")
    return {
        "format": target_format,
        "path": str(output),
        "bytes": output.stat().st_size,
        "converter": Path(arguments[0]).name,
    }
