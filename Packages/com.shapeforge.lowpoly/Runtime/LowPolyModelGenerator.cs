using System;
using System.Collections.Generic;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Reuses a complete Low Poly pipeline for validated definitions and external JSON documents.
    /// </summary>
    public sealed class LowPolyModelGenerator
    {
        private readonly ShapeJsonSerializer      serializer = new();
        private readonly ShapeStyleResolver       styleResolver;
        private readonly UnityShapeModelGenerator modelGenerator;

        /// <summary>
        /// Initializes a reusable Low Poly generation pipeline.
        /// </summary>
        public LowPolyModelGenerator(IEnumerable<ShapeStyleDefinition> styles = null)
        {
            styleResolver  = new(styles ?? Array.Empty<ShapeStyleDefinition>());
            modelGenerator = new(
                new IUnityShapeGenerator[] { new LowPolyPrimitiveGenerator() },
                styleResolver);
        }

        /// <summary>
        /// Adds or replaces an engine-agnostic style for subsequent generations.
        /// </summary>
        public void SetStyle(ShapeStyleDefinition style)
        {
            styleResolver.Set(style);
        }

        /// <summary>
        /// Adds or replaces a validated style JSON document for subsequent generations.
        /// </summary>
        public void SetStyleJson(string json)
        {
            styleResolver.Set(serializer.DeserializeStyle(json));
        }

        /// <summary>
        /// Validates and generates a model from an engine-agnostic definition.
        /// </summary>
        public GameObject Generate(ShapeDefinition definition, Transform parent = null)
        {
            return modelGenerator.Generate(definition, parent);
        }

        /// <summary>
        /// Validates and generates a model directly from a ShapeForge JSON document.
        /// </summary>
        public GameObject GenerateJson(string json, Transform parent = null)
        {
            return Generate(serializer.DeserializeShape(json), parent);
        }
    }
}
