using NUnit.Framework;

namespace ShapeForge.Unity.Tests
{
    /// <summary>
    /// Verifies the reference JSON contract for cross-engine ShapeForge data.
    /// </summary>
    public sealed class ShapeJsonSerializerTests
    {
        [Test]
        public void ShapeRoundTripPreservesReadableVersionedData()
        {
            ShapeNode root  = new ShapeNode("robot", "Robot", ShapeTypes.Group);
            ShapeNode child = new ShapeNode("body", "Body", "example/cube");
            child.Transform.Position          = new ForgeVector3(1f, 2f, 3f);
            child.Appearance.ColorRole        = "primary";
            child.Appearance.HasColorOverride = true;
            child.Appearance.Color            = new ForgeColor(1f, 0f, 0f);
            child.Parameters["topWidth"]      = 0.6f;
            root.Add(child);

            ShapeDefinition source = new ShapeDefinition("Robot", root)
            {
                Style = "example/style"
            };
            ShapeJsonSerializer serializer = new ShapeJsonSerializer();

            string          json   = serializer.Serialize(source);
            ShapeDefinition result = serializer.DeserializeShape(json);

            Assert.That(json, Does.Contain("\"schema\":\"shapeforge.shape/1.0\""));
            Assert.That(json, Does.Contain("\"position\":{\"x\":1.0"));
            Assert.That(json, Does.Contain("\"parameters\":{\"topWidth\":0.6"));
            Assert.That(result.Root.Children, Has.Count.EqualTo(1));
            Assert.That(result.Root.Children[0].Transform.Position, Is.EqualTo(new ForgeVector3(1f, 2f, 3f)));
            Assert.That(result.Root.Children[0].Appearance.Color, Is.EqualTo(new ForgeColor(1f, 0f, 0f)));
            Assert.That(result.Root.Children[0].Parameters["topWidth"], Is.EqualTo(0.6f));
        }

        [Test]
        public void StyleRoundTripPreservesPalette()
        {
            ShapeStyleDefinition source = new ShapeStyleDefinition("lowpoly/default");
            source.Palette.Set("primary", new ForgeColor(0f, 0f, 1f));

            ShapeJsonSerializer serializer = new ShapeJsonSerializer();
            string               json       = serializer.Serialize(source);
            ShapeStyleDefinition result     = serializer.DeserializeStyle(json);

            Assert.That(json, Does.Contain("\"schema\":\"shapeforge.style/1.0\""));
            Assert.That(result.Palette.TryGetColor("primary", out ForgeColor color), Is.True);
            Assert.That(color, Is.EqualTo(new ForgeColor(0f, 0f, 1f)));
        }
    }
}
