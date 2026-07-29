using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Defines one serializable shape in an editable model hierarchy.
    /// </summary>
    [Serializable]
    public sealed class ShapeNode
    {
        private readonly List<ShapeNode> children = new List<ShapeNode>();

        /// <summary>
        /// Initializes an empty group node for serialization.
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
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the generated object name.
        /// </summary>
        public string Name { get; set; } = "Shape";

        /// <summary>
        /// Gets or sets the extensible shape type identifier.
        /// </summary>
        public string Type { get; set; } = ShapeTypes.Group;

        /// <summary>
        /// Gets or sets the local transform definition.
        /// </summary>
        public ShapeTransform Transform { get; set; } = new ShapeTransform();

        /// <summary>
        /// Gets or sets the appearance request for this node.
        /// </summary>
        public ShapeAppearance Appearance { get; set; } = new ShapeAppearance();

        /// <summary>
        /// Gets the child shape definitions.
        /// </summary>
        public IList<ShapeNode> Children => children;

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
