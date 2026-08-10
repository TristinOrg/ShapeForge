using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Defines a versioned provider-neutral comparison between reference and candidate renders.
    /// </summary>
    [Serializable]
    public sealed class ShapeRenderComparison
    {
        /// <summary>Identifies the current render-comparison schema.</summary>
        public const string CurrentSchema = "shapeforge.render-compare/1.0";

        /// <summary>Gets or sets the schema identifier.</summary>
        public string Schema { get; set; } = CurrentSchema;

        /// <summary>Gets or sets the external reference identifier.</summary>
        public string ReferenceId { get; set; } = string.Empty;

        /// <summary>Gets or sets the compared candidate identifier.</summary>
        public string CandidateId { get; set; } = string.Empty;

        /// <summary>Gets or sets named per-view observations.</summary>
        public IList<ShapeViewComparison> Views { get; set; } = new List<ShapeViewComparison>();

        /// <summary>Gets or sets localized actionable discrepancies.</summary>
        public IList<ShapeVisualDiscrepancy> Discrepancies { get; set; } = new List<ShapeVisualDiscrepancy>();
    }
}
