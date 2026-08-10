using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Defines a versioned multi-view reference-image manifest for external visual comparison.
    /// </summary>
    [Serializable]
    public sealed class ShapeReferenceImageSet
    {
        /// <summary>Identifies the current reference-image schema.</summary>
        public const string CurrentSchema = "shapeforge.reference-images/1.0";
        /// <summary>Gets or sets the schema identifier.</summary>
        public string Schema { get; set; } = CurrentSchema;
        /// <summary>Gets or sets the stable reference identifier.</summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>Gets or sets ordered named image views.</summary>
        public IList<ShapeReferenceImage> Images { get; set; } = new List<ShapeReferenceImage>();
    }
}
