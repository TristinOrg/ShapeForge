using System;
using UnityEngine;

namespace ShapeForge
{
    /// <summary>
    /// Defines a serializable procedural model and its requested style.
    /// </summary>
    [Serializable]
    public sealed class ShapeDefinition
    {
        [SerializeField] private string    name  = "Shape";
        [SerializeField] private string    style = string.Empty;
        [SerializeField] private ShapeNode root  = new ShapeNode();

        /// <summary>
        /// Initializes an empty definition for Unity serialization.
        /// </summary>
        public ShapeDefinition()
        {
        }

        /// <summary>
        /// Initializes a model definition with a root shape.
        /// </summary>
        public ShapeDefinition(string name, ShapeNode root)
        {
            Name = name;
            Root = root;
        }

        /// <summary>
        /// Gets or sets the model name.
        /// </summary>
        public string Name
        {
            get => name;
            set => name = string.IsNullOrWhiteSpace(value) ? "Shape" : value;
        }

        /// <summary>
        /// Gets or sets the style identifier resolved by a generation backend.
        /// </summary>
        public string Style
        {
            get => style;
            set => style = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the root shape node.
        /// </summary>
        public ShapeNode Root
        {
            get => root;
            set => root = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
