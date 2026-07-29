using UnityEngine;

namespace ShapeForge
{
    /// <summary>
    /// Generates Unity geometry for supported shape node types.
    /// </summary>
    public interface IShapeGenerator
    {
        /// <summary>
        /// Determines whether this generator supports a shape node.
        /// </summary>
        bool CanGenerate(ShapeNode node);

        /// <summary>
        /// Generates the Unity object representing a supported shape node.
        /// </summary>
        GameObject Generate(ShapeNode node, ShapeGenerationContext context);
    }
}
