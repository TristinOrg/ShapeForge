using NUnit.Framework;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies the articulated fantasy hero through the complete generation pipeline.
    /// </summary>
    public sealed class LowPolyHeroPresetTests
    {
        private GameObject generatedRoot;

        [TearDown]
        public void TearDown()
        {
            if (generatedRoot != null)
                Object.DestroyImmediate(generatedRoot);
        }

        [Test]
        public void GenerateCreatesHumanProportionsAndMotionTargets()
        {
            ShapeDefinition          definition = LowPolyHeroPreset.CreateDefinition();
            ShapeStyleDefinition     style      = LowPolyHeroPreset.CreateStyle();
            ShapeStyleResolver       resolver   = new(new[] { style });
            UnityShapeModelGenerator generator  = new(
                new IUnityShapeGenerator[] { new LowPolyPrimitiveGenerator() },
                resolver);

            generatedRoot = generator.Generate(definition);

            Assert.That(generatedRoot.name, Is.EqualTo("Fantasy Hero"));
            Assert.That(generatedRoot.GetComponentsInChildren<MeshRenderer>(), Has.Length.EqualTo(35));
            Assert.That(generatedRoot.transform.Find("Head Pivot/Layered Hair Crown"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Pelvis Pivot/Spine Pivot/Fitted Coat"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Left Hip Pivot/Left Knee Pivot/Left Boot"), Is.Not.Null);

            UnityShapeModel model = generatedRoot.GetComponent<UnityShapeModel>();
            Assert.That(model.TryGetTarget("hero.spine.pivot", out _), Is.True);
            Assert.That(model.TryGetTarget("hero.head.pivot", out _), Is.True);
            Assert.That(model.TryGetTarget("hero.leg.left.knee.pivot", out _), Is.True);
        }
    }
}
