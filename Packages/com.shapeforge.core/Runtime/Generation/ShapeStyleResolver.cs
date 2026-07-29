using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Resolves shape colors from explicitly supplied serializable style definitions.
    /// </summary>
    public sealed class ShapeStyleResolver : IShapeStyleResolver
    {
        private readonly Dictionary<string, ShapeStyleDefinition> styles = new(StringComparer.Ordinal);
        private readonly ShapeStyleDefinitionValidator validator = new();

        /// <summary>
        /// Initializes a resolver with its available style definitions.
        /// </summary>
        public ShapeStyleResolver(IEnumerable<ShapeStyleDefinition> styles)
        {
            if (styles == null)
                throw new ArgumentNullException(nameof(styles));

            foreach (ShapeStyleDefinition style in styles)
            {
                if (style == null)
                    throw new ArgumentException("Styles cannot contain null entries.", nameof(styles));

                validator.Validate(style);

                if (!this.styles.TryAdd(style.Id, style))
                    throw new ArgumentException($"Duplicate style ID '{style.Id}'.", nameof(styles));
            }
        }

        /// <summary>
        /// Adds or replaces a validated style without rebuilding the resolver.
        /// </summary>
        public void Set(ShapeStyleDefinition style)
        {
            if (style == null)
                throw new ArgumentNullException(nameof(style));

            validator.Validate(style);
            styles[style.Id] = style;
        }

        /// <inheritdoc />
        public bool TryResolveColor(ShapeDefinition definition, ShapeNode node, out ForgeColor color)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (styles.TryGetValue(definition.Style, out ShapeStyleDefinition style))
                return style.Palette.TryGetColor(node.Appearance.ColorRole, out color);

            color = default;
            return false;
        }
    }
}
