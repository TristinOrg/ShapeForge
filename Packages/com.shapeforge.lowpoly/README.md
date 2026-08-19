# ShapeForge Low Poly

The first official ShapeForge implementation. It provides Low Poly geometry, palettes, styles, and reusable presets for characters, furniture, buildings, props, and other model categories through ShapeForge extension contracts.

Built-in Cube, Sphere, Cylinder, and Capsule shapes reuse cached Unity meshes and base materials without generating Collider components. Parameterized Wedge and Frustum shapes use flat-shaded procedural meshes; equivalent parameter sets share one cached Mesh. Profile Cage joins ordered sections whose closed profiles may differ at every depth, enabling asymmetric shells that Profile Loft cannot express. Bounded cached interpolation adds rounded continuity without expanding authoring JSON.

`LowPolyReferenceProfileCageMapper` deterministically combines generic front, side, and optional back silhouettes into Profile Cage sections. Bounded resampling runs only during generation; generated geometry continues through the shared mesh cache with no per-frame work.

The Fantasy Hero uses authored, semantically aligned profile cages for a coherent rounded head and unified hair volume, while articulated clothing and boots use rounded procedural profiles. `LowPolyStylizedHumanTemplate.Compile` also accepts a caller-supplied multi-view reference definition when measured reconstruction is needed.

`LowPolyHumanoidHeroPreset` starts from the complete `LowPolyHeroPreset` definition, preserves every authored appearance node, then reparents its pivots and inserts the missing Chest, Neck, Hand, and Foot bones required by `UnityHumanoidAvatarBuilder`. The original display preset remains unchanged.

Use `LowPolyShapeCapabilityCatalog.Instance` to query all twelve supported geometry types without reflection. `TryGet` performs a cached exact-ID lookup. Call `CreateDocument` and serialize it through the Unity Adapter only when external tools need the complete machine-readable catalog.

`LowPolyStylizedHumanTemplate` is the first optional semantic compiler. It accepts a validated `LowPolyStylizedHumanSpecification` with readable body, head, and hair controls, then produces the same articulated Shape Definition used by the Hero preset. Its Draft 2020-12 Schema and prompt-ready example live under `Documentation~/Templates`.

External tools can pass a generic, validated `ShapeReferenceDefinition` to the template when measured reconstruction is needed. Low Poly maps aligned `character/head` and `character/hair` observations through the shared reference-to-profile-cage path rather than maintaining a second character-only reference protocol.

Use `LowPolyModelGenerator` as the reusable runtime entry point for validated definitions or external ShapeForge JSON. Register style documents once, then reuse the same pipeline for subsequent models.

Applications can configure `ShapeValidationLimits` for untrusted documents and use `Regenerate` or `RegenerateJson` to replace an existing model only after its new hierarchy completes successfully.

For repeated models, call `ParseJson` once and pass the returned `ShapeDefinition` to `Generate`. `GenerateJson` is intended for one-off documents because it parses and validates on every call.

For a large number of instances, create a `LowPolyGenerationBatch` and call `GenerateNext` or `GenerateForMilliseconds` from an existing update or loading scheduler. The caller owns the count or elapsed-time budget; ShapeForge creates no coroutine, iterator state machine, global runner, or hidden update loop.

Use `ShapeForge > Generate` in the Unity Editor to preview the articulated Sentinel Robot, Fantasy Hero, detailed Inventor Workbench, compact Japanese Town, and dense Shibuya Crossing presets with full Undo support. Shibuya combines diagonal scramble markings, four commercial towers, media screens, signals, signage, and a lightweight crossing crowd. Select an imported Humanoid `AnimationClip`, then use `Animated Humanoid T-Pose Hero From Selected Clip` to create a persistent Avatar, Animator Controller, and playable retargeting preview with root motion enabled. The animated variants add mechanical and human walking examples that control cached `IShapeTransformTarget` instances without defining a ShapeForge animation format; a future motion system remains responsible for clips, tracks, interpolation, and serialization.

Use `ShapeForge > Generate > Noctis Chibi Reference Experiment` for the tuned 3.5-head character, or `ShapeForge > Experiments > Render Noctis Chibi Reference Views` to write deterministic front, three-quarter, side, back, top, and bottom PNG captures under `Library/ShapeForgeExperiments/NoctisChibi/Renders`. This preset validates recognizable low-poly reconstruction; it does not replace freeform sculpting, skinning, texture painting, or strand-level hair authoring.

Preset menu commands persist their procedural meshes under `Assets/ShapeForge/Generated` before returning the hierarchy. Keep the generated mesh asset alongside any Prefab created from that hierarchy; built-in Unity primitive meshes are not duplicated.

Use `ShapeForge > Diagnostics > Benchmark JSON Generation` to measure the end-to-end JSON generation path, managed heap growth, and shared render resources on the current Editor machine. Treat results as a local baseline, not a cross-platform guarantee.
