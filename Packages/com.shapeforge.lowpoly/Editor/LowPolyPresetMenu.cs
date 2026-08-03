using ShapeForge.Unity;
using ShapeForge.Unity.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ShapeForge.LowPoly.Editor
{
    /// <summary>
    /// Provides undoable Unity Editor commands for previewing official Low Poly presets.
    /// </summary>
    internal static class LowPolyPresetMenu
    {
        private const string GeneratedAssetFolder = "Assets/ShapeForge/Generated";

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

        [MenuItem("ShapeForge/Generate/Fantasy Hero", false, 13)]
        private static void GenerateHero()
        {
            Generate(
                LowPolyHeroPreset.CreateDefinition(),
                LowPolyHeroPreset.CreateStyle());
        }

        [MenuItem("ShapeForge/Generate/Shibuya Crossing", false, 14)]
        private static void GenerateShibuyaCrossing()
        {
            Generate(
                LowPolyShibuyaCrossingPreset.CreateDefinition(),
                LowPolyShibuyaCrossingPreset.CreateStyle());
        }

        [MenuItem("ShapeForge/Generate/Humanoid T-Pose Hero", false, 15)]
        private static void GenerateHumanoidHero()
        {
            Generate(
                LowPolyHumanoidHeroPreset.CreateDefinition(),
                LowPolyHumanoidHeroPreset.CreateStyle());
        }

        [MenuItem("ShapeForge/Generate/Animated Humanoid T-Pose Hero From Selected Clip", false, 16)]
        private static void GenerateAnimatedHumanoidHero()
        {
            AnimationClip clip = Selection.activeObject as AnimationClip;
            if (clip == null || !clip.humanMotion)
            {
                EditorUtility.DisplayDialog(
                    "Select a Humanoid AnimationClip",
                    "Select an imported Humanoid AnimationClip in the Project window before generating this preview.",
                    "OK");
                return;
            }

            ShapeDefinition definition = LowPolyHumanoidHeroPreset.CreateDefinition();
            GameObject generated = Generate(definition, LowPolyHumanoidHeroPreset.CreateStyle());
            Avatar     avatar    = UnityHumanoidAvatarBuilder.CreateAvatar(
                generated.GetComponent<UnityShapeModel>(),
                definition.Rig);

            string             avatarPath  = CreateAsset(avatar, "Humanoid Hero Avatar");
            AnimatorController controller = CreatePreviewController(clip);
            Animator           animator    = Undo.AddComponent<Animator>(generated);
            animator.avatar                    = AssetDatabase.LoadAssetAtPath<Avatar>(avatarPath);
            animator.runtimeAnimatorController = controller;
            Selection.activeGameObject         = generated;
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

        [MenuItem("ShapeForge/Generate/Animated Fantasy Hero", false, 22)]
        private static void GenerateAnimatedHero()
        {
            Generate(
                LowPolyHeroPreset.CreateDefinition(),
                LowPolyHeroPreset.CreateStyle(),
                LowPolyMotionPreset.HumanHeroWalk);
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

        private static string CreateAsset(Avatar avatar, string name)
        {
            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{GeneratedAssetFolder}/{name}.asset");
            AssetDatabase.CreateAsset(avatar, assetPath);
            AssetDatabase.SaveAssets();
            return assetPath;
        }

        private static AnimatorController CreatePreviewController(AnimationClip clip)
        {
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(
                $"{GeneratedAssetFolder}/{clip.name} Humanoid Preview.controller");
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(assetPath);
            AnimatorState state = controller.layers[0].stateMachine.AddState("Preview");
            state.motion = clip;
            controller.layers[0].stateMachine.defaultState = state;
            AssetDatabase.SaveAssets();
            return controller;
        }
    }
}
