using System;
using UnityEngine;

namespace ShapeForge
{
    /// <summary>
    /// Maps a semantic color role to a concrete color.
    /// </summary>
    [Serializable]
    public sealed class ShapePaletteEntry
    {
        [SerializeField] private string role  = string.Empty;
        [SerializeField] private Color  color = Color.white;

        /// <summary>
        /// Initializes an empty palette entry for Unity serialization.
        /// </summary>
        public ShapePaletteEntry()
        {
        }

        /// <summary>
        /// Initializes a palette entry with a semantic role and color.
        /// </summary>
        public ShapePaletteEntry(string role, Color color)
        {
            Role  = role;
            Color = color;
        }

        /// <summary>
        /// Gets or sets the semantic color role.
        /// </summary>
        public string Role
        {
            get => role;
            set => role = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the resolved color.
        /// </summary>
        public Color Color
        {
            get => color;
            set => color = value;
        }
    }
}
