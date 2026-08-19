using ShapeForge.Unity;
using ShapeForge.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace ShapeForge.LowPoly.Editor
{
    /// <summary>
    /// Provides undoable Unity Editor commands for previewing official Low Poly presets.
    /// </summary>
    internal static class LowPolyPresetMenu
    {
        [MenuItem("ShapeForge/Generate/Inventor Workbench", false, 10)]
        private static void GenerateWorkbench()
        {
            Generate(
                LowPolyWorkbenchPreset.CreateDefinition(),
                LowPolyWorkbenchPreset.CreateStyle());
        }

        [MenuItem("ShapeForge/Generate/Low Poly Robot", false, 11)]
        private static void GenerateRobot()
        {
            Generate(
                LowPolyRobotPreset.CreateDefinition(),
                LowPolyRobotPreset.CreateStyle());
        }

        [MenuItem("ShapeForge/Generate/Japanese Town", false, 12)]
        private static void GenerateJapaneseTown()
        {
            Generate(
                LowPolyJapaneseTownPreset.CreateDefinition(),
                LowPolyJapaneseTownPreset.CreateStyle());
        }

        [MenuItem("ShapeForge/Generate/Shibuya Crossing", false, 14)]
        private static void GenerateShibuyaCrossing()
        {
            Generate(
                LowPolyShibuyaCrossingPreset.CreateDefinition(),
                LowPolyShibuyaCrossingPreset.CreateStyle());
        }

        [MenuItem("ShapeForge/Generate/Animated Inventor Workbench", false, 20)]
        private static void GenerateAnimatedWorkbench()
        {
            Generate(
                LowPolyWorkbenchPreset.CreateDefinition(),
                LowPolyWorkbenchPreset.CreateStyle(),
                LowPolyMotionPreset.WorkbenchShowcase);
        }

        [MenuItem("ShapeForge/Generate/Animated Low Poly Robot", false, 21)]
        private static void GenerateAnimatedRobot()
        {
            Generate(
                LowPolyRobotPreset.CreateDefinition(),
                LowPolyRobotPreset.CreateStyle(),
                LowPolyMotionPreset.RobotShowcase);
        }

        private static GameObject Generate(
            ShapeDefinition      definition,
            ShapeStyleDefinition style,
            LowPolyMotionPreset? motionPreset = null)
        {
            ShapeStyleResolver       resolver  = new(new[] { style });
            UnityShapeModelGenerator generator = new(
                new IUnityShapeGenerator[] { new LowPolyPrimitiveGenerator() },
                resolver);
            GameObject generated = generator.Generate(definition);
            UnityGeneratedModelAssetStore.PersistMeshes(generated);

            if (motionPreset.HasValue)
            {
                LowPolyMotionExample motion = generated.AddComponent<LowPolyMotionExample>();
                motion.Configure(motionPreset.Value);
            }

            Undo.RegisterCreatedObjectUndo(generated, $"Generate {generated.name}");
            Selection.activeGameObject = generated;
            EditorGUIUtility.PingObject(generated);
            return generated;
        }

    }
}
