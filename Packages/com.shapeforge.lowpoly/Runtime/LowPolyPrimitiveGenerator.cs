using System;
using System.Collections.Generic;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Generates official primitive and parameterized Low Poly shapes from cached render resources.
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
            return node != null &&
                   (PrimitiveTypes.ContainsKey(node.Type) ||
                    node.Type == LowPolyShapeTypes.Wedge ||
                    node.Type == LowPolyShapeTypes.Frustum ||
                    node.Type == LowPolyShapeTypes.ExtrudedProfile);
        }

        /// <inheritdoc />
        public GameObject Generate(ShapeNode node, ShapeGenerationContext context)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Mesh             mesh;
            Material         material;
            if (PrimitiveTypes.TryGetValue(node.Type, out PrimitiveType primitiveType))
            {
                PrimitiveTemplate template = UnityPrimitiveTemplateCache.Get(primitiveType);
                mesh     = template.Mesh;
                material = template.Material;
            }
            else
            {
                mesh     = GetProceduralMesh(node);
                material = UnityPrimitiveTemplateCache.Get(PrimitiveType.Cube).Material;
            }

            GameObject        generated    = new();
            MeshFilter        meshFilter   = generated.AddComponent<MeshFilter>();
            MeshRenderer      meshRenderer = generated.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh       = mesh;
            meshRenderer.sharedMaterial = material;
            return generated;
        }

        private static Mesh GetProceduralMesh(ShapeNode node)
        {
            if (node.Type == LowPolyShapeTypes.Wedge)
                return LowPolyProceduralMeshCache.GetWedge();

            if (node.Type == LowPolyShapeTypes.ExtrudedProfile)
            {
                float depth = GetPositiveParameter(node, LowPolyShapeParameters.ProfileDepth, 0.2f);
                return LowPolyProceduralMeshCache.GetExtrudedProfile(node.Profile, depth);
            }

            if (node.Type != LowPolyShapeTypes.Frustum)
                throw new ArgumentException($"Unsupported Low Poly shape type '{node.Type}'.", nameof(node));

            float topWidth    = GetPositiveParameter(node, LowPolyShapeParameters.TopWidth, 0.65f);
            float topDepth    = GetPositiveParameter(node, LowPolyShapeParameters.TopDepth, 0.65f);
            float bottomWidth = GetPositiveParameter(node, LowPolyShapeParameters.BottomWidth, 1f);
            float bottomDepth = GetPositiveParameter(node, LowPolyShapeParameters.BottomDepth, 1f);
            return LowPolyProceduralMeshCache.GetFrustum(topWidth, topDepth, bottomWidth, bottomDepth);
        }

        private static float GetPositiveParameter(ShapeNode node, string name, float defaultValue)
        {
            if (!node.Parameters.TryGetValue(name, out float value))
                return defaultValue;

            if (value <= 0f)
                throw new ArgumentOutOfRangeException(name, value, "Procedural dimensions must be positive.");

            return value;
        }
    }
}
