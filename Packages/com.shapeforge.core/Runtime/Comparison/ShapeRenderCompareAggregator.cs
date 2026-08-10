using System;

namespace ShapeForge
{
    /// <summary>
    /// Aggregates validated per-view observations without depending on a renderer or vision provider.
    /// </summary>
    public sealed class ShapeRenderCompareAggregator
    {
        /// <summary>Validates and aggregates a render comparison.</summary>
        public ShapeRenderCompareReport Aggregate(ShapeRenderComparison comparison)
        {
            ShapeDiagnosticReport diagnostics = new ShapeRenderComparisonValidator().Analyze(comparison);
            if (!diagnostics.IsValid)
                return new(0f, 0f, 0f, 0f, 0f, Array.Empty<ShapeVisualDiscrepancy>(), diagnostics);

            float totalWeight = 0f;
            float silhouette = 0f;
            float proportion = 0f;
            float color      = 0f;
            float detail     = 0f;
            float confidence = 0f;
            foreach (ShapeViewComparison view in comparison.Views)
            {
                totalWeight += view.Weight;
                silhouette += view.Scores.Silhouette * view.Weight;
                proportion += view.Scores.Proportion * view.Weight;
                color      += view.Scores.Color * view.Weight;
                detail     += view.Scores.Detail * view.Weight;
                confidence += view.Confidence * view.Weight;
            }

            return new(
                silhouette / totalWeight,
                proportion / totalWeight,
                color / totalWeight,
                detail / totalWeight,
                confidence / totalWeight,
                comparison.Discrepancies,
                diagnostics);
        }
    }
}
