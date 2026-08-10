using System;

namespace ShapeForge
{
    /// <summary>
    /// Identifies one named reference view and its external image file.
    /// </summary>
    [Serializable]
    public sealed class ShapeReferenceImage
    {
        /// <summary>Gets or sets the stable view identifier.</summary>
        public string ViewId { get; set; } = string.Empty;
        /// <summary>Gets or sets the image path relative to its manifest or as an absolute path.</summary>
        public string ImagePath { get; set; } = string.Empty;
        /// <summary>Gets or sets the positive comparison weight.</summary>
        public float Weight { get; set; } = 1f;
    }
}
