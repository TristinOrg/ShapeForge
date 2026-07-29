using NUnit.Framework;
using UnityEngine;

namespace ShapeForge.Unity.Tests
{
    /// <summary>
    /// Verifies Unity hierarchy adaptation and validation behavior.
    /// </summary>
    public sealed class UnityShapeModelGeneratorTests
    {
        private GameObject generatedRoot;

        [TearDown]
        public void TearDown()
        {
            if (generatedRoot != null)
                Object.DestroyImmediate(generatedRoot);
        }

        [Test]
        public void GenerateBuildsEditableUnityHierarchy()
        {
            ShapeNode root  = new ShapeNode("robot", "Robot", ShapeTypes.Group);
            ShapeNode child = new ShapeNode("body", "Body", TestGenerator.Type);
            child.Transform.Scale = new ForgeVector3(2f, 3f, 4f);
            root.Add(child);

            UnityShapeModelGenerator generator = new UnityShapeModelGenerator(new IUnityShapeGenerator[]
            {
                new TestGenerator()
            });

            generatedRoot = generator.Generate(new ShapeDefinition("Robot", root));

            Assert.That(generatedRoot.name, Is.EqualTo("Robot"));
            Assert.That(generatedRoot.transform.childCount, Is.EqualTo(1));
            Assert.That(generatedRoot.transform.GetChild(0).name, Is.EqualTo("Body"));
            Assert.That(generatedRoot.transform.GetChild(0).localScale, Is.EqualTo(new Vector3(2f, 3f, 4f)));

            UnityShapeModel model = generatedRoot.GetComponent<UnityShapeModel>();
            Assert.That(model.BindingCount, Is.EqualTo(2));
            Assert.That(model.TryGetTarget("body", out IShapeTransformTarget target), Is.True);

            target.LocalPosition = new(5f, 6f, 7f);
            Assert.That(generatedRoot.transform.GetChild(0).localPosition, Is.EqualTo(new Vector3(5f, 6f, 7f)));
            Assert.That(model.TryGetTarget("missing", out _), Is.False);
        }

        [Test]
        public void GenerateRejectsDuplicateNodeIdsBeforeCreatingObjects()
        {
            ShapeNode root = new ShapeNode("duplicate", "Root", ShapeTypes.Group)
                .Add(new ShapeNode("duplicate", "Child", TestGenerator.Type));
            UnityShapeModelGenerator generator = new UnityShapeModelGenerator(new IUnityShapeGenerator[]
            {
                new TestGenerator()
            });

            ShapeValidationException exception = Assert.Throws<ShapeValidationException>(() =>
                generator.Generate(new ShapeDefinition("Invalid", root)));

            Assert.That(exception.Message, Does.Contain("Duplicate"));
            Assert.That(GameObject.Find("Root"), Is.Null);
        }

        private sealed class TestGenerator : IUnityShapeGenerator
        {
            public const string Type = "test/shape";

            public bool CanGenerate(ShapeNode node)
            {
                return node != null && node.Type == Type;
            }

            public GameObject Generate(ShapeNode node, ShapeGenerationContext context)
            {
                return new GameObject();
            }
        }
    }
}
