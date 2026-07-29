using NUnit.Framework;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies a non-character furniture model through the complete generation pipeline.
    /// </summary>
    public sealed class LowPolyTablePresetTests
    {
        private GameObject generatedRoot;

        [TearDown]
        public void TearDown()
        {
            if (generatedRoot != null)
                Object.DestroyImmediate(generatedRoot);
        }

        [Test]
        public void GenerateCreatesColoredEditableFurnitureHierarchy()
        {
            ShapeDefinition          definition = LowPolyTablePreset.CreateDefinition();
            ShapeStyleDefinition     style      = LowPolyTablePreset.CreateStyle();
            ShapeStyleResolver       resolver   = new ShapeStyleResolver(new[] { style });
            UnityShapeModelGenerator generator  = new UnityShapeModelGenerator(
                new IUnityShapeGenerator[] { new LowPolyCubeGenerator() },
                resolver);

            generatedRoot = generator.Generate(definition);

            Assert.That(generatedRoot.name, Is.EqualTo("Table"));
            Assert.That(generatedRoot.transform.childCount, Is.EqualTo(5));
            Assert.That(generatedRoot.transform.Find("Top"), Is.Not.Null);

            Renderer renderer = generatedRoot.transform.Find("Top").GetComponent<Renderer>();
            MaterialPropertyBlock properties = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(properties);
            Color color = properties.GetColor(Shader.PropertyToID("_Color"));
            Assert.That(color.r, Is.EqualTo(0.45f).Within(0.0001f));
            Assert.That(color.g, Is.EqualTo(0.22f).Within(0.0001f));
            Assert.That(color.b, Is.EqualTo(0.08f).Within(0.0001f));
            Assert.That(color.a, Is.EqualTo(1f).Within(0.0001f));
        }
    }
}
