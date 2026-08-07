using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Describes an LLM-readable assessment made before reference-driven asset construction.
    /// </summary>
    [Serializable]
    public sealed class ShapeReferenceAssessment
    {
        /// <summary>Identifies the current assessment schema.</summary>
        public const string CurrentSchema = "shapeforge.reference-assessment/1.0";

        /// <summary>Gets or sets the schema identifier.</summary>
        public string Schema { get; set; } = CurrentSchema;

        /// <summary>Gets or sets the assessed subject category.</summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>Gets or sets the observed visual style.</summary>
        public string Style { get; set; } = string.Empty;

        /// <summary>Gets or sets the dominant camera azimuth in degrees.</summary>
        public float CameraAzimuth { get; set; }

        /// <summary>Gets or sets the dominant camera elevation in degrees.</summary>
        public float CameraElevation { get; set; }

        /// <summary>Gets or sets an extensible detail-level label.</summary>
        public string DetailLevel { get; set; } = string.Empty;

        /// <summary>Gets or sets assessment confidence from zero to one.</summary>
        public float Confidence { get; set; }

        /// <summary>Gets or sets directly visible semantic features.</summary>
        public IList<string> VisibleFeatures { get; set; } = new List<string>();

        /// <summary>Gets or sets ambiguities that downstream generation must not silently invent.</summary>
        public IList<string> Uncertainties { get; set; } = new List<string>();
    }
}
