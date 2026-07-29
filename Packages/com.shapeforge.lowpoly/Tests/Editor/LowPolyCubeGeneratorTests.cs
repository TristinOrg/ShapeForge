using NUnit.Framework;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies the first official Low Poly shape implementation.
    /// </summary>
    public sealed class LowPolyCubeGeneratorTests
    {
        private GameObject generatedRoot;

        [TearDown]
        public void TearDown()
        {
            if (generatedRoot != null)
                UnityEngine.Object.DestroyImmediate(generatedRoot);
        }

        [Test]
        public void GenerateCreatesRenderableCubeWithoutPhysicsDependency()
        {
            ShapeNode cube = new("cube", "Cube", LowPolyShapeTypes.Cube);
            cube.Transform.Scale                = new(2f, 3f, 4f);
            cube.Appearance.HasColorOverride    = true;
            cube.Appearance.Color               = new(1f, 0f, 0f);

            UnityShapeModelGenerator generator = new(new IUnityShapeGenerator[]
            {
                new LowPolyCubeGenerator()
            });

            generatedRoot = generator.Generate(new("Cube", cube));

            Assert.That(generatedRoot.GetComponent<MeshFilter>(), Is.Not.Null);
            Assert.That(generatedRoot.GetComponent<MeshRenderer>(), Is.Not.Null);
            Assert.That(generatedRoot.GetComponent<Collider>(), Is.Null);
            Assert.That(generatedRoot.transform.localScale, Is.EqualTo(new Vector3(2f, 3f, 4f)));

            UnityShapeAppearanceManifest manifest = generatedRoot.GetComponent<UnityShapeAppearanceManifest>();
            Assert.That(manifest, Is.Not.Null);
            Assert.That(manifest.BindingCount, Is.EqualTo(1));

            MaterialPropertyBlock properties = new();
            Renderer renderer = generatedRoot.GetComponent<Renderer>();
            renderer.GetPropertyBlock(properties);
            Assert.That(properties.GetColor(Shader.PropertyToID("_Color")), Is.EqualTo(Color.red));

            renderer.SetPropertyBlock(null);
            manifest.Apply();
            renderer.GetPropertyBlock(properties);
            Assert.That(properties.GetColor(Shader.PropertyToID("_Color")), Is.EqualTo(Color.red));
        }

        [Test]
        public void GenerateSupportsCachedBuiltInPrimitiveMeshes()
        {
            ShapeNode root = new("primitives", "Primitives", ShapeTypes.Group);
            root
                .Add(new("sphere-a", "Sphere A", LowPolyShapeTypes.Sphere))
                .Add(new("sphere-b", "Sphere B", LowPolyShapeTypes.Sphere))
                .Add(new("cylinder", "Cylinder", LowPolyShapeTypes.Cylinder))
                .Add(new("capsule", "Capsule", LowPolyShapeTypes.Capsule));

            UnityShapeModelGenerator generator = new(new IUnityShapeGenerator[]
            {
                new LowPolyPrimitiveGenerator()
            });

            generatedRoot = generator.Generate(new("Primitives", root));

            MeshFilter firstSphere  = generatedRoot.transform.Find("Sphere A").GetComponent<MeshFilter>();
            MeshFilter secondSphere = generatedRoot.transform.Find("Sphere B").GetComponent<MeshFilter>();

            Assert.That(generatedRoot.GetComponentsInChildren<MeshRenderer>().Length, Is.EqualTo(4));
            Assert.That(generatedRoot.GetComponentsInChildren<Collider>().Length, Is.Zero);
            Assert.That(firstSphere.sharedMesh, Is.SameAs(secondSphere.sharedMesh));
        }
    }
}
