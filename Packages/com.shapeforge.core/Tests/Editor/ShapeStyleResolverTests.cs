using NUnit.Framework;
using UnityEngine;

namespace ShapeForge.Tests
{
    /// <summary>
    /// Verifies style lookup and direct appearance override behavior.
    /// </summary>
    public sealed class ShapeStyleResolverTests
    {
        [Test]
        public void ResolverUsesSelectedStylePalette()
        {
            ShapeStyleDefinition style = new ShapeStyleDefinition("lowpoly/default");
            style.Palette.Set("primary", Color.blue);

            ShapeDefinition definition = new ShapeDefinition(
                "Model",
                new ShapeNode("root", "Root", ShapeTypes.Group))
            {
                Style = style.Id
            };
            ShapeNode node = new ShapeNode("body", "Body", "example/cube");
            node.Appearance.ColorRole = "primary";

            string json = JsonUtility.ToJson(style);
            style       = JsonUtility.FromJson<ShapeStyleDefinition>(json);

            ShapeStyleResolver resolver = new ShapeStyleResolver(new[] { style });

            Assert.That(resolver.TryResolveColor(definition, node, out Color color), Is.True);
            Assert.That(color, Is.EqualTo(Color.blue));
        }

        [Test]
        public void ContextPrefersDirectColorOverride()
        {
            ShapeDefinition definition = new ShapeDefinition(
                "Model",
                new ShapeNode("root", "Root", ShapeTypes.Group));
            ShapeNode node = new ShapeNode("body", "Body", "example/cube");
            node.Appearance.HasColorOverride = true;
            node.Appearance.Color            = Color.red;

            ShapeGenerationContext context = new ShapeGenerationContext(
                definition,
                new ConstantColorResolver(Color.blue));

            Assert.That(context.TryResolveColor(node, out Color color), Is.True);
            Assert.That(color, Is.EqualTo(Color.red));
        }

        [Test]
        public void ResolverReturnsFalseForUnknownStyle()
        {
            ShapeDefinition definition = new ShapeDefinition(
                "Model",
                new ShapeNode("root", "Root", ShapeTypes.Group))
            {
                Style = "missing/style"
            };
            ShapeNode node = new ShapeNode("body", "Body", "example/cube");
            node.Appearance.ColorRole = "primary";

            ShapeStyleResolver resolver = new ShapeStyleResolver(new ShapeStyleDefinition[0]);

            Assert.That(resolver.TryResolveColor(definition, node, out _), Is.False);
        }

        private sealed class ConstantColorResolver : IShapeStyleResolver
        {
            private readonly Color color;

            public ConstantColorResolver(Color color)
            {
                this.color = color;
            }

            public bool TryResolveColor(ShapeDefinition definition, ShapeNode node, out Color color)
            {
                color = this.color;
                return true;
            }
        }
    }
}
