# ShapeForge Development Roadmap

This document is the repository-owned continuation point for maintainers and coding agents. Update it in the same commit that completes or materially changes a milestone.

The public execution board is [ShapeForge Roadmap](https://github.com/orgs/TristinOrg/projects/1). GitHub Issues are the task/status source of truth; this file preserves architectural context and acceptance criteria beside the code.

## Product direction

ShapeForge is an AI-era game asset compiler:

```text
Image / Prompt / Structured Intent
                -> Semantic Definition
                -> Deterministic ShapeForge Compilation
                -> Engine-native Game Asset
```

AI authors structured intent. ShapeForge owns deterministic validation, editing, semantic completeness, generation, and engine adaptation. Core and Schema must remain independent of render engines, concrete art styles, and AI providers.

## Resume instructions

Before changing code:

1. Read `AGENTS.md` and `C:/Users/Administrator/.codex/PREFERENCES.md`.
2. Read this roadmap and inspect the latest commits.
3. Preserve unrelated working-tree changes.
4. Extend existing package boundaries instead of creating parallel systems.
5. Complete one coherent stage, validate it, commit with Conventional Commits, and push.

Useful commands:

```text
python tools/shapeforge.py repository
python tools/shapeforge.py verify
git log --oneline -15
git status --short
```

`verify` requires a connected Unity MCP session. Document operations use the real C# implementation through the Editor automation bridge.

## Definition of done

Every milestone should include, where applicable:

- engine-neutral versioned contracts;
- deterministic validation with stable diagnostic codes and JSON-style paths;
- Core behavior and EditMode tests;
- Draft 2020-12 JSON Schema and minimal examples;
- Unity JSON/adapter integration without leaking Unity into Core;
- Python CLI orchestration for repeatable agent workflows;
- README and changelog updates;
- repository validation and relevant Unity tests;
- independently useful commits pushed to the current branch.

## Completed foundation

### Framework and generation foundation — Complete

- Schema, Core, Unity, and Low Poly package boundaries.
- Deterministic shape definitions, styles, capabilities, templates, references, and generation.
- Unity hierarchy adaptation, lifecycle-safe regeneration, cached materials, and Prefab-safe mesh storage.
- Semantic rigs, canonical Humanoid roles, runtime Avatar creation, and FBX Humanoid animation integration.

### LLM editing foundation — Complete

- Structured validation diagnostics — `56c82a3`.
- Deterministic semantic definition diffs — `3ed6c1e`.
- Atomic ShapePatch add/remove/move/update operations — `3a42cce`.
- Patch JSON workflow — `960e53c`.
- Declarative Quality Gate and structural metrics — `c80c2d9`.
- Quality/Patch schemas and LLM correction loop — `3639fc6`.

### Reconstruction planning foundation — Implemented

- Reference Assessment contract, validation, Schema, and JSON ingestion — `a4389ad`.
- Detail Inventory contract and definition coverage analysis — `88b99f2`.
- Detail Inventory Schema, Unity JSON, CLI command, and documentation — `0e14b52`.

### Automation CLI — Implemented

- C# Editor bridge and Python document commands — `46fc66d`.
- Repository verification, Unity test orchestration, instance selection, reconnect handling, and Python tests — `9cd471d`.

Current validation note: the Unity MCP HTTP server is reachable, but the Editor reports `Unity session not available`. Python tests and repository validation pass. A temporary MSBuild import compiled all Core, Unity, and Low Poly test assemblies successfully on 2026-08-10, then was removed. After reconnecting the Editor session, run `python tools/shapeforge.py verify`; do not mark the live execution debt cleared unless it executes at least one test and succeeds.

## Ordered milestones

### M1 — Render Compare IR

Status: Implemented; live EditMode execution pending — [#2](https://github.com/TristinOrg/ShapeForge/issues/2). Commits `7388c62`, `0ddde54`.

Goal: represent visual comparison results without coupling Core to cameras, renderers, or vision providers.

Deliverables:

- versioned comparison request/result contracts;
- named views and observation dimensions;
- normalized score components such as silhouette, proportion, color, and semantic-detail coverage;
- localized discrepancy records linked to stable node/detail IDs;
- deterministic validation and comparison aggregation;
- JSON Schema, examples, Unity serialization, CLI command, and tests;
- explicit boundary: external systems render and observe; ShapeForge consumes structured observations.

### M2 — Construction Pass System

Status: Implemented; live EditMode execution pending — [#3](https://github.com/TristinOrg/ShapeForge/issues/3). Commits `fb66be1`, `1ccdc22`.

Goal: build and review assets in deterministic passes rather than one monolithic generation.

Initial passes:

```text
Structure -> Primary Forms -> Secondary Forms -> Details -> Appearance -> Gameplay Semantics -> Final Quality
```

Deliverables include versioned pass plans, dependencies, completion state, per-pass patches, quality policies, resumability, and reports.

### M3 — Game Semantic Metadata

Status: Implemented; live EditMode execution pending — [#4](https://github.com/TristinOrg/ShapeForge/issues/4). Commits `b28ce6e`, `784d7f0`, `cf75cb1`.

Goal: make generated output directly useful as a game asset rather than only a model.

Develop in this order:

1. generic semantic anchors and sockets;
2. hand grips, weapon sockets, mount points, and interaction points;
3. damage zones and gameplay tags;
4. Foot IK and grounding markers;
5. collider rules;
6. LOD rules.

Core owns engine-neutral contracts and validation. Engine packages compile them into native components and assets.

### M4 — Semantic Template Library

Status: Implemented; live EditMode execution pending — [#5](https://github.com/TristinOrg/ShapeForge/issues/5). Commits `12cb22d`, `0cc90ba`.

Expand reusable templates only after the metadata foundation exists:

1. hair;
2. clothing and armor;
3. weapons and props;
4. buildings;
5. vehicles.

Each library must publish discovery metadata, bounded parameters, detail inventories, quality policies, representative presets, and tests. Low Poly remains the first implementation, not the framework identity.

### M5 — Reconstruction Orchestration

Status: Implemented; live EditMode execution pending — [#6](https://github.com/TristinOrg/ShapeForge/issues/6). Commits `24d1925`, `ce6bacf`.

Compose Reference Assessment, Detail Inventory, passes, Render Compare, Patch, and Quality Gate into a provider-neutral reconstruction workflow. ShapeForge must not own provider credentials, prompting clients, or hidden vision logic.

### M6 — MotionForge boundary integration

Status: Implemented; live EditMode execution pending — [#7](https://github.com/TristinOrg/ShapeForge/issues/7). Commit `56cd954`.

ShapeForge continues to own rest pose, stable nodes, semantic rig roles, constraints, and transform targets. MotionForge should own engine-neutral motion intent/IR, tracks, keyframes, curves, composition, and serialization. Native adapters own playback, blending, IK, retargeting, and optimization.

Do not place complete animation formats or Unity Animator ownership in ShapeForge Core.

### M7 — Engine adapters and export

Status: In progress — [#8](https://github.com/TristinOrg/ShapeForge/issues/8). Unity Prefab compilation and adapter conformance are implemented in `eb652b2` and `d431b7b`; Godot, Unreal, glTF, and licensed FBX/USD paths remain.

Develop only after Core contracts stabilize:

1. finish Unity native Prefab compilation and asset metadata;
2. define an adapter conformance suite;
3. Godot scene adapter;
4. Unreal Actor adapter;
5. glTF export;
6. FBX/USD export where licensing and toolchains permit.

### M8 — ShapeForge Agent and MCP

Status: Implemented over the mature local workflows; live MCP execution pending — [#9](https://github.com/TristinOrg/ShapeForge/issues/9). Commit `a29d133` plus the earlier automation commits.

Expose stable workflows only after their local APIs and Python orchestration are mature:

- capability and template discovery;
- validate, assess, inventory, compare, patch, and quality operations;
- staged build execution and resumability;
- render/observe/patch orchestration through external providers;
- concise machine-readable results with bounded output.

MCP must remain a thin orchestration surface, not a second implementation of ShapeForge rules.

## Explicit non-goals

- Competing with image-to-3D systems on raw reconstruction fidelity.
- Moving provider SDKs into Schema or Core.
- Treating Low Poly or Unity as the ShapeForge framework identity.
- Reimplementing C# validation, Patch, Diff, or Quality Gate logic in Python or MCP.
- Adding primitives without a demonstrated semantic/template requirement.
- Starting multi-engine adapters before the conformance contracts are stable.

## Immediate continuation

1. In Unity, start the MCP Editor session and run `python tools/shapeforge.py verify`.
2. Fix any live EditMode failures before changing implemented milestone statuses to Done.
3. Continue M7 with a geometry-preserving glTF exporter and conformance fixture.
4. Add real Godot and Unreal adapters only with their toolchains available for end-to-end validation.
5. Treat FBX/USD as optional until the selected exporters and licenses are recorded.
