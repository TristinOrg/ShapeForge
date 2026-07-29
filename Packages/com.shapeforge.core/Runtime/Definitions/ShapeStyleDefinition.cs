using System;
using UnityEngine;

namespace ShapeForge
{
    /// <summary>
    /// Defines serializable style data independently from model geometry.
    /// </summary>
    [Serializable]
    public sealed class ShapeStyleDefinition
    {
        [SerializeField] private string       id      = string.Empty;
        [SerializeField] private ShapePalette palette = new ShapePalette();

        /// <summary>
        /// Initializes an empty style for Unity serialization.
        /// </summary>
        public ShapeStyleDefinition()
        {
        }

        /// <summary>
        /// Initializes a style with a stable identifier.
        /// </summary>
        public ShapeStyleDefinition(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets or sets the stable style identifier.
        /// </summary>
        public string Id
        {
            get => id;
            set => id = value ?? string.Empty;
        }

        /// <summary>
        /// Gets the semantic color palette.
        /// </summary>
        public ShapePalette Palette => palette;
    }
}
