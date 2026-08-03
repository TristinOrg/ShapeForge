using System.Collections.Generic;
using NUnit.Framework;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies the full Fantasy Hero appearance through the Unity Humanoid generation pipeline.
    /// </summary>
    public sealed class LowPolyHumanoidHeroPresetTests
    {
        private GameObject generatedRoot;
        private GameObject originalRoot;
        private Avatar     avatar;

        [TearDown]
        public void TearDown()
        {
            if (avatar != null)
                Object.DestroyImmediate(avatar);

            if (generatedRoot != null)
                Object.DestroyImmediate(generatedRoot);
            if (originalRoot != null)
                Object.DestroyImmediate(originalRoot);
        }

        [Test]
        public void DefinitionPreservesEveryFantasyHeroAppearanceNode()
        {
            ShapeDefinition original = LowPolyHeroPreset.CreateDefinition();
            ShapeDefinition humanoid = LowPolyHumanoidHeroPreset.CreateDefinition();
            HashSet<string> originalShapeIds = CollectShapeIds(original.Root);
            HashSet<string> humanoidShapeIds = CollectShapeIds(humanoid.Root);

            Assert.That(humanoidShapeIds, Is.EquivalentTo(originalShapeIds));
        }

        [Test]
        public void DefinitionProvidesCanonicalTposeHierarchy()
        {
            ShapeDefinition definition = LowPolyHumanoidHeroPreset.CreateDefinition();

            ShapeHumanoidRig.ValidateRequiredRoles(definition.Rig);

            ShapeNode chest         = FindNode(definition.Root, "hero.humanoid.chest");
            ShapeNode hips          = FindNode(definition.Root, "hero.pelvis.pivot");
            ShapeNode headVisuals   = FindNode(definition.Root, "hero.head.pivot.humanoid.visual");
            ShapeNode leftUpperArm  = FindNode(definition.Root, "hero.arm.left.shoulder.pivot");
            ShapeNode rightUpperArm = FindNode(definition.Root, "hero.arm.right.shoulder.pivot");
            Assert.That(chest.Children, Does.Contain(leftUpperArm));
            Assert.That(chest.Children, Does.Contain(rightUpperArm));
            Assert.That(leftUpperArm.Transform.EulerAngles.Z, Is.EqualTo(-90f));
            Assert.That(rightUpperArm.Transform.EulerAngles.Z, Is.EqualTo(90f));
            Assert.That(definition.Root.Transform.EulerAngles.Y, Is.Zero);
            Assert.That(hips.Transform.EulerAngles.Y, Is.Zero);
            Assert.That(headVisuals.Transform.EulerAngles.Y, Is.EqualTo(180f));
            Assert.That(FindNode(headVisuals, "hero.hair.fringe.primary"), Is.Not.Null);
        }

        [Test]
        public void GenerateBuildsValidRigidHumanoidAvatar()
        {
            ShapeDefinition       original   = LowPolyHeroPreset.CreateDefinition();
            ShapeDefinition       definition = LowPolyHumanoidHeroPreset.CreateDefinition();
            LowPolyModelGenerator generator  = new(new[] { LowPolyHumanoidHeroPreset.CreateStyle() });
            originalRoot = generator.Generate(original);

            generatedRoot = generator.Generate(definition);
            avatar        = UnityHumanoidAvatarBuilder.CreateAvatar(
                generatedRoot.GetComponent<UnityShapeModel>(),
                definition.Rig);

            Assert.That(
                generatedRoot.GetComponentsInChildren<MeshRenderer>().Length,
                Is.EqualTo(originalRoot.GetComponentsInChildren<MeshRenderer>().Length));
            Assert.That(avatar.isValid, Is.True);
            Assert.That(avatar.isHuman, Is.True);
        }

        private static HashSet<string> CollectShapeIds(ShapeNode root)
        {
            HashSet<string> ids = new();
            CollectShapeIds(root, ids);
            return ids;
        }

        private static void CollectShapeIds(ShapeNode node, ISet<string> ids)
        {
            if (node.Type != ShapeTypes.Group)
                ids.Add(node.Id);

            foreach (ShapeNode child in node.Children)
                CollectShapeIds(child, ids);
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
