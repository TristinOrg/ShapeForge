using NUnit.Framework;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies the first official Low Poly shape implementation.
    /// </summary>
    public sealed class LowPolyPrimitiveGeneratorTests
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
                new LowPolyPrimitiveGenerator()
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

        [Test]
        public void GenerateSupportsCachedParameterizedMeshes()
        {
            ShapeNode root          = new("procedural", "Procedural Shapes", ShapeTypes.Group);
            ShapeNode wedge         = new("wedge", "Wedge", LowPolyShapeTypes.Wedge);
            ShapeNode firstFrustum  = CreateFrustum("frustum-a", "Frustum A", 0.45f);
            ShapeNode secondFrustum = CreateFrustum("frustum-b", "Frustum B", 0.45f);
            ShapeNode wideFrustum   = CreateFrustum("frustum-c", "Frustum C", 0.8f);
            root.Add(wedge).Add(firstFrustum).Add(secondFrustum).Add(wideFrustum);

            UnityShapeModelGenerator generator = new(new IUnityShapeGenerator[]
            {
                new LowPolyPrimitiveGenerator()
            });

            generatedRoot = generator.Generate(new("Procedural Shapes", root));

            Mesh wedgeMesh     = generatedRoot.transform.Find("Wedge").GetComponent<MeshFilter>().sharedMesh;
            Mesh firstMesh     = generatedRoot.transform.Find("Frustum A").GetComponent<MeshFilter>().sharedMesh;
            Mesh secondMesh    = generatedRoot.transform.Find("Frustum B").GetComponent<MeshFilter>().sharedMesh;
            Mesh differentMesh = generatedRoot.transform.Find("Frustum C").GetComponent<MeshFilter>().sharedMesh;

            Assert.That(wedgeMesh.vertexCount, Is.EqualTo(18));
            Assert.That(wedgeMesh.hideFlags, Is.EqualTo(HideFlags.None));
            Assert.That(wedgeMesh.normals[0].y, Is.LessThan(-0.99f));
            Assert.That(firstMesh.vertexCount, Is.EqualTo(24));
            Assert.That(firstMesh.normals[0].y, Is.LessThan(-0.99f));
            Assert.That(firstMesh, Is.SameAs(secondMesh));
            Assert.That(firstMesh, Is.Not.SameAs(differentMesh));
        }

        [Test]
        public void GenerateTriangulatesAndCachesConcaveExtrudedProfiles()
        {
            ForgeVector2[] points =
            {
                new(-0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, -0.5f),
                new(0f, -0.15f), new(-0.5f, -0.5f)
            };
            ShapeNode first  = CreateProfile("profile-a", "Profile A", points);
            ShapeNode second = CreateProfile("profile-b", "Profile B", points);
            ShapeNode root   = new("profiles", "Profiles", ShapeTypes.Group);
            root.Add(first).Add(second);

            UnityShapeModelGenerator generator = new(new IUnityShapeGenerator[]
            {
                new LowPolyPrimitiveGenerator()
            });

            generatedRoot = generator.Generate(new("Profiles", root));

            Mesh firstMesh  = generatedRoot.transform.Find("Profile A").GetComponent<MeshFilter>().sharedMesh;
            Mesh secondMesh = generatedRoot.transform.Find("Profile B").GetComponent<MeshFilter>().sharedMesh;
            Assert.That(firstMesh.name, Is.EqualTo("Low Poly Extruded Profile"));
            Assert.That(firstMesh.vertexCount, Is.EqualTo(38));
            Assert.That(firstMesh.bounds.size.z, Is.EqualTo(0.2f).Within(0.0001f));
            Assert.That(firstMesh, Is.SameAs(secondMesh));
        }

        private static ShapeNode CreateFrustum(string id, string name, float topWidth)
        {
            ShapeNode node = new(id, name, LowPolyShapeTypes.Frustum);
            node.Parameters[LowPolyShapeParameters.TopWidth]    = topWidth;
            node.Parameters[LowPolyShapeParameters.TopDepth]    = 0.6f;
            node.Parameters[LowPolyShapeParameters.BottomWidth] = 1f;
            node.Parameters[LowPolyShapeParameters.BottomDepth] = 0.9f;
            return node;
        }

        private static ShapeNode CreateProfile(string id, string name, ForgeVector2[] points)
        {
            ShapeNode node = new(id, name, LowPolyShapeTypes.ExtrudedProfile);
            node.Parameters[LowPolyShapeParameters.ProfileDepth] = 0.2f;
            foreach (ForgeVector2 point in points)
                node.Profile.Add(point);

            return node;
        }
    }
}
