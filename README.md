# ShapeForge

ShapeForge is an extensible procedural shape framework for Unity. It lets developers describe models as serializable hierarchies of shapes, transforms, appearances, and pivots, then generate editable Unity objects through replaceable generators.

Low Poly will be the first official implementation and sample. It does not define the framework: developers can provide their own geometry, materials, palettes, styles, and generation backends without modifying ShapeForge Core.

## Direction

```text
Code / JSON / Editor / External AI
                 |
                 v
          Shape Definition
                 |
                 v
      Style-aware Generator
                 |
                 v
      Unity Object Hierarchy
```

External AI integrations remain outside the core package. They produce validated ShapeForge definitions through the same public format used by scripts and authoring tools.

## Initial scope

- A dependency-light core package with serializable shape definitions and extension contracts.
- An official Low Poly package implementing the contracts.
- Hierarchical pivots for simple modular animation.
- Customizable appearance, palettes, and styles separated from geometry.
- EditMode tests for core behavior and package samples for developer workflows.

Unity version: `2022.3.62f3`.

