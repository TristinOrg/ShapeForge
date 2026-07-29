using NUnit.Framework;
using UnityEngine;

namespace ShapeForge.Tests
{
    /// <summary>
    /// Verifies the serializable Core shape data contract.
    /// </summary>
    public sealed class ShapeDefinitionTests
    {
        [Test]
        public void JsonRoundTripPreservesHierarchyAndAppearance()
        {
            ShapeNode root  = new ShapeNode("robot", "Robot", ShapeTypes.Group);
            ShapeNode child = new ShapeNode("body", "Body", "example/cube");

            child.Transform.Position           = new Vector3(1f, 2f, 3f);
            child.Appearance.ColorRole         = "primary";
            child.Appearance.HasColorOverride  = true;
            child.Appearance.Color             = Color.red;
            root.Add(child);

            ShapeDefinition source = new ShapeDefinition("Robot", root)
            {
                Style = "example/style"
            };

            string          json   = JsonUtility.ToJson(source);
            ShapeDefinition result = JsonUtility.FromJson<ShapeDefinition>(json);

            Assert.That(result.Name, Is.EqualTo("Robot"));
            Assert.That(result.Style, Is.EqualTo("example/style"));
            Assert.That(result.Root.Children, Has.Count.EqualTo(1));
            Assert.That(result.Root.Children[0].Id, Is.EqualTo("body"));
            Assert.That(result.Root.Children[0].Transform.Position, Is.EqualTo(new Vector3(1f, 2f, 3f)));
            Assert.That(result.Root.Children[0].Appearance.ColorRole, Is.EqualTo("primary"));
            Assert.That(result.Root.Children[0].Appearance.Color, Is.EqualTo(Color.red));
        }
    }
}
