using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeForge
{
    /// <summary>
    /// Builds an editable Unity hierarchy from a shape definition and explicit generators.
    /// </summary>
    public sealed class ShapeModelGenerator
    {
        private readonly List<IShapeGenerator> generators = new List<IShapeGenerator>();
        private readonly IShapeStyleResolver   styleResolver;

        /// <summary>
        /// Initializes a model generator with its available shape implementations.
        /// </summary>
        public ShapeModelGenerator(
            IEnumerable<IShapeGenerator> generators,
            IShapeStyleResolver           styleResolver = null)
        {
            if (generators == null)
                throw new ArgumentNullException(nameof(generators));

            this.styleResolver = styleResolver;

            foreach (IShapeGenerator generator in generators)
            {
                if (generator == null)
                    throw new ArgumentException("Generators cannot contain null entries.", nameof(generators));

                this.generators.Add(generator);
            }
        }

        /// <summary>
        /// Generates a model hierarchy under an optional parent transform.
        /// </summary>
        public GameObject Generate(ShapeDefinition definition, Transform parent = null)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            HashSet<string>         nodeIds = new HashSet<string>(StringComparer.Ordinal);
            ShapeGenerationContext context = new ShapeGenerationContext(definition, styleResolver);

            ValidateTree(definition.Root, nodeIds);
            return GenerateNode(definition.Root, parent, context);
        }

        private GameObject GenerateNode(
            ShapeNode              node,
            Transform              parent,
            ShapeGenerationContext context)
        {
            GameObject generated = node.Type == ShapeTypes.Group
                ? new GameObject(node.Name)
                : GenerateGeometry(node, context);

            if (generated == null)
                throw new ShapeGenerationException($"Generator returned null for node '{node.Id}'.");

            generated.name = node.Name;
            generated.transform.SetParent(parent, false);
            node.Transform.ApplyTo(generated.transform);

            try
            {
                foreach (ShapeNode child in node.Children)
                    GenerateNode(child, generated.transform, context);
            }
            catch
            {
                DestroyGeneratedObject(generated);
                throw;
            }

            return generated;
        }

        private GameObject GenerateGeometry(ShapeNode node, ShapeGenerationContext context)
        {
            foreach (IShapeGenerator generator in generators)
            {
                if (generator.CanGenerate(node))
                    return generator.Generate(node, context);
            }

            throw new ShapeGenerationException($"No generator supports shape type '{node.Type}'.");
        }

        private static void ValidateTree(ShapeNode node, HashSet<string> nodeIds)
        {
            if (node == null)
                throw new ShapeGenerationException("Shape definitions cannot contain null nodes.");

            if (string.IsNullOrWhiteSpace(node.Id))
                throw new ShapeGenerationException("Every shape node requires a stable ID.");

            if (!nodeIds.Add(node.Id))
                throw new ShapeGenerationException($"Duplicate shape node ID '{node.Id}'.");

            if (string.IsNullOrWhiteSpace(node.Type))
                throw new ShapeGenerationException($"Shape node '{node.Id}' requires a type.");

            foreach (ShapeNode child in node.Children)
                ValidateTree(child, nodeIds);
        }

        private static void DestroyGeneratedObject(GameObject generated)
        {
            if (Application.isPlaying)
                UnityEngine.Object.Destroy(generated);
            else
                UnityEngine.Object.DestroyImmediate(generated);
        }
    }
}
