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

            Assert.That(generatedRoot.name, Is.EqualTo("Pocket Fantasy Hero"));
            Assert.That(generatedRoot.GetComponentsInChildren<MeshRenderer>(), Has.Length.EqualTo(31));
            Assert.That(generatedRoot.transform.Find("Head Pivot/Unified Polygon Hair Shell"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Left Ear"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Left Ear (Mirror X)"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Pelvis Pivot/Spine Pivot/Left Cropped Jacket"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Pelvis Pivot/Spine Pivot/Long Black Shirt"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Left Hip Pivot/Left Knee Pivot/Left Fitted Tall Boot"), Is.Not.Null);

            MeshFilter coat = generatedRoot.transform
                .Find("Pelvis Pivot/Spine Pivot/Long Black Shirt")
                .GetComponent<MeshFilter>();
            MeshFilter head = generatedRoot.transform
                .Find("Head Pivot/Sculpted Human Head")
                .GetComponent<MeshFilter>();
            MeshFilter hairMesh = generatedRoot.transform
                .Find("Head Pivot/Unified Polygon Hair Shell")
                .GetComponent<MeshFilter>();
            MeshFilter jacketPanel = generatedRoot.transform
                .Find("Pelvis Pivot/Spine Pivot/Left Cropped Jacket")
                .GetComponent<MeshFilter>();
            MeshFilter boot = generatedRoot.transform
                .Find("Left Hip Pivot/Left Knee Pivot/Left Tapered Boot")
                .GetComponent<MeshFilter>();

            Assert.That(coat.sharedMesh.name, Is.EqualTo("Low Poly Profile Loft"));
            Assert.That(head.sharedMesh.name, Is.EqualTo("Low Poly Lathe Profile"));
            Assert.That(hairMesh.sharedMesh.name, Is.EqualTo("Low Poly Profile Loft"));
            Assert.That(jacketPanel.sharedMesh.name, Is.EqualTo("Low Poly Extruded Profile"));
            Assert.That(boot.sharedMesh.name, Is.EqualTo("Low Poly Profile Loft"));

            Assert.That(style.Palette.TryGetColor("hair", out ForgeColor hair), Is.True);
            Assert.That(hair.B, Is.GreaterThan(hair.R));
            Assert.That(style.Palette.TryGetColor("jacket", out ForgeColor jacket), Is.True);
            Assert.That(jacket.R, Is.LessThan(0.05f));

            UnityShapeModel model = generatedRoot.GetComponent<UnityShapeModel>();
            Assert.That(model.TryGetTarget("hero.spine.pivot", out _), Is.True);
            Assert.That(model.TryGetTarget("hero.head.pivot", out _), Is.True);
            Assert.That(model.TryGetTarget("hero.leg.left.knee.pivot", out _), Is.True);
        }
    }
}
