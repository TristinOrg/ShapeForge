# ShapeForge Low Poly

The first official ShapeForge implementation. It provides Low Poly geometry, palettes, styles, and reusable presets for characters, furniture, buildings, props, and other model categories through ShapeForge extension contracts.

Built-in Cube, Sphere, Cylinder, and Capsule shapes reuse cached Unity meshes and base materials without generating Collider components.

Use `LowPolyModelGenerator` as the reusable runtime entry point for validated definitions or external ShapeForge JSON. Register style documents once, then reuse the same pipeline for subsequent models.

For repeated models, call `ParseJson` once and pass the returned `ShapeDefinition` to `Generate`. `GenerateJson` is intended for one-off documents because it parses and validates on every call.

For a large number of instances, create a `LowPolyGenerationBatch` and start its `GenerateOverFrames` enumerator as a coroutine. The caller selects `modelsPerFrame`; no global runner or hidden update loop is installed.

Use `ShapeForge > Generate` in the Unity Editor to preview the official Table and Robot presets with full Undo support.

Use `ShapeForge > Diagnostics > Benchmark JSON Generation` to measure the end-to-end JSON generation path, managed heap growth, and shared render resources on the current Editor machine. Treat results as a local baseline, not a cross-platform guarantee.
