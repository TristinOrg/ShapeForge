using System;

namespace ShapeForge
{
    /// <summary>
    /// Defines one deterministic camera view requested from an engine adapter.
    /// </summary>
    [Serializable]
    public sealed class ShapeRenderCaptureView
    {
        /// <summary>Gets or sets the stable view identifier.</summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>Gets or sets horizontal camera orbit in degrees.</summary>
        public float Azimuth { get; set; }
        /// <summary>Gets or sets vertical camera orbit in degrees.</summary>
        public float Elevation { get; set; }
        /// <summary>Gets or sets framing scale where one tightly fits the model.</summary>
        public float FramingScale { get; set; } = 1.1f;
    }
}
