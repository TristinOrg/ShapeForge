# ShapeForge Low Poly

The first official ShapeForge implementation. It provides Low Poly geometry, palettes, styles, and reusable presets for characters, furniture, buildings, props, and other model categories through ShapeForge extension contracts.

Built-in Cube, Sphere, Cylinder, and Capsule shapes reuse cached Unity meshes and base materials without generating Collider components. Parameterized Wedge and Frustum shapes use flat-shaded procedural meshes; equivalent parameter sets share one cached Mesh. Profile Cage joins ordered sections whose closed profiles may differ at every depth, enabling asymmetric shells that Profile Loft cannot express. Bounded cached interpolation adds rounded continuity without expanding authoring JSON.

`LowPolyReferenceProfileCageMapper` deterministically combines generic front, side, and optional back silhouettes into Profile Cage sections. Bounded resampling runs only during generation; generated geometry continues through the shared mesh cache with no per-frame work.

Use `LowPolyShapeCapabilityCatalog.Instance` to query all eleven supported geometry types without reflection. `TryGet` performs a cached exact-ID lookup. Call `CreateDocument` and serialize it through the Unity Adapter only when external tools need the complete machine-readable catalog.

`LowPolyStylizedHumanTemplate` is the first optional semantic compiler. It accepts a validated `LowPolyStylizedHumanSpecification` with readable body, head, and hair controls, then produces the same articulated Shape Definition used by the Hero preset. Its Draft 2020-12 Schema and prompt-ready example live under `Documentation~/Templates`.

`LowPolyStylizedHumanReferenceMapper` converts normalized front-view measurements and optional side-view measurements into that semantic specification. Measurements are ratios rather than pixels, making them resolution-independent and straightforward for external LLMs to author. Missing side-view data deliberately preserves the base head depth instead of guessing an invisible dimension. A versioned reference Schema, example, and provider-neutral extraction guide live under `Documentation~/Templates`; `LowPolyStylizedHumanReferencePrompt.Create` exposes the compact protocol to integrations.

Call `LowPolyStylizedHumanReferenceAnalyzer.Analyze` before mapping to inspect deterministic baseline deviations and confirm whether a side view constrains all currently supported geometric dimensions. A valid report describes representable measurements; it does not claim visual similarity for unsupported clothing, topology, palette, or back-view details.

Use `LowPolyModelGenerator` as the reusable runtime entry point for validated definitions or external ShapeForge JSON. Register style documents once, then reuse the same pipeline for subsequent models.

For repeated models, call `ParseJson` once and pass the returned `ShapeDefinition` to `Generate`. `GenerateJson` is intended for one-off documents because it parses and validates on every call.

For a large number of instances, create a `LowPolyGenerationBatch` and call `GenerateNext` or `GenerateForMilliseconds` from an existing update or loading scheduler. The caller owns the count or elapsed-time budget; ShapeForge creates no coroutine, iterator state machine, global runner, or hidden update loop.

Use `ShapeForge > Generate` in the Unity Editor to preview the articulated Sentinel Robot, realistically proportioned Fantasy Hero, detailed Inventor Workbench, and compact Japanese Town presets with full Undo support. The town combines buildings, environment dressing, street furniture, and shrine elements in one 65-renderer hierarchy. The animated variants add lightweight mechanical and human walking examples that control cached `IShapeTransformTarget` instances without defining a ShapeForge animation format; a future motion system remains responsible for clips, tracks, interpolation, and serialization.

Use `ShapeForge > Diagnostics > Benchmark JSON Generation` to measure the end-to-end JSON generation path, managed heap growth, and shared render resources on the current Editor machine. Treat results as a local baseline, not a cross-platform guarantee.
