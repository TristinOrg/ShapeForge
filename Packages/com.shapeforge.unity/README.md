# ShapeForge Unity Adapter

The Unity Adapter converts engine-agnostic ShapeForge definitions into Unity object hierarchies and provides the reference JSON implementation.

Generated appearance uses shared cached materials for reusable palette colors and a `MaterialPropertyBlock` only for explicit per-shape overrides. A single root manifest restores all renderer state across Unity lifecycle events.
