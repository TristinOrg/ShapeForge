using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Defines provider- and engine-independent observations extracted from aligned reference views.
    /// </summary>
    [Serializable]
    public sealed class ShapeReferenceDefinition
    {
        /// <summary>Identifies the current reference observation schema.</summary>
        public const string CurrentSchema = "shapeforge.reference/1.0";

        /// <summary>Gets or sets the schema identifier.</summary>
        public string Schema { get; set; } = CurrentSchema;

        /// <summary>Gets or sets the referenced object name.</summary>
        public string Name { get; set; } = "Reference";

        /// <summary>Gets or sets semantic parts observed across reference views.</summary>
        public IList<ShapeReferencePart> Parts { get; set; } = new List<ShapeReferencePart>();
    }
}
