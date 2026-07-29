# Unity Project Context

<!-- unity-onboarding:generated:start -->

## Project Summary

- Project root: `D:/git/ShapeForge`
- Purpose: extensible procedural shape framework; Low Poly is the first official implementation.
- Last analyzed: 2026-07-29
- Last analyzed commit: `ef20d77`

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
| `Packages/com.shapeforge.core` | Engine-neutral validation and style resolution | Confirmed | package manifest |
| `Packages/com.shapeforge.schema` | Engine-agnostic versioned data contracts | Confirmed | package manifest |
| `Packages/com.shapeforge.unity` | Unity generation and reference JSON adapter | Confirmed | package manifest |
| `Packages/com.shapeforge.lowpoly` | Official Low Poly implementation | Confirmed | package manifest |
| `Docs/AI` | Concise persistent project context | Confirmed | this document |

## Assembly Boundaries

Schema and Core use assemblies with no UnityEngine references. Unity adapts Schema/Core to GameObjects and JSON. LowPoly references the Unity Adapter; dependencies remain one-directional.

## Scenes And Startup Flow

- Build scenes: none enabled.
- Likely startup scene: unknown; `Assets/Scenes/SampleScene.unity` exists but is not in Build Settings.
- Scene loading flow: none implemented.

## Architecture

Schema provides versioned engine-agnostic shape and style documents. Core provides validation, fluent authoring, and mutable style registration without UnityEngine. The Unity Adapter owns JSON, GameObject generation, and lifecycle-safe serialized appearance. Its hybrid appearance backend uses cached shared materials for palette roles, property blocks for direct overrides, and one serialized root manifest per model. LowPoly supplies a reusable definition/JSON generation pipeline, cached Cube, Sphere, Cylinder, and Capsule render resources, Cube-based Table and modular Robot presets, plus undoable Editor preview commands.

## Coding Conventions

- Follow `C:/Users/Administrator/.codex/PREFERENCES.md` and `.editorconfig`.
- Use English comments and concise XML documentation for public APIs.
- Keep runtime and editor assemblies separate.

## Testing And Validation

- Unity Test Framework is installed.
- Sixteen first-party EditMode tests cover fluent authoring, the JSON contract and runtime generation entry point, hierarchy adaptation, identity and color validation, lifecycle-safe color reapplication, shared material and primitive-mesh reuse, style resolution, LowPoly generation, furniture generation, and modular Robot pivots.
- `ShapeForge > Diagnostics > Benchmark JSON Generation` measures 200 alternating Table and Robot JSON generations, managed heap growth, and shared render-resource counts without saving generated objects.
- No first-party PlayMode, CI, or build validation exists yet.

## Available Unity Tooling

- Repository and Git CLI access: available.
- Unity MCP and live Editor inspection: available through CoplayDev Unity MCP 10.1.0.
- Unity Editor-generated C# projects exist, but no first-party code exists to compile.

## Important Constraints

- Shape is the smallest public modelling concept.
- Geometry, appearance, style, and animation data must remain extensible.
- Low Poly is an implementation package, not a Core dependency.
- External AI produces validated definitions and is not a Core dependency.
- Preserve generated node hierarchy for editing and pivot animation.

## Unknowns And Confidence

- The initial API and Unity JSON serialization shape are implemented but remain pre-1.0 and may evolve.
- Style inheritance, palette resolution, animation bindings, and external JSON schema versioning remain design work.
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
