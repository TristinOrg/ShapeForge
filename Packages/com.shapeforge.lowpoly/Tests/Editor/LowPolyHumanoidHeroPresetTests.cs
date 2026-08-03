using NUnit.Framework;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies the rigid-part Humanoid validation preset through the Unity generation pipeline.
    /// </summary>
    public sealed class LowPolyHumanoidHeroPresetTests
    {
        private GameObject generatedRoot;
        private Avatar     avatar;

        [TearDown]
        public void TearDown()
        {
            if (avatar != null)
                Object.DestroyImmediate(avatar);

            if (generatedRoot != null)
                Object.DestroyImmediate(generatedRoot);
        }

        [Test]
        public void DefinitionProvidesCanonicalTposeHierarchy()
        {
            ShapeDefinition definition = LowPolyHumanoidHeroPreset.CreateDefinition();

            ShapeHumanoidRig.ValidateRequiredRoles(definition.Rig);

            ShapeNode chest         = FindNode(definition.Root, "humanoid-hero.chest");
            ShapeNode leftUpperArm  = FindNode(definition.Root, "humanoid-hero.left-upper-arm");
            ShapeNode rightUpperArm = FindNode(definition.Root, "humanoid-hero.right-upper-arm");
            ShapeNode leftArmMesh   = FindNode(definition.Root, "humanoid-hero.left-upper-mesh");
            ShapeNode rightArmMesh  = FindNode(definition.Root, "humanoid-hero.right-upper-mesh");
            ShapeNode leftGlove     = FindNode(definition.Root, "humanoid-hero.left-glove");
            ShapeNode leftBootSole  = FindNode(definition.Root, "humanoid-hero.left-sole");
            Assert.That(chest.Children, Does.Contain(leftUpperArm));
            Assert.That(chest.Children, Does.Contain(rightUpperArm));
            Assert.That(leftUpperArm.Transform.Position.X, Is.LessThan(0f));
            Assert.That(rightUpperArm.Transform.Position.X, Is.GreaterThan(0f));
            Assert.That(leftArmMesh.Transform.EulerAngles.Z, Is.EqualTo(90f));
            Assert.That(rightArmMesh.Transform.EulerAngles.Z, Is.EqualTo(90f));
            Assert.That(leftGlove.Appearance.ColorRole, Is.EqualTo("glove"));
            Assert.That(leftBootSole.Appearance.ColorRole, Is.EqualTo("sole"));
        }

        [Test]
        public void GenerateBuildsValidRigidHumanoidAvatar()
        {
            ShapeDefinition      definition = LowPolyHumanoidHeroPreset.CreateDefinition();
            LowPolyModelGenerator generator  = new(new[] { LowPolyHumanoidHeroPreset.CreateStyle() });

            generatedRoot = generator.Generate(definition);
            avatar        = UnityHumanoidAvatarBuilder.CreateAvatar(
                generatedRoot.GetComponent<UnityShapeModel>(),
                definition.Rig);

            Assert.That(generatedRoot.GetComponentsInChildren<MeshRenderer>(), Has.Length.GreaterThan(18));
            Assert.That(avatar.isValid, Is.True);
            Assert.That(avatar.isHuman, Is.True);
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
