using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeForge
{
    /// <summary>
    /// Defines serializable semantic colors that a style can resolve.
    /// </summary>
    [Serializable]
    public sealed class ShapePalette
    {
        [SerializeField] private List<ShapePaletteEntry> entries = new List<ShapePaletteEntry>();

        /// <summary>
        /// Gets the palette entries in their serialized order.
        /// </summary>
        public IReadOnlyList<ShapePaletteEntry> Entries => entries;

        /// <summary>
        /// Adds or replaces a semantic color and returns this palette.
        /// </summary>
        public ShapePalette Set(string role, Color color)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("A palette role cannot be empty.", nameof(role));

            foreach (ShapePaletteEntry entry in entries)
            {
                if (!string.Equals(entry.Role, role, StringComparison.Ordinal))
                    continue;

                entry.Color = color;
                return this;
            }

            entries.Add(new ShapePaletteEntry(role, color));
            return this;
        }

        /// <summary>
        /// Attempts to resolve a semantic color role.
        /// </summary>
        public bool TryGetColor(string role, out Color color)
        {
            foreach (ShapePaletteEntry entry in entries)
            {
                if (!string.Equals(entry.Role, role, StringComparison.Ordinal))
                    continue;

                color = entry.Color;
                return true;
            }

            color = default;
            return false;
        }
    }
}
