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
            Assert.That(generatedRoot.GetComponentsInChildren<MeshRenderer>(), Has.Length.EqualTo(43));
            Assert.That(generatedRoot.transform.Find("Head Pivot/Layered Hair Crown"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Layered Back Hair"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Layered Front Hair"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Left Pupil"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Nose"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Mouth"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Pelvis Pivot/Spine Pivot/Fitted Coat"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Left Hip Pivot/Left Knee Pivot/Left Boot"), Is.Not.Null);

            MeshFilter coat = generatedRoot.transform
                .Find("Pelvis Pivot/Spine Pivot/Fitted Coat")
                .GetComponent<MeshFilter>();
            MeshFilter head = generatedRoot.transform
                .Find("Head Pivot/Sculpted Head")
                .GetComponent<MeshFilter>();
            MeshFilter coatTail = generatedRoot.transform
                .Find("Pelvis Pivot/Spine Pivot/Left Coat Tail")
                .GetComponent<MeshFilter>();
            MeshFilter boot = generatedRoot.transform
                .Find("Left Hip Pivot/Left Knee Pivot/Left Boot")
                .GetComponent<MeshFilter>();

            Assert.That(coat.sharedMesh.name, Is.EqualTo("Low Poly Profile Loft"));
            Assert.That(head.sharedMesh.name, Is.EqualTo("Low Poly Profile Loft"));
            Assert.That(coatTail.sharedMesh.name, Is.EqualTo("Low Poly Extruded Profile"));
            Assert.That(boot.sharedMesh.name, Is.EqualTo("Low Poly Profile Loft"));

            UnityShapeModel model = generatedRoot.GetComponent<UnityShapeModel>();
            Assert.That(model.TryGetTarget("hero.spine.pivot", out _), Is.True);
            Assert.That(model.TryGetTarget("hero.head.pivot", out _), Is.True);
            Assert.That(model.TryGetTarget("hero.leg.left.knee.pivot", out _), Is.True);
        }
    }
}
