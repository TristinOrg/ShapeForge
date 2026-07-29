using NUnit.Framework;
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
            cube.Transform.Scale                = new Vector3(2f, 3f, 4f);
            cube.Appearance.HasColorOverride    = true;
            cube.Appearance.Color               = Color.red;

            ShapeModelGenerator generator = new ShapeModelGenerator(new IShapeGenerator[]
            {
                new LowPolyCubeGenerator()
            });

            generatedRoot = generator.Generate(new ShapeDefinition("Cube", cube));

            Assert.That(generatedRoot.GetComponent<MeshFilter>(), Is.Not.Null);
            Assert.That(generatedRoot.GetComponent<MeshRenderer>(), Is.Not.Null);
            Assert.That(generatedRoot.GetComponent<Collider>(), Is.Null);
            Assert.That(generatedRoot.transform.localScale, Is.EqualTo(new Vector3(2f, 3f, 4f)));

            MaterialPropertyBlock properties = new MaterialPropertyBlock();
            generatedRoot.GetComponent<Renderer>().GetPropertyBlock(properties);
            Assert.That(properties.GetColor(Shader.PropertyToID("_Color")), Is.EqualTo(Color.red));
        }
    }
}
