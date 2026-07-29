using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeForge
{
    /// <summary>
    /// Defines one serializable shape in an editable model hierarchy.
    /// </summary>
    [Serializable]
    public sealed class ShapeNode
    {
        [SerializeField] private string           id         = string.Empty;
        [SerializeField] private string           name       = "Shape";
        [SerializeField] private string           type       = ShapeTypes.Group;
        [SerializeField] private ShapeTransform   transform  = new ShapeTransform();
        [SerializeField] private ShapeAppearance  appearance = new ShapeAppearance();
        [SerializeField] private List<ShapeNode>  children   = new List<ShapeNode>();

        /// <summary>
        /// Initializes an empty group node for Unity serialization.
        /// </summary>
        public ShapeNode()
        {
        }

        /// <summary>
        /// Initializes a shape node with stable identity and type information.
        /// </summary>
        public ShapeNode(string id, string name, string type)
        {
            Id   = id;
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Gets or sets the stable node identifier used by animation and regeneration.
        /// </summary>
        public string Id
        {
            get => id;
            set => id = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the generated GameObject name.
        /// </summary>
        public string Name
        {
            get => name;
            set => name = string.IsNullOrWhiteSpace(value) ? "Shape" : value;
        }

        /// <summary>
        /// Gets or sets the extensible shape type identifier.
        /// </summary>
        public string Type
        {
            get => type;
            set => type = value ?? string.Empty;
        }

        /// <summary>
        /// Gets the local transform definition.
        /// </summary>
        public ShapeTransform Transform => transform;

        /// <summary>
        /// Gets the appearance request for this node.
        /// </summary>
        public ShapeAppearance Appearance => appearance;

        /// <summary>
        /// Gets the child shape definitions.
        /// </summary>
        public IReadOnlyList<ShapeNode> Children => children;

        /// <summary>
        /// Adds a child shape and returns this node for fluent composition.
        /// </summary>
        public ShapeNode Add(ShapeNode child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            children.Add(child);
            return this;
        }
    }
}

