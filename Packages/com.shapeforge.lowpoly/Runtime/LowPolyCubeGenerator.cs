using System;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Generates the official Low Poly cube implementation.
    /// </summary>
    public sealed class LowPolyCubeGenerator : IUnityShapeGenerator
    {
        /// <inheritdoc />
        public bool CanGenerate(ShapeNode node)
        {
            return node != null && node.Type == LowPolyShapeTypes.Cube;
        }

        /// <inheritdoc />
        public GameObject Generate(ShapeNode node, ShapeGenerationContext context)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Collider   collider   = gameObject.GetComponent<Collider>();
            Renderer   renderer   = gameObject.GetComponent<Renderer>();

            if (Application.isPlaying)
                UnityEngine.Object.Destroy(collider);
            else
                UnityEngine.Object.DestroyImmediate(collider);

            if (context.TryResolveColor(node, out ForgeColor color))
            {
                UnityShapeColor shapeColor = gameObject.AddComponent<UnityShapeColor>();
                shapeColor.Configure(renderer, color);
            }

            return gameObject;
        }
    }
}
