using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeForge
{
    /// <summary>
    /// Provides deterministic aggregate similarity and validated localized discrepancies.
    /// </summary>
    public sealed class ShapeRenderCompareReport
    {
        /// <summary>Initializes an immutable comparison report.</summary>
        public ShapeRenderCompareReport(
            float                               silhouetteScore,
            float                               proportionScore,
            float                               colorScore,
            float                               detailScore,
            float                               confidence,
            IEnumerable<ShapeVisualDiscrepancy> discrepancies,
            ShapeDiagnosticReport               diagnostics)
        {
            if (discrepancies == null)
                throw new ArgumentNullException(nameof(discrepancies));
            SilhouetteScore = silhouetteScore;
            ProportionScore = proportionScore;
            ColorScore      = colorScore;
            DetailScore     = detailScore;
            Confidence      = confidence;
            Discrepancies   = discrepancies.ToArray();
            Diagnostics     = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
        }

        /// <summary>Gets aggregate silhouette similarity.</summary>
        public float SilhouetteScore { get; }

        /// <summary>Gets aggregate proportion similarity.</summary>
        public float ProportionScore { get; }

        /// <summary>Gets aggregate color-block similarity.</summary>
        public float ColorScore { get; }

        /// <summary>Gets aggregate semantic-detail similarity.</summary>
        public float DetailScore { get; }

        /// <summary>Gets the equally weighted aggregate component score.</summary>
        public float OverallScore => (SilhouetteScore + ProportionScore + ColorScore + DetailScore) * 0.25f;

        /// <summary>Gets aggregate observation confidence.</summary>
        public float Confidence { get; }

        /// <summary>Gets localized provider observations in authored order.</summary>
        public IReadOnlyList<ShapeVisualDiscrepancy> Discrepancies { get; }

        /// <summary>Gets contract-validation diagnostics.</summary>
        public ShapeDiagnosticReport Diagnostics { get; }

        /// <summary>Gets whether the comparison contract is valid.</summary>
        public bool IsValid => Diagnostics.IsValid;

        /// <summary>Gets whether at least one blocking visual discrepancy exists.</summary>
        public bool HasBlockingDiscrepancies => Discrepancies.Any(item => item.Severity == ShapeVisualDiscrepancySeverity.Error);
    }
}
