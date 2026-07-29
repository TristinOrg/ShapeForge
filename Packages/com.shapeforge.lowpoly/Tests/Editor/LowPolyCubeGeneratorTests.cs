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
            ShapeNode cube = new ShapeNode("cube", "Cube", LowPolyShapeTypes.Cube);
            cube.Transform.Scale                = new ForgeVector3(2f, 3f, 4f);
            cube.Appearance.HasColorOverride    = true;
            cube.Appearance.Color               = new ForgeColor(1f, 0f, 0f);

            UnityShapeModelGenerator generator = new UnityShapeModelGenerator(new IUnityShapeGenerator[]
            {
                new LowPolyCubeGenerator()
            });

            generatedRoot = generator.Generate(new ShapeDefinition("Cube", cube));

            Assert.That(generatedRoot.GetComponent<MeshFilter>(), Is.Not.Null);
            Assert.That(generatedRoot.GetComponent<MeshRenderer>(), Is.Not.Null);
            Assert.That(generatedRoot.GetComponent<Collider>(), Is.Null);
            Assert.That(generatedRoot.transform.localScale, Is.EqualTo(new Vector3(2f, 3f, 4f)));

            UnityShapeColor shapeColor = generatedRoot.GetComponent<UnityShapeColor>();
            Assert.That(shapeColor, Is.Not.Null);
            Assert.That(shapeColor.Color, Is.EqualTo(Color.red));

            MaterialPropertyBlock properties = new MaterialPropertyBlock();
            Renderer renderer = generatedRoot.GetComponent<Renderer>();
            renderer.GetPropertyBlock(properties);
            Assert.That(properties.GetColor(Shader.PropertyToID("_Color")), Is.EqualTo(Color.red));

            renderer.SetPropertyBlock(null);
            shapeColor.Apply();
            renderer.GetPropertyBlock(properties);
            Assert.That(properties.GetColor(Shader.PropertyToID("_Color")), Is.EqualTo(Color.red));
        }
    }
}
