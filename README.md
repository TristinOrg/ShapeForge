# ShapeForge

ShapeForge is an engine-agnostic procedural shape specification with a reference Unity implementation. It lets developers describe models as versioned hierarchies of shapes, transforms, appearances, and pivots, then generate editable engine objects through replaceable adapters.

Low Poly will be the first official implementation and sample. It does not define the framework: developers can provide their own geometry, materials, palettes, styles, and generation backends without modifying ShapeForge Core.

## Direction

```text
Code / JSON / Editor / External AI
                 |
                 v
          Shape Definition
                 |
                 v
          Engine Adapter
                 |
                 v
     Native Object Hierarchy
```

External AI integrations remain outside the schema and core packages. They produce validated ShapeForge documents through the same public JSON format used by scripts and authoring tools.

Draft 2020-12 JSON Schema documents and minimal prompt examples live in `Packages/com.shapeforge.schema/Documentation~`. External tools should validate generated documents against the matching versioned schema before passing them to an engine adapter.

## Initial scope

- An engine-agnostic schema package with versioned shape and style documents.
- A dependency-light core package with validation and extension contracts.
- A Unity adapter providing the reference JSON and hierarchy implementation.
- An official Low Poly package implementing the contracts.
- Hierarchical pivots for simple modular animation.
- Customizable appearance, palettes, and styles separated from geometry.
- Inheritable styles that override semantic palette roles without duplicating complete palettes.
- EditMode tests for core behavior and package samples for developer workflows.

Unity version: `2022.3.62f3`.
