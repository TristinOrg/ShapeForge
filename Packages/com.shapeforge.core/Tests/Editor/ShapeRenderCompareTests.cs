using NUnit.Framework;

namespace ShapeForge.Tests
{
    /// <summary>
    /// Verifies provider-neutral render comparison validation and aggregation.
    /// </summary>
    public sealed class ShapeRenderCompareTests
    {
        [Test]
        public void AggregateWeightsViewsAndPreservesLocalizedDiscrepancies()
        {
            ShapeRenderComparison comparison = Comparison();
            comparison.Views.Add(View("front", 3f, 1f, 0.8f, 0.6f, 0.4f, 1f));
            comparison.Views.Add(View("side", 1f, 0.5f, 0.4f, 0.2f, 0f, 0.6f));
            comparison.Discrepancies.Add(new()
            {
                Id = "head-width", Category = "proportion", ViewId = "front",
                NodeId = "head", Severity = ShapeVisualDiscrepancySeverity.Warning,
                Message = "Head is too wide.", SuggestedAction = "Reduce head scale on X."
            });

            ShapeRenderCompareReport report = new ShapeRenderCompareAggregator().Aggregate(comparison);

            Assert.That(report.IsValid, Is.True);
            Assert.That(report.SilhouetteScore, Is.EqualTo(0.875f));
            Assert.That(report.ProportionScore, Is.EqualTo(0.7f));
            Assert.That(report.ColorScore, Is.EqualTo(0.5f));
            Assert.That(report.DetailScore, Is.EqualTo(0.3f));
            Assert.That(report.Confidence, Is.EqualTo(0.9f));
            Assert.That(report.OverallScore, Is.EqualTo(0.59375f));
            Assert.That(report.Discrepancies[0].NodeId, Is.EqualTo("head"));
        }

        [Test]
        public void ValidatorReportsAllUnsafeViewAndDiscrepancyValues()
        {
            ShapeRenderComparison comparison = Comparison();
            comparison.Views.Add(View("front", 0f, 2f, -1f, 0f, 0f, 0f));
            comparison.Discrepancies.Add(new()
            {
                Id = "missing-view", ViewId = "back", Message = string.Empty
            });

            ShapeDiagnosticReport report = new ShapeRenderComparisonValidator().Analyze(comparison);

            Assert.That(report.IsValid, Is.False);
            Assert.That(report.Diagnostics.Count, Is.EqualTo(5));
            Assert.That(report.Diagnostics[0].Code, Is.EqualTo("shape.compare.view.weight.invalid"));
            Assert.That(report.Diagnostics[1].Code, Is.EqualTo("shape.compare.view.confidence.invalid"));
            Assert.That(report.Diagnostics[2].Code, Is.EqualTo("shape.compare.score.invalid"));
            Assert.That(report.Diagnostics[3].Code, Is.EqualTo("shape.compare.discrepancy.view.unknown"));
            Assert.That(report.Diagnostics[4].Code, Is.EqualTo("shape.compare.discrepancy.message.required"));
        }

        [Test]
        public void AggregateRejectsMissingViewsWithoutDividingByZero()
        {
            ShapeRenderCompareReport report = new ShapeRenderCompareAggregator().Aggregate(Comparison());

            Assert.That(report.IsValid, Is.False);
            Assert.That(report.OverallScore, Is.Zero);
            Assert.That(report.Diagnostics.Diagnostics[0].Code, Is.EqualTo("shape.compare.views.required"));
        }

        private static ShapeRenderComparison Comparison() => new()
        {
            ReferenceId = "reference/front-side",
            CandidateId = "hero/revision-3"
        };

        private static ShapeViewComparison View(
            string id,
            float  weight,
            float  confidence,
            float  silhouette,
            float  proportion,
            float  color,
            float  detail)
        {
            return new()
            {
                ViewId    = id,
                Weight    = weight,
                Confidence = confidence,
                Scores     = new ShapeComparisonScores
                {
                    Silhouette = silhouette,
                    Proportion = proportion,
                    Color      = color,
                    Detail     = detail
                }
            };
        }
    }
}
