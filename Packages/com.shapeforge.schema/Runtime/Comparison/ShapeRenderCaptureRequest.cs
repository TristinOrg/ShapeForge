using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Requests deterministic transparent candidate renders without naming a render engine.
    /// </summary>
    [Serializable]
    public sealed class ShapeRenderCaptureRequest
    {
        /// <summary>Identifies the current render-capture schema.</summary>
        public const string CurrentSchema = "shapeforge.render-capture/1.0";
        /// <summary>Gets or sets the schema identifier.</summary>
        public string Schema { get; set; } = CurrentSchema;
        /// <summary>Gets or sets the stable capture identifier.</summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>Gets or sets the candidate revision identifier.</summary>
        public string CandidateId { get; set; } = string.Empty;
        /// <summary>Gets or sets output width in pixels.</summary>
        public int Width { get; set; } = 512;
        /// <summary>Gets or sets output height in pixels.</summary>
        public int Height { get; set; } = 512;
        /// <summary>Gets or sets ordered requested camera views.</summary>
        public IList<ShapeRenderCaptureView> Views { get; set; } = new List<ShapeRenderCaptureView>();
    }
}
