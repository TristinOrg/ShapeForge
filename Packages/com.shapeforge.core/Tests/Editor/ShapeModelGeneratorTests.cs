using System;
using NUnit.Framework;
using UnityEngine;

namespace ShapeForge.Tests
{
    /// <summary>
    /// Verifies Core hierarchy generation and validation behavior.
    /// </summary>
    public sealed class ShapeModelGeneratorTests
    {
        private GameObject generatedRoot;

        [TearDown]
        public void TearDown()
        {
            if (generatedRoot != null)
                UnityEngine.Object.DestroyImmediate(generatedRoot);
        }

        [Test]
        public void GenerateBuildsEditableHierarchy()
        {
            ShapeNode root  = new ShapeNode("robot", "Robot", ShapeTypes.Group);
            ShapeNode child = new ShapeNode("body", "Body", TestGenerator.Type);
            child.Transform.Scale = new Vector3(2f, 3f, 4f);
            root.Add(child);

            ShapeModelGenerator generator = new ShapeModelGenerator(new IShapeGenerator[]
            {
                new TestGenerator()
            });

            generatedRoot = generator.Generate(new ShapeDefinition("Robot", root));

            Assert.That(generatedRoot.name, Is.EqualTo("Robot"));
            Assert.That(generatedRoot.transform.childCount, Is.EqualTo(1));
            Assert.That(generatedRoot.transform.GetChild(0).name, Is.EqualTo("Body"));
            Assert.That(generatedRoot.transform.GetChild(0).localScale, Is.EqualTo(new Vector3(2f, 3f, 4f)));
        }

        [Test]
        public void GenerateRejectsDuplicateNodeIds()
        {
            ShapeNode root = new ShapeNode("duplicate", "Root", ShapeTypes.Group)
                .Add(new ShapeNode("duplicate", "Child", TestGenerator.Type));

            ShapeModelGenerator generator = new ShapeModelGenerator(new IShapeGenerator[]
            {
                new TestGenerator()
            });

            ShapeGenerationException exception = Assert.Throws<ShapeGenerationException>(() =>
                generator.Generate(new ShapeDefinition("Invalid", root)));

            Assert.That(exception.Message, Does.Contain("Duplicate"));
            Assert.That(GameObject.Find("Root"), Is.Null);
        }

        private sealed class TestGenerator : IShapeGenerator
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
