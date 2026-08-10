using System;

namespace ShapeForge
{
    /// <summary>
    /// Records one rendered view and its portable image path.
    /// </summary>
    [Serializable]
    public sealed class ShapeRenderCaptureImage
    {
        /// <summary>Gets or sets the stable view identifier.</summary>
        public string ViewId { get; set; } = string.Empty;
        /// <summary>Gets or sets the normalized PNG filesystem path.</summary>
        public string ImagePath { get; set; } = string.Empty;
    }
}
