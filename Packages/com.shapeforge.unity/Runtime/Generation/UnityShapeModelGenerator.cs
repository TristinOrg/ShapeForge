using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeForge.Unity
{
    /// <summary>
    /// Adapts a validated ShapeForge definition to an editable Unity hierarchy.
    /// </summary>
    public sealed class UnityShapeModelGenerator
    {
        private readonly List<IUnityShapeGenerator> generators = new List<IUnityShapeGenerator>();
        private readonly IShapeStyleResolver        styleResolver;
        private readonly ShapeDefinitionValidator  validator = new ShapeDefinitionValidator();

        /// <summary>
        /// Initializes a Unity model generator with explicit shape implementations.
        /// </summary>
        public UnityShapeModelGenerator(
            IEnumerable<IUnityShapeGenerator> generators,
            IShapeStyleResolver                styleResolver = null)
        {
            if (generators == null)
                throw new ArgumentNullException(nameof(generators));

            this.styleResolver = styleResolver;

            foreach (IUnityShapeGenerator generator in generators)
            {
                if (generator == null)
                    throw new ArgumentException("Generators cannot contain null entries.", nameof(generators));

                this.generators.Add(generator);
            }
        }

        /// <summary>
        /// Generates a Unity hierarchy under an optional parent transform.
        /// </summary>
        public GameObject Generate(ShapeDefinition definition, Transform parent = null)
        {
            validator.Validate(definition);

            ShapeGenerationContext context = new ShapeGenerationContext(definition, styleResolver);
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
                throw new UnityShapeGenerationException($"Generator returned null for node '{node.Id}'.");

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
            foreach (IUnityShapeGenerator generator in generators)
            {
                if (generator.CanGenerate(node))
                    return generator.Generate(node, context);
            }

            throw new UnityShapeGenerationException($"No Unity generator supports shape type '{node.Type}'.");
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
