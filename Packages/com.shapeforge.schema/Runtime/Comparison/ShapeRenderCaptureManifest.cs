using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Lists deterministic candidate renders produced by an engine adapter.
    /// </summary>
    [Serializable]
    public sealed class ShapeRenderCaptureManifest
    {
        /// <summary>Gets or sets the source capture identifier.</summary>
        public string CaptureId { get; set; } = string.Empty;
        /// <summary>Gets or sets the candidate revision identifier.</summary>
        public string CandidateId { get; set; } = string.Empty;
        /// <summary>Gets or sets ordered rendered images.</summary>
        public IList<ShapeRenderCaptureImage> Images { get; set; } = new List<ShapeRenderCaptureImage>();
    }
}
