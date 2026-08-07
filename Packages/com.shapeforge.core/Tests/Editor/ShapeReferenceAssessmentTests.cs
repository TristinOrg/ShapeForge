using NUnit.Framework;

namespace ShapeForge.Tests
{
    /// <summary>Verifies structured reference assessment validation.</summary>
    public sealed class ShapeReferenceAssessmentTests
    {
        [Test]
        public void AnalyzeAcceptsBoundedSemanticAssessment()
        {
            ShapeReferenceAssessment assessment = new()
            {
                Subject         = "stylized humanoid",
                Style           = "low-poly fantasy",
                CameraAzimuth   = 25f,
                CameraElevation = 10f,
                DetailLevel     = "medium",
                Confidence      = 0.9f
            };
            assessment.VisibleFeatures.Add("shoulder armor");
            assessment.Uncertainties.Add("back of cape");

            Assert.That(new ShapeReferenceAssessmentValidator().Analyze(assessment).IsValid, Is.True);
        }

        [Test]
        public void AnalyzeReportsAllUnsafeAssumptions()
        {
            ShapeReferenceAssessment assessment = new()
            {
                CameraAzimuth   = 181f,
                CameraElevation = -91f,
                Confidence      = 2f
            };
            assessment.VisibleFeatures.Add("belt");
            assessment.VisibleFeatures.Add("belt");

            ShapeDiagnosticReport report = new ShapeReferenceAssessmentValidator().Analyze(assessment);

            Assert.That(report.Diagnostics.Count, Is.EqualTo(5));
            Assert.That(report.Diagnostics[0].Code, Is.EqualTo("shape.assessment.subject.required"));
            Assert.That(report.Diagnostics[4].Code, Is.EqualTo("shape.assessment.item.duplicate"));
        }
    }
}
