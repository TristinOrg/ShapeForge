using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeForge
{
    /// <summary>
    /// Resolves shape colors from explicitly supplied serializable style definitions.
    /// </summary>
    public sealed class ShapeStyleResolver : IShapeStyleResolver
    {
        private readonly Dictionary<string, ShapeStyleDefinition> styles =
            new Dictionary<string, ShapeStyleDefinition>(StringComparer.Ordinal);

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

                if (string.IsNullOrWhiteSpace(style.Id))
                    throw new ArgumentException("Every style requires a stable ID.", nameof(styles));

                if (!this.styles.TryAdd(style.Id, style))
                    throw new ArgumentException($"Duplicate style ID '{style.Id}'.", nameof(styles));
            }
        }

        /// <inheritdoc />
        public bool TryResolveColor(ShapeDefinition definition, ShapeNode node, out Color color)
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
