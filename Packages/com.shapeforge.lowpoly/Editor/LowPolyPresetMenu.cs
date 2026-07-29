using ShapeForge.Unity;
using UnityEditor;
using UnityEngine;

namespace ShapeForge.LowPoly.Editor
{
    /// <summary>
    /// Provides undoable Unity Editor commands for previewing official Low Poly presets.
    /// </summary>
    internal static class LowPolyPresetMenu
    {
        [MenuItem("ShapeForge/Generate/Low Poly Table", false, 10)]
        private static void GenerateTable()
        {
            Generate(
                LowPolyTablePreset.CreateDefinition(),
                LowPolyTablePreset.CreateStyle());
        }

        [MenuItem("ShapeForge/Generate/Low Poly Robot", false, 11)]
        private static void GenerateRobot()
        {
            Generate(
                LowPolyRobotPreset.CreateDefinition(),
                LowPolyRobotPreset.CreateStyle());
        }

        private static void Generate(
            ShapeDefinition      definition,
            ShapeStyleDefinition style)
        {
            ShapeStyleResolver       resolver  = new ShapeStyleResolver(new[] { style });
            UnityShapeModelGenerator generator = new UnityShapeModelGenerator(
                new IUnityShapeGenerator[] { new LowPolyCubeGenerator() },
                resolver);
            GameObject generated = generator.Generate(definition);

            Undo.RegisterCreatedObjectUndo(generated, $"Generate {generated.name}");
            Selection.activeGameObject = generated;
            EditorGUIUtility.PingObject(generated);
        }
    }
}
