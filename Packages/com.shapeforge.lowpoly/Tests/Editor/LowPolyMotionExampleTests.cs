using NUnit.Framework;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies Low Poly motion examples through engine-neutral transform targets.
    /// </summary>
    public sealed class LowPolyMotionExampleTests
    {
        private GameObject generatedRoot;

        [TearDown]
        public void TearDown()
        {
            if (generatedRoot != null)
                Object.DestroyImmediate(generatedRoot);
        }

        [Test]
        public void RobotExampleAnimatesArticulatedTargets()
        {
            generatedRoot = Generate(
                LowPolyRobotPreset.CreateDefinition(),
                LowPolyRobotPreset.CreateStyle());
            UnityShapeModel model = generatedRoot.GetComponent<UnityShapeModel>();
            model.TryGetTarget("robot.arm.left.shoulder.pivot", out IShapeTransformTarget shoulder);
            ForgeVector3 initialRotation = shoulder.LocalEulerAngles;

            LowPolyMotionExample motion = generatedRoot.AddComponent<LowPolyMotionExample>();
            motion.Configure(LowPolyMotionPreset.RobotShowcase);
            motion.Evaluate(0.25f);

            Assert.That(
                Mathf.Abs(shoulder.LocalEulerAngles.Z - initialRotation.Z),
                Is.GreaterThan(0.01f));
        }

        [Test]
        public void WorkbenchExampleOpensDrawerAndMovesLamp()
        {
            generatedRoot = Generate(
                LowPolyWorkbenchPreset.CreateDefinition(),
                LowPolyWorkbenchPreset.CreateStyle());
            UnityShapeModel model = generatedRoot.GetComponent<UnityShapeModel>();
            model.TryGetTarget("workbench.drawer", out IShapeTransformTarget drawer);
            model.TryGetTarget("workbench.lamp", out IShapeTransformTarget lamp);
            ForgeVector3 initialDrawerPosition = drawer.LocalPosition;
            ForgeVector3 initialLampRotation   = lamp.LocalEulerAngles;

            LowPolyMotionExample motion = generatedRoot.AddComponent<LowPolyMotionExample>();
            motion.Configure(LowPolyMotionPreset.WorkbenchShowcase);
            motion.Evaluate(0.25f);

            Assert.That(drawer.LocalPosition.Z, Is.LessThan(initialDrawerPosition.Z));
            Assert.That(
                Mathf.Abs(lamp.LocalEulerAngles.Y - initialLampRotation.Y),
                Is.GreaterThan(0.01f));
        }

        private static GameObject Generate(
            ShapeDefinition      definition,
            ShapeStyleDefinition style)
        {
            ShapeStyleResolver       resolver  = new(new[] { style });
            UnityShapeModelGenerator generator = new(
                new IUnityShapeGenerator[] { new LowPolyPrimitiveGenerator() },
                resolver);
            return generator.Generate(definition);
        }
    }
}
