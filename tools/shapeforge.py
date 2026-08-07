#!/usr/bin/env python3
"""Thin Python orchestrator for ShapeForge's authoritative Unity/C# APIs."""

from __future__ import annotations

import argparse
import json
import os
import subprocess
import sys
import time
import urllib.request
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
WORK = ROOT / "Library" / "ShapeForgeAutomation"
MCP_URL = os.environ.get("SHAPEFORGE_MCP_URL", "http://127.0.0.1:8080/mcp")


class McpClient:
    def __init__(self, instance: str | None):
        self.session = None
        self._request("initialize", {
            "protocolVersion": "2025-03-26", "capabilities": {},
            "clientInfo": {"name": "ShapeForge CLI", "version": "1.0"},
        }, initialize=True)
        if instance:
            self.call("set_active_instance", {"instance": instance})

    def call(self, name: str, arguments: dict) -> dict:
        result = self._request("tools/call", {"name": name, "arguments": arguments})
        content = result.get("result", {}).get("structuredContent", {})
        return content.get("result", content)

    def _request(self, method: str, params: dict, initialize: bool = False) -> dict:
        body = json.dumps({"jsonrpc": "2.0", "id": 1, "method": method, "params": params}).encode()
        headers = {"Content-Type": "application/json", "Accept": "application/json, text/event-stream"}
        if self.session:
            headers.update({"Mcp-Session-Id": self.session, "MCP-Protocol-Version": "2025-03-26"})
        with urllib.request.urlopen(urllib.request.Request(MCP_URL, body, headers), timeout=45) as response:
            if initialize:
                self.session = response.headers.get("Mcp-Session-Id")
            lines = response.read().decode("utf-8").splitlines()
        payloads = [json.loads(line[6:]) for line in lines if line.startswith("data: ")]
        return payloads[-1] if payloads else {}


def run_document(args: argparse.Namespace) -> int:
    WORK.mkdir(parents=True, exist_ok=True)
    request = {"command": args.command, "source": str(Path(args.source).resolve())}
    if getattr(args, "other", None):
        request["other"] = str(Path(args.other).resolve())
    (WORK / "request.json").write_text(json.dumps(request), encoding="utf-8")
    client = McpClient(args.instance)
    response = client.call("execute_menu_item", {"menu_path": "ShapeForge/Automation/Process Request"})
    if not response.get("success", False):
        return fail(response.get("error", "Unity rejected the automation request."))
    result = json.loads((WORK / "result.json").read_text(encoding="utf-8"))
    output = json.dumps(result.get("data"), indent=2, ensure_ascii=False)
    if args.output:
        Path(args.output).write_text(output + "\n", encoding="utf-8")
    else:
        print(output)
    if not result.get("success"):
        if result.get("error"):
            print(result["error"], file=sys.stderr)
        return 1
    return 0


def run_repository(_: argparse.Namespace) -> int:
    return subprocess.run([sys.executable, ROOT / ".github/scripts/validate_repository.py"], cwd=ROOT).returncode


def fail(message: str) -> int:
    print(f"ERROR: {message}", file=sys.stderr)
    return 1


def parser() -> argparse.ArgumentParser:
    result = argparse.ArgumentParser(description=__doc__)
    result.add_argument("--instance", help="Unity MCP instance, for example ShapeForge@abc123")
    commands = result.add_subparsers(dest="command", required=True)
    for name, other in (("validate", None), ("diff", "after"), ("patch", "patch"),
                        ("quality", "policy"), ("assess", None)):
        command = commands.add_parser(name)
        command.add_argument("source")
        if other:
            command.add_argument("other", metavar=other)
        command.add_argument("-o", "--output")
        command.set_defaults(handler=run_document)
    repository = commands.add_parser("repository")
    repository.set_defaults(handler=run_repository)
    return result


if __name__ == "__main__":
    arguments = parser().parse_args()
    raise SystemExit(arguments.handler(arguments))
