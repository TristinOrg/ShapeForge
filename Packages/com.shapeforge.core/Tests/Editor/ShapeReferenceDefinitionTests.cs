using NUnit.Framework;

namespace ShapeForge.Tests
{
    /// <summary>
    /// Verifies generic multi-view reference validation and coverage diagnostics.
    /// </summary>
    public sealed class ShapeReferenceDefinitionTests
    {
        [Test]
        public void AnalyzerReportsCompleteConsistentPartCoverage()
        {
            ShapeReferenceDefinition definition = Definition(
                View(0.2f, 0.1f, 0.8f, 0.9f),
                View(0.3f, 0.1f, 0.7f, 0.9f),
                View(0.2f, 0.1f, 0.8f, 0.9f));

            ShapeReferenceCoverageReport report = new ShapeReferenceCoverageAnalyzer().Analyze(definition);

            Assert.That(report.PartCount, Is.EqualTo(1));
            Assert.That(report.CompletePartCount, Is.EqualTo(1));
            Assert.That(report.HasCompleteCoverage, Is.True);
            Assert.That(report.IsConsistent, Is.True);
        }

        [Test]
        public void AnalyzerReportsMissingViewAndHeightMismatch()
        {
            ShapeReferenceDefinition definition = Definition(
                View(0.2f, 0.1f, 0.8f, 0.9f),
                View(0.3f, 0.2f, 0.7f, 0.8f),
                null);

            ShapeReferenceCoverageReport report = new ShapeReferenceCoverageAnalyzer().Analyze(definition);

            Assert.That(report.HasCompleteCoverage, Is.False);
            Assert.That(report.IsConsistent, Is.False);
            Assert.That(report.InconsistentPartIds, Is.EqualTo(new[] { "head" }));
        }

        [Test]
        public void ValidatorRejectsSilhouetteOutsidePartBounds()
        {
            ShapeReferenceViewObservation front = View(0.2f, 0.1f, 0.8f, 0.9f);
            front.Silhouette.Add(new(0.1f, 0.5f));
            front.Silhouette.Add(new(0.5f, 0.1f));
            front.Silhouette.Add(new(0.8f, 0.9f));

            ShapeReferenceDefinition definition = Definition(front, null, null);

            Assert.Throws<ShapeValidationException>(() => new ShapeReferenceDefinitionValidator().Validate(definition));
        }

        [Test]
        public void ExtractionPromptForbidsInventingMissingViews()
        {
            const string schema = "{\"title\":\"reference\"}";

            string prompt = ShapeReferenceExtractionPrompt.Create(schema);

            Assert.That(prompt, Does.Contain("stable semantic part IDs"));
            Assert.That(prompt, Does.Contain("never infer unseen depth"));
            Assert.That(prompt, Does.EndWith(schema));
            Assert.Throws<System.ArgumentException>(() => ShapeReferenceExtractionPrompt.Create(" "));
        }

        private static ShapeReferenceDefinition Definition(
            ShapeReferenceViewObservation front,
            ShapeReferenceViewObservation side,
            ShapeReferenceViewObservation back)
        {
            ShapeReferenceDefinition definition = new()
            {
                Name = "Character"
            };
            definition.Parts.Add(new ShapeReferencePart
            {
                Id    = "head",
                Front = front,
                Side  = side,
                Back  = back
            });
            return definition;
        }

        private static ShapeReferenceViewObservation View(
            float minimumX,
            float minimumY,
            float maximumX,
            float maximumY)
        {
            return new ShapeReferenceViewObservation
            {
                Minimum = new(minimumX, minimumY),
                Maximum = new(maximumX, maximumY)
            };
        }
    }
}
