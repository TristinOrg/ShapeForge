# ShapeForge Schema

ShapeForge Schema contains engine-agnostic model and style contracts. It references neither Unity nor any JSON implementation.

`ShapeStyleDefinition.BaseStyle` optionally identifies another style document. This keeps style variants compact and readable for code, authoring tools, and external AI while leaving resolution policy in ShapeForge Core.
