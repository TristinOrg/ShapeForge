using System;

namespace ShapeForge
{
    /// <summary>
    /// Describes normalized observations for one named rendered view.
    /// </summary>
    [Serializable]
    public sealed class ShapeViewComparison
    {
        /// <summary>Gets or sets a stable view identifier such as front or side.</summary>
        public string ViewId { get; set; } = string.Empty;

        /// <summary>Gets or sets the contribution of this view to aggregate scores.</summary>
        public float Weight { get; set; } = 1f;

        /// <summary>Gets or sets observation confidence from zero to one.</summary>
        public float Confidence { get; set; } = 1f;

        /// <summary>Gets or sets normalized similarity scores.</summary>
        public ShapeComparisonScores Scores { get; set; } = new();
    }
}
