using NUnit.Framework;

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
            style.Palette.Set("primary", new ForgeColor(0f, 0f, 1f));

            ShapeDefinition definition = new ShapeDefinition(
                "Model",
                new ShapeNode("root", "Root", ShapeTypes.Group))
            {
                Style = style.Id
            };
            ShapeNode node = new ShapeNode("body", "Body", "example/cube");
            node.Appearance.ColorRole = "primary";

            ShapeStyleResolver resolver = new ShapeStyleResolver(new[] { style });

            Assert.That(resolver.TryResolveColor(definition, node, out ForgeColor color), Is.True);
            Assert.That(color, Is.EqualTo(new ForgeColor(0f, 0f, 1f)));
        }

        [Test]
        public void ContextPrefersDirectColorOverride()
        {
            ShapeDefinition definition = new ShapeDefinition(
                "Model",
                new ShapeNode("root", "Root", ShapeTypes.Group));
            ShapeNode node = new ShapeNode("body", "Body", "example/cube");
            node.Appearance.HasColorOverride = true;
            node.Appearance.Color            = new ForgeColor(1f, 0f, 0f);

            ShapeGenerationContext context = new ShapeGenerationContext(
                definition,
                new ConstantColorResolver(new ForgeColor(0f, 0f, 1f)));

            Assert.That(context.TryResolveColor(node, out ForgeColor color), Is.True);
            Assert.That(color, Is.EqualTo(new ForgeColor(1f, 0f, 0f)));
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
            private readonly ForgeColor color;

            public ConstantColorResolver(ForgeColor color)
            {
                this.color = color;
            }

            public bool TryResolveColor(ShapeDefinition definition, ShapeNode node, out ForgeColor color)
            {
                color = this.color;
                return true;
            }
        }
    }
}
