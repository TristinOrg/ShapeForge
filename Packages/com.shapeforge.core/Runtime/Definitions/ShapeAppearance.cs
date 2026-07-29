using System;
using UnityEngine;

namespace ShapeForge
{
    /// <summary>
    /// Describes style-independent appearance requests for a shape node.
    /// </summary>
    [Serializable]
    public sealed class ShapeAppearance
    {
        [SerializeField] private string colorRole       = string.Empty;
        [SerializeField] private bool   hasColorOverride;
        [SerializeField] private Color  color            = Color.white;

        /// <summary>
        /// Gets or sets the semantic palette role used by a style.
        /// </summary>
        public string ColorRole
        {
            get => colorRole;
            set => colorRole = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets whether this node overrides its resolved palette color.
        /// </summary>
        public bool HasColorOverride
        {
            get => hasColorOverride;
            set => hasColorOverride = value;
        }

        /// <summary>
        /// Gets or sets the direct color override.
        /// </summary>
        public Color Color
        {
            get => color;
            set => color = value;
        }
    }
}

