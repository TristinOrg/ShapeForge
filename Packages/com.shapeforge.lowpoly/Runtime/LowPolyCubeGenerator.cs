using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Generates the official Low Poly cube implementation.
    /// </summary>
    public sealed class LowPolyCubeGenerator : IUnityShapeGenerator
    {
        private readonly LowPolyPrimitiveGenerator generator = new();

        /// <inheritdoc />
        public bool CanGenerate(ShapeNode node)
        {
            return node != null && node.Type == LowPolyShapeTypes.Cube;
        }

        /// <inheritdoc />
        public GameObject Generate(ShapeNode node, ShapeGenerationContext context)
        {
            return generator.Generate(node, context);
        }
    }
}
