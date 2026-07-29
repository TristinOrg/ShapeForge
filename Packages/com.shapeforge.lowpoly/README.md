# ShapeForge Low Poly

The first official ShapeForge implementation. It provides Low Poly geometry, palettes, styles, and reusable presets for characters, furniture, buildings, props, and other model categories through ShapeForge extension contracts.

Built-in Cube, Sphere, Cylinder, and Capsule shapes reuse cached Unity meshes and base materials without generating Collider components.

Use `LowPolyModelGenerator` as the reusable runtime entry point for validated definitions or external ShapeForge JSON. Register style documents once, then reuse the same pipeline for subsequent models.

Use `ShapeForge > Generate` in the Unity Editor to preview the official Table and Robot presets with full Undo support.
