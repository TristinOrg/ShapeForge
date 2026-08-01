# ShapeForge

**ShapeForge is an engine-agnostic, schema-driven procedural 3D generation framework for creating editable model hierarchies from code, JSON, authoring tools, and LLM-friendly semantic specifications.**

Rather than treating a model as an opaque mesh file, ShapeForge represents it as a versioned hierarchy of named nodes, procedural shapes, local transforms, appearances, styles, and motion-ready pivots. An engine adapter turns that validated definition into native objects that remain inspectable and editable after generation.

The repository includes a reference Unity adapter and an official Low Poly implementation, but neither Unity nor Low Poly defines the framework itself. Geometry backends, render pipelines, palettes, semantic templates, and external AI providers remain replaceable extensions.

> ShapeForge is not intended to replace Blender or other general-purpose modelling software. Its goal is to provide a deterministic semantic layer between high-level asset descriptions and engine-native objects.

## Why ShapeForge?

Traditional procedural generation APIs expose engine-specific primitives, while generated mesh pipelines often produce assets that are difficult for tools or language models to understand and modify reliably.

ShapeForge separates the problem into explicit layers:

```text
Code / JSON / Editor Tool / External AI
                    |
                    v
       Semantic Specification (optional)
                    |
                    v
             Shape Definition
                    |
          validation and styles
                    |
                    v
         Engine Generation Adapter
                    |
                    v
        Editable Native Hierarchy
```

This provides several useful properties:

- **Deterministic generation** — the same validated definition produces the same model structure.
- **Engine independence** — Schema and Core contain no Unity dependencies.
- **Editable output** — generated Unity models are normal `GameObject` hierarchies rather than opaque imported assets.
- **LLM-friendly authoring** — external AI produces constrained, versioned data instead of arbitrary mesh topology.
- **Discoverable capabilities** — a backend can publish exactly which shapes, parameters, limits, and costs it supports.
- **Semantic compilation** — domain templates can expose readable controls such as body proportions and hair shape, then compile them into ordinary ShapeForge nodes.
- **Motion-ready structure** — stable node IDs and explicit pivot groups allow external animation systems to resolve and control generated parts.

## Repository Structure

ShapeForge is divided into four packages with deliberately narrow responsibilities.

### `com.shapeforge.schema`

Defines the versioned, engine-neutral data contracts used to describe models and styles.

It includes hierarchical shape definitions, node IDs, transforms, geometry parameters, style documents, optional style inheritance references, Draft 2020-12 JSON Schemas, and compact examples for external tools and LLM prompts.

The Schema package references neither Unity nor a specific JSON library.

### `com.shapeforge.core`

Provides framework behavior and extension contracts without depending on a geometry implementation or rendering engine.

It includes validation, fluent authoring, cached style inheritance, palette-role resolution, engine-neutral transform contracts, geometry capability descriptions, semantic-template compilation, discovery catalogs, and reusable backend-independent abstractions.

Core owns framework concepts such as `core/group`, but it does not know what a low-poly wedge, character, building, or Unity `GameObject` is.

### `com.shapeforge.unity`

Provides the reference Unity integration.

It includes JSON serialization, validated semantic-specification boundaries, conversion from ShapeForge definitions to Unity hierarchies, lifecycle-safe appearance restoration, shared render resources, prepared generation plans, and stable node-ID-to-`Transform` resolution through `UnityShapeModel`.

Every generated root contains a `UnityShapeModel`. External motion code can resolve an `IShapeTransformTarget` once by node ID and cache it instead of traversing the hierarchy every frame.

### `com.shapeforge.lowpoly`

The first official ShapeForge geometry implementation and showcase package.

It includes ten discoverable geometry capabilities, cached meshes and materials, procedural flat-shaded geometry, palettes and inheritable styles, runtime generation pipelines, count- and time-budgeted batch generation, a Stylized Human semantic template, normalized reference-image measurement mapping, Editor presets, diagnostics, and animated hierarchy examples.

Low Poly demonstrates how an implementation can use ShapeForge contracts. Other packages can provide entirely different geometry, materials, or asset categories without modifying Schema or Core.

## Core Model

A ShapeForge model is a tree of nodes. A node may be either:

- a **group**, used for hierarchy, organization, transforms, and animation pivots; or
- a **shape**, interpreted by the selected geometry backend.

Each node can carry a stable ID, display name, local transform, geometry type and parameters, optional profile/path/section data, style or semantic palette role, and child nodes.

The resulting engine hierarchy preserves those relationships:

```text
Fantasy Hero
├── Pelvis Pivot
│   ├── Torso
│   ├── Left Leg Pivot
│   │   ├── Left Leg
│   │   └── Left Boot
│   └── Right Leg Pivot
├── Spine Pivot
│   ├── Head Pivot
│   │   ├── Head
│   │   └── Hair
│   ├── Left Shoulder Pivot
│   └── Right Shoulder Pivot
└── Accessories
```

This structure is useful for inspection, customization, attachment points, lightweight rigid-part animation, and future motion tooling.

## Geometry Capability Discovery

ShapeForge does not assume every backend supports the same primitives.

Implementations expose their authoring surface through `IShapeCapabilityCatalog`. A capability can describe:

- the stable shape type ID;
- intended uses and limitations;
- numeric parameters and supported ranges;
- required profile, path, or section counts;
- generation-cost behavior;
- guidance suitable for tools and external AI.

The Low Poly package exposes its cached catalog through `LowPolyShapeCapabilityCatalog.Instance`. Complete versioned capability documents can be serialized when an external tool needs machine-readable discovery.

This allows an LLM or editor extension to ask what the backend can actually generate before authoring a definition, rather than guessing unsupported types or parameters.

## Semantic Templates

Raw shape definitions are useful for precise procedural authoring, but they are often too detailed for high-level asset requests.

ShapeForge supports optional semantic templates through `ShapeTemplate<TSpecification>`.

A template accepts a domain-readable specification and compiles it into a normal validated `ShapeDefinition`:

```text
Stylized Human Specification
- body proportions
- head dimensions
- jaw width
- hair volume
- hair parting
- fringe length
- sideburn length
          |
          v
LowPolyStylizedHumanTemplate
          |
          v
Articulated Shape Definition
```

Templates do not add character-, furniture-, or building-specific concepts to ShapeForge Core. Each implementation package owns its specification, validator, schema, compiler, and discovery metadata.

`ShapeTemplateCatalog` provides cached exact-ID lookup and can publish versioned descriptors containing categories, tags, required shape capabilities, and the matching specification schema.

## Reference-Image Mapping

Schema and Core provide a generic `shapeforge.reference/1.0` contract for any model category. Each semantic part can carry aligned front, side, and back bounds, confidence, and an optional ordered silhouette. Core validates observations, reports missing coverage or cross-view height disagreement, and publishes provider-neutral extraction instructions that forbid inventing hidden geometry.

The Low Poly package includes a deterministic reference-measurement pipeline for the Stylized Human template.

The bundled Fantasy Hero uses authored, semantically aligned profile cages for a coherent rounded head and primary hair volume. The same template can still consume aligned front, side, and back silhouettes when a caller explicitly supplies a measured reference.

`LowPolyStylizedHumanReferenceMapper` maps normalized observations from a front view and an optional side view into a `LowPolyStylizedHumanSpecification`.

Current measurements include head dimensions, shoulder and torso width, leg length, jaw width, hair width and depth, hair parting, fringe length, and sideburn length.

Measurements are ratios rather than pixels, making them independent of image resolution. Missing side-view data deliberately preserves the base depth instead of inventing dimensions that are not visible.

A provider-neutral extraction protocol, JSON Schema, and example are published under:

```text
Packages/com.shapeforge.lowpoly/Documentation~/Templates
```

`LowPolyStylizedHumanReferencePrompt.Create` can combine the runtime protocol with the authoritative schema for a vision-capable external model. ShapeForge itself remains independent of OpenAI, Gemini, or any other AI provider.

## Styles and Appearance

Geometry and appearance are intentionally separate.

Nodes can reference semantic palette roles such as body, clothing, metal, wood, trim, or accent colors. Registered styles resolve those roles into concrete appearance values.

Styles may inherit from another style:

```text
Base Fantasy Style
├── body
├── clothing
├── leather
└── metal

Dark Variant
└── overrides clothing and accent only
```

`ShapeStyleResolver` validates missing parents and inheritance cycles during registration, flattens the result, and caches palette lookup for generation.

In Unity, reusable colors use cached shared materials. Explicit per-shape overrides use `MaterialPropertyBlock`, and a root appearance manifest restores renderer state safely across Unity lifecycle events.

## Runtime Generation

`LowPolyModelGenerator` is the reusable entry point for Low Poly runtime generation.

Typical workflows are:

- generate an already validated `ShapeDefinition`;
- deserialize and generate one external JSON document;
- parse JSON once and reuse the resulting immutable definition;
- prepare a generation plan when creating many instances;
- spread large batches over caller-owned count or elapsed-time budgets.

For one-off documents, `GenerateJson` performs parsing and validation for that call.

For repeated models, parse once and reuse the resulting definition. At the Unity Adapter level, prepared generation plans validate an immutable tree once and skip redundant validation for later instances.

`LowPolyGenerationBatch` supports `GenerateNext` and `GenerateForMilliseconds`. ShapeForge intentionally creates no hidden coroutine, iterator state machine, global runner, or background update loop—the caller owns scheduling and budgeting.

## Editor Tools and Presets

Use the Unity menu:

```text
ShapeForge > Generate
```

to preview showcase definitions with full Undo support.

The Low Poly package currently includes:

- **Sentinel Robot** — articulated mechanical hierarchy and pivot-driven movement;
- **Fantasy Hero** — a proportioned stylized human assembled from semantic and procedural parts;
- **Inventor Workbench** — detailed furniture and prop composition;
- **Japanese Town** — buildings, environment dressing, street furniture, and shrine elements in one generated hierarchy;
- animated variants demonstrating lightweight mechanical motion and a simple human walk.

The animated samples control cached `IShapeTransformTarget` instances without defining a ShapeForge animation-file format. Clips, tracks, interpolation, blending, retargeting, and playback remain responsibilities of a future dedicated motion layer.

## Motion Integration Boundary

ShapeForge does not currently provide a complete animation system. It provides the structural boundary required by one:

- hierarchical group and pivot nodes;
- stable node IDs;
- an engine-neutral `IShapeTransformResolver`;
- readable and writable `IShapeTransformTarget` values;
- Unity bindings persisted by `UnityShapeModel`.

This is enough for code-driven rigid-part animation and for a separate system—such as a future MotionForge package—to bind tracks to generated nodes.

ShapeForge remains responsible for model structure and target discovery. A motion package should own clips, tracks, keyframes, curves, quaternion interpolation, rest poses, blending, constraints, retargeting, playback, and serialization.

## Performance Design

The reference implementation avoids unnecessary per-instance resources:

- built-in primitive meshes are reused;
- equivalent procedural geometry parameters share cached meshes;
- reusable appearance uses shared materials;
- explicit overrides use property blocks;
- style inheritance is flattened and cached;
- capability and semantic-template catalogs use cached exact-ID lookup;
- repeated generation can reuse parsed definitions and prepared plans;
- motion examples resolve transform targets once rather than searching every frame.

Use:

```text
ShapeForge > Diagnostics > Benchmark JSON Generation
```

to measure parsing, prepared generation, managed heap growth, and shared render-resource counts on the current Editor machine. Results are local diagnostics, not cross-platform guarantees.

## JSON and Validation

ShapeForge documents are versioned and validated before generation.

Published Draft 2020-12 schemas and minimal examples live under package `Documentation~` directories. External tools should validate against the matching schema version before passing data to an engine adapter.

Semantic specifications add their own validator boundary. The Unity serializer requires the owning package's validator callback when deserializing template-owned data, preventing external documents from bypassing semantic validation.

## Current Scope

The repository currently focuses on:

- engine-neutral shape and style contracts;
- deterministic hierarchy generation;
- replaceable geometry backends;
- Unity as the reference adapter;
- Low Poly as the first official implementation;
- machine-readable capability discovery;
- semantic asset templates;
- LLM-friendly JSON workflows;
- normalized reference-image measurements;
- reusable styles and palettes;
- motion-ready pivots and stable transform bindings;
- runtime and Editor generation workflows.

## Non-Goals

ShapeForge is intentionally not trying to become:

- a general vertex or sculpting editor;
- a replacement for Blender, Maya, or Houdini;
- a universal FBX or OBJ asset importer;
- a full skinned-mesh character pipeline;
- a built-in LLM service client;
- a complete animation state machine or timeline editor;
- an opaque one-click generator with no inspectable intermediate representation.

The framework is most useful when an asset can be represented as meaningful parts, constraints, parameters, and styles that tools can validate and regenerate.

## Tests

The repository currently contains 63 first-party EditMode tests covering fluent authoring, validation, JSON contracts, style inheritance, capability discovery, semantic templates, Stylized Human compilation, reference mapping, runtime generation, hierarchy adaptation, appearance lifecycle behavior, shared resources, procedural geometry, presets, and motion-ready pivots.

## Project Status

ShapeForge is pre-1.0. Public APIs, schema details, and package boundaries may continue to evolve while the architecture is validated through additional asset categories and integrations.

Current known areas for future work include:

- additional semantic asset templates;
- richer reference calibration and automated landmark extraction;
- a formal motion-role and rig contract;
- quaternion-based motion targets;
- dedicated motion clip and playback packages;
- more engine adapters and geometry implementations;
- public release and compatibility policy.

## Requirements

Reference project Unity version:

```text
2022.3.62f3
```

## Design Principle

ShapeForge follows one central rule:

> High-level tools describe what an asset is; ShapeForge validates and compiles that meaning into deterministic, editable engine objects.

This keeps procedural generation, external AI, runtime tools, and engine integration connected without collapsing them into one monolithic modelling application.
