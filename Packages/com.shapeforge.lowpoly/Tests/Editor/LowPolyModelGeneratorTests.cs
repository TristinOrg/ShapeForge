using NUnit.Framework;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies the reusable Low Poly pipeline for external ShapeForge JSON documents.
    /// </summary>
    public sealed class LowPolyModelGeneratorTests
    {
        private GameObject generatedRoot;

        [TearDown]
        public void TearDown()
        {
            if (generatedRoot != null)
                Object.DestroyImmediate(generatedRoot);
        }

        [Test]
        public void GenerateJsonReusesPipelineAndRegisteredStyle()
        {
            ShapeJsonSerializer   serializer = new();
            LowPolyModelGenerator generator  = new();
            generator.SetStyleJson(serializer.Serialize(LowPolyTablePreset.CreateStyle()));

            ShapeStyleDefinition replacementStyle = LowPolyTablePreset.CreateStyle();
            replacementStyle.Palette.Set("wood.primary", new(0.1f, 0.2f, 0.3f));
            generator.SetStyleJson(serializer.Serialize(replacementStyle));

            generatedRoot = generator.GenerateJson(serializer.Serialize(LowPolyTablePreset.CreateDefinition()));

            Renderer top = generatedRoot.transform.Find("Top").GetComponent<Renderer>();
            Assert.That(generatedRoot.transform.childCount, Is.EqualTo(5));
            Assert.That(top.sharedMaterial.color.r, Is.EqualTo(0.1f).Within(0.0001f));
            Assert.That(top.HasPropertyBlock(), Is.False);
        }

        [Test]
        public void GenerateNextHonorsPerStepModelBudget()
        {
            generatedRoot = new("Batch Root");
            LowPolyModelGenerator  generator  = new(new[] { LowPolyTablePreset.CreateStyle() });
            LowPolyGenerationBatch batch      = generator.CreateBatch(
                LowPolyTablePreset.CreateDefinition(),
                5,
                generatedRoot.transform);
            Assert.That(batch.GenerateNext(2), Is.EqualTo(2));
            Assert.That(batch.GeneratedCount, Is.EqualTo(2));
            Assert.That(batch.GenerateNext(2), Is.EqualTo(2));
            Assert.That(batch.GeneratedCount, Is.EqualTo(4));
            Assert.That(batch.GenerateNext(2), Is.EqualTo(1));
            Assert.That(batch.GeneratedCount, Is.EqualTo(5));
            Assert.That(batch.GenerateNext(2), Is.Zero);
            Assert.That(batch.IsCompleted, Is.True);
            Assert.That(generatedRoot.transform.childCount, Is.EqualTo(5));
        }
    }
}
