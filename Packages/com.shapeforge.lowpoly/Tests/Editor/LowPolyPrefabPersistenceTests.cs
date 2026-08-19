using NUnit.Framework;
using ShapeForge.Unity;
using ShapeForge.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies that generated Low Poly models survive Unity Prefab serialization.
    /// </summary>
    public sealed class LowPolyPrefabPersistenceTests
    {
        private const string TestFolder = "Assets/ShapeForgePrefabTests";
        private GameObject generatedRoot;
        private GameObject prefabRoot;

        [TearDown]
        public void TearDown()
        {
            if (prefabRoot != null)
                PrefabUtility.UnloadPrefabContents(prefabRoot);

            if (generatedRoot != null)
                Object.DestroyImmediate(generatedRoot);

            AssetDatabase.DeleteAsset(TestFolder);
        }

        [Test]
        public void GeneratedModelRetainsGeometryAndAppearanceAfterPrefabReload()
        {
            ShapeStyleResolver       resolver  = new(new[] { LowPolyHeroPreset.CreateStyle() });
            UnityShapeModelGenerator generator = new(
                new IUnityShapeGenerator[] { new LowPolyPrimitiveGenerator() },
                resolver);
            ShapeDefinition definition = LowPolyHeroPreset.CreateDefinition();
            generatedRoot = generator.Generate(definition);

            AssetDatabase.CreateFolder("Assets", "ShapeForgePrefabTests");
            UnityShapePrefabCompilationResult compilation =
                UnityShapePrefabCompiler.Compile(generatedRoot, definition, TestFolder);
            Assert.That(compilation.MeshAssetPath, Is.Not.Null);
            Transform generatedHair = generatedRoot.transform.Find("Head Pivot/Reference Unified Hair Volume");
            Assert.That(AssetDatabase.Contains(generatedHair.GetComponent<MeshFilter>().sharedMesh), Is.True);
            Color expectedColor = generatedHair.GetComponent<Renderer>().sharedMaterial.color;

            Object.DestroyImmediate(generatedRoot);
            generatedRoot = null;

            prefabRoot = PrefabUtility.LoadPrefabContents(compilation.PrefabPath);
            Transform  hair         = prefabRoot.transform.Find("Head Pivot/Reference Unified Hair Volume");
            Renderer   hairRenderer = hair.GetComponent<Renderer>();
            MeshFilter hairFilter   = hair.GetComponent<MeshFilter>();
            Color      color        = hairRenderer.sharedMaterial.color;

            Assert.That(hairFilter.sharedMesh, Is.Not.Null);
            Assert.That(hairFilter.sharedMesh.name, Is.EqualTo("Low Poly Profile Cage"));
            Assert.That(hairRenderer.sharedMaterial, Is.Not.Null);
            Assert.That(color.r, Is.EqualTo(expectedColor.r).Within(0.0001f));
            Assert.That(color.g, Is.EqualTo(expectedColor.g).Within(0.0001f));
            Assert.That(color.b, Is.EqualTo(expectedColor.b).Within(0.0001f));
            Assert.That(color.a, Is.EqualTo(expectedColor.a).Within(0.0001f));
            Assert.That(prefabRoot.GetComponent<UnityShapeAppearanceManifest>().BindingCount, Is.EqualTo(74));
            UnityShapeAssetManifest assetManifest = prefabRoot.GetComponent<UnityShapeAssetManifest>();
            Assert.That(assetManifest.Schema, Is.EqualTo(ShapeDefinition.CurrentSchema));
            Assert.That(assetManifest.NodeCount, Is.EqualTo(prefabRoot.GetComponent<UnityShapeModel>().BindingCount));
            Assert.That(assetManifest.Fingerprint, Has.Length.EqualTo(64));
            Assert.That(assetManifest.DefinitionJson, Does.Contain("\"schema\":\"shapeforge.shape/1.0\""));
        }
    }
}
