using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Describes a part's normalized bounds and optional ordered silhouette in one orthographic view.
    /// </summary>
    [Serializable]
    public sealed class ShapeReferenceViewObservation
    {
        /// <summary>Gets or sets the image-normalized lower bounds.</summary>
        public ForgeVector2 Minimum { get; set; } = ForgeVector2.Zero;

        /// <summary>Gets or sets the image-normalized upper bounds.</summary>
        public ForgeVector2 Maximum { get; set; } = ForgeVector2.One;

        /// <summary>Gets or sets extraction confidence from zero to one.</summary>
        public float Confidence { get; set; } = 1f;

        /// <summary>Gets or sets an optional clockwise silhouette in image-normalized coordinates.</summary>
        public IList<ForgeVector2> Silhouette { get; set; } = new List<ForgeVector2>();
    }
}
