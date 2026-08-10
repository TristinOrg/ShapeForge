using System;

namespace ShapeForge
{
    /// <summary>
    /// Stores normalized visual similarity components from zero to one.
    /// </summary>
    [Serializable]
    public sealed class ShapeComparisonScores
    {
        /// <summary>Gets or sets silhouette similarity.</summary>
        public float Silhouette { get; set; }

        /// <summary>Gets or sets proportion similarity.</summary>
        public float Proportion { get; set; }

        /// <summary>Gets or sets color-block similarity.</summary>
        public float Color { get; set; }

        /// <summary>Gets or sets semantic-detail similarity.</summary>
        public float Detail { get; set; }
    }
}
