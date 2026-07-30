using NUnit.Framework;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies the modular robot preset and its transform-ready pivot hierarchy.
    /// </summary>
    public sealed class LowPolyRobotPresetTests
    {
        private GameObject generatedRoot;

        [TearDown]
        public void TearDown()
        {
            if (generatedRoot != null)
                Object.DestroyImmediate(generatedRoot);
        }

        [Test]
        public void DefinitionKeepsMotionTargetsSeparateFromGeometry()
        {
            ShapeDefinition definition = LowPolyRobotPreset.CreateDefinition();

            ShapeNode leftShoulder = FindNode(definition.Root, "robot.arm.left.shoulder.pivot");
            ShapeNode leftArm      = FindNode(definition.Root, "robot.arm.left.upper");

            Assert.That(leftShoulder, Is.Not.Null);
            Assert.That(leftArm, Is.Not.Null);
            Assert.That(leftShoulder.Type, Is.EqualTo(ShapeTypes.Group));
            Assert.That(FindNode(leftShoulder, leftArm.Id), Is.SameAs(leftArm));
            Assert.That(leftArm.Type, Is.EqualTo(LowPolyShapeTypes.Capsule));
        }

        [Test]
        public void GenerateCreatesColoredRobotWithEditablePivots()
        {
            ShapeDefinition          definition = LowPolyRobotPreset.CreateDefinition();
            ShapeStyleDefinition     style      = LowPolyRobotPreset.CreateStyle();
            ShapeStyleResolver       resolver   = new(new[] { style });
            UnityShapeModelGenerator generator  = new(
                new IUnityShapeGenerator[] { new LowPolyPrimitiveGenerator() },
                resolver);

            generatedRoot = generator.Generate(definition);

            Assert.That(generatedRoot.GetComponentsInChildren<MeshRenderer>(), Has.Length.EqualTo(27));
            Assert.That(generatedRoot.transform.Find("Left Shoulder Pivot/Left Elbow Pivot/Left Hand"), Is.Not.Null);
            Assert.That(generatedRoot.transform.Find("Right Hip Pivot/Right Knee Pivot/Right Foot"), Is.Not.Null);

            UnityShapeModel model = generatedRoot.GetComponent<UnityShapeModel>();
            Assert.That(model.TryGetTarget("robot.arm.left.elbow.pivot", out _), Is.True);
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
