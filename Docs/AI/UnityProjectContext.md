# Unity Project Context

<!-- unity-onboarding:generated:start -->

## Project Summary

- Project root: `D:/git/ShapeForge`
- Purpose: extensible procedural shape framework; Low Poly is the first official implementation.
- Last analyzed: 2026-07-29
- Last analyzed commit: initial uncommitted project

## Confirmed Environment

- Unity version: 2022.3.62f3
- Render pipeline: Built-in Render Pipeline
- Input system: legacy Input Manager
- Target platforms: not yet defined

## Important Packages And Frameworks

| Area | Finding | Confidence | Evidence |
| --- | --- | --- | --- |
| Tests | Unity Test Framework 1.1.33 | Confirmed | `Packages/packages-lock.json` |
| Rendering | No SRP package or configured render pipeline | Confirmed | `Packages/manifest.json`, `ProjectSettings/GraphicsSettings.asset` |
| Networking | No networking framework detected | Confirmed | `Packages/manifest.json` |

## Directory Structure

| Path | Purpose | Confidence | Evidence |
| --- | --- | --- | --- |
| `Assets/Scenes` | Default Unity sample scene | Confirmed | repository files |
| `Packages` | Unity project dependencies; ShapeForge packages not created yet | Confirmed | repository files |
| `Docs/AI` | Concise persistent project context | Confirmed | this document |

## Assembly Boundaries

No first-party assembly definitions exist yet. Planned boundaries are independent Core and LowPoly runtime, editor, and test assemblies.

## Scenes And Startup Flow

- Build scenes: none enabled.
- Likely startup scene: unknown; `Assets/Scenes/SampleScene.unity` exists but is not in Build Settings.
- Scene loading flow: none implemented.

## Architecture

The project currently contains no first-party runtime code. The confirmed direction is a serializable shape hierarchy consumed by replaceable style-aware generators. Core must not depend on LowPoly or external AI providers.

## Coding Conventions

- Follow `C:/Users/Administrator/.codex/PREFERENCES.md` and `.editorconfig`.
- Use English comments and concise XML documentation for public APIs.
- Keep runtime and editor assemblies separate.

## Testing And Validation

- Unity Test Framework is installed.
- No first-party EditMode, PlayMode, CI, or build validation exists yet.

## Available Unity Tooling

- Repository and Git CLI access: available.
- Unity MCP and live Editor inspection: unavailable; no provider detected.
- Unity Editor-generated C# projects exist, but no first-party code exists to compile.

## Important Constraints

- Shape is the smallest public modelling concept.
- Geometry, appearance, style, and animation data must remain extensible.
- Low Poly is an implementation package, not a Core dependency.
- External AI produces validated definitions and is not a Core dependency.
- Preserve generated node hierarchy for editing and pivot animation.

## Unknowns And Confidence

- Package API, serialization schema, stable node identity, and style inheritance remain design work.
- Supported Unity versions and public release license are not yet decided.

## Source Files Inspected

- `ProjectSettings/ProjectVersion.txt`
- `ProjectSettings/GraphicsSettings.asset`
- `ProjectSettings/ProjectSettings.asset`
- `ProjectSettings/EditorBuildSettings.asset`
- `Packages/manifest.json`
- `Packages/packages-lock.json`
- `Assets/Scenes/SampleScene.unity`

<!-- unity-onboarding:generated:end -->

