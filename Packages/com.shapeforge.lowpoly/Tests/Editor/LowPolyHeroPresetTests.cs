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
            ShapeRigIndex rig = new(definition.Rig);

            generatedRoot = generator.Generate(definition);

            Assert.That(generatedRoot.name, Is.EqualTo("Pocket Fantasy Hero"));
            Assert.That(rig.TryGetNodeId(ShapeRigRoles.Hips, out string hipsId), Is.True);
            Assert.That(hipsId, Is.EqualTo("hero.pelvis.pivot"));
            Assert.That(rig.ConstrainRotationOffset(ShapeRigRoles.LeftKnee, new(-180f, 15f, 15f)),
                Is.EqualTo(new ForgeVector3(-120f, 0f, 0f)));
            Assert.That(generatedRoot.GetComponentsInChildren<MeshRenderer>(), Has.Length.EqualTo(42));
            Assert.That(generatedRoot.transform.Find("Head Pivot/Reference Unified Hair Volume"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Left Ear"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Left Ear (Mirror X)"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Left Blue Gray Eye"), Is.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot/Subtle Mouth"), Is.Null);
            Assert.That(generatedRoot.transform.Find("Head Pivot").localScale, Is.EqualTo(Vector3.one * 0.67f));
            Assert.That(generatedRoot.transform.Find("Pelvis Pivot/Spine Pivot/Left Open Short Jacket"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Pelvis Pivot/Spine Pivot/Fitted Black Shirt"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Left Hip Pivot/Left Knee Pivot/Left Fitted Tall Boot"), Is.Not.Null);

            MeshFilter coat = generatedRoot.transform
                .Find("Pelvis Pivot/Spine Pivot/Fitted Black Shirt")
                .GetComponent<MeshFilter>();
            MeshFilter head = generatedRoot.transform
                .Find("Head Pivot/Reference Sculpted Head")
                .GetComponent<MeshFilter>();
            MeshFilter hairMesh = generatedRoot.transform
                .Find("Head Pivot/Reference Unified Hair Volume")
                .GetComponent<MeshFilter>();
            MeshFilter jacketPanel = generatedRoot.transform
                .Find("Pelvis Pivot/Spine Pivot/Left Open Short Jacket")
                .GetComponent<MeshFilter>();
            MeshFilter boot = generatedRoot.transform
                .Find("Left Hip Pivot/Left Knee Pivot/Left Long Toe Boot")
                .GetComponent<MeshFilter>();

            Assert.That(coat.sharedMesh.name, Is.EqualTo("Low Poly Profile Cage"));
            Assert.That(head.sharedMesh.name, Is.EqualTo("Low Poly Profile Cage"));
            Assert.That(hairMesh.sharedMesh.name, Is.EqualTo("Low Poly Profile Cage"));
            Assert.That(jacketPanel.sharedMesh.name, Is.EqualTo("Low Poly Extruded Profile"));
            Assert.That(boot.sharedMesh.name, Is.EqualTo("Low Poly Profile Cage"));

            Assert.That(style.Palette.TryGetColor("hair", out ForgeColor hair), Is.True);
            Assert.That(hair.B, Is.GreaterThan(hair.R));
            Assert.That(style.Palette.TryGetColor("jacket", out ForgeColor jacket), Is.True);
            Assert.That(jacket.R, Is.EqualTo(0.102f).Within(0.0001f));

            UnityShapeModel model = generatedRoot.GetComponent<UnityShapeModel>();
            Assert.That(model.TryGetTarget("hero.spine.pivot", out _), Is.True);
            Assert.That(model.TryGetTarget("hero.head.pivot", out _), Is.True);
            Assert.That(model.TryGetTarget("hero.leg.left.knee.pivot", out _), Is.True);
        }

        private static ShapeNode FindNode(ShapeNode node, string id)
        {
            if (node.Id == id)
                return node;

            foreach (ShapeNode child in node.Children)
            {
                ShapeNode result = FindNode(child, id);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
