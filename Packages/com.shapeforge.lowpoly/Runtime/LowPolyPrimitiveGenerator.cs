using System;
using System.Collections.Generic;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Generates official Low Poly primitives from cached Unity render resources.
    /// </summary>
    public sealed class LowPolyPrimitiveGenerator : IUnityShapeGenerator
    {
        private static readonly Dictionary<string, PrimitiveType> PrimitiveTypes = new(StringComparer.Ordinal)
        {
            [LowPolyShapeTypes.Cube]     = PrimitiveType.Cube,
            [LowPolyShapeTypes.Sphere]   = PrimitiveType.Sphere,
            [LowPolyShapeTypes.Cylinder] = PrimitiveType.Cylinder,
            [LowPolyShapeTypes.Capsule]  = PrimitiveType.Capsule
        };

        /// <inheritdoc />
        public bool CanGenerate(ShapeNode node)
        {
            return node != null && PrimitiveTypes.ContainsKey(node.Type);
        }

        /// <inheritdoc />
        public GameObject Generate(ShapeNode node, ShapeGenerationContext context)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!PrimitiveTypes.TryGetValue(node.Type, out PrimitiveType primitiveType))
                throw new ArgumentException($"Unsupported Low Poly shape type '{node.Type}'.", nameof(node));

            PrimitiveTemplate template     = UnityPrimitiveTemplateCache.Get(primitiveType);
            GameObject        generated    = new();
            MeshFilter        meshFilter   = generated.AddComponent<MeshFilter>();
            MeshRenderer      meshRenderer = generated.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh       = template.Mesh;
            meshRenderer.sharedMaterial = template.Material;
            return generated;
        }
    }
}
