using System.IO;
using NUnit.Framework;
using ShapeForge.Unity;
using UnityEditor.PackageManager;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies normalized reference validation and deterministic semantic mapping.
    /// </summary>
    public sealed class LowPolyStylizedHumanReferenceTests
    {
        [Test]
        public void BaselineReferenceMapsToDefaultSemanticControls()
        {
            LowPolyStylizedHumanSpecification result = new LowPolyStylizedHumanReferenceMapper().Map(new());

            Assert.That(result.Proportions.ShoulderWidth, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(result.Proportions.BodyWidth, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(result.Proportions.LegLength, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(result.Head.Width, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(result.Head.Height, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(result.Head.Depth, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(result.Hair.Volume, Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void FrontMeasurementsMapToExpectedSemanticMultipliers()
        {
            LowPolyStylizedHumanReferenceSpecification reference = new();
            reference.Front.HeadWidth            = 0.264f;
            reference.Front.ShoulderWidth        = 0.374f;
            reference.Front.JawWidthToHeadWidth  = 0.702f;
            reference.Front.HairWidthToHeadWidth = 1.265f;
            reference.Front.Parting              = 0.3f;
            reference.Front.FringeLength         = 0.8f;
            reference.Front.SideburnLength       = 0.2f;

            LowPolyStylizedHumanSpecification result = new LowPolyStylizedHumanReferenceMapper().Map(reference);

            Assert.That(result.Head.Width, Is.EqualTo(1.1f).Within(0.0001f));
            Assert.That(result.Proportions.ShoulderWidth, Is.EqualTo(1.1f).Within(0.0001f));
            Assert.That(result.Head.JawWidth, Is.EqualTo(0.9f).Within(0.0001f));
            Assert.That(result.Hair.Volume, Is.EqualTo(1.1f).Within(0.0001f));
            Assert.That(result.Hair.Parting, Is.EqualTo(0.3f));
            Assert.That(result.Hair.FringeLength, Is.EqualTo(0.8f));
            Assert.That(result.Hair.SideburnLength, Is.EqualTo(0.2f));
        }

        [Test]
        public void MissingSideViewPreservesBaseDepthWithoutMutatingBase()
        {
            LowPolyStylizedHumanSpecification baseline = new();
            baseline.Head.Depth = 1.2f;

            LowPolyStylizedHumanSpecification result = new LowPolyStylizedHumanReferenceMapper().Map(new(), baseline);

            Assert.That(result, Is.Not.SameAs(baseline));
            Assert.That(result.Head, Is.Not.SameAs(baseline.Head));
            Assert.That(result.Head.Depth, Is.EqualTo(1.2f));
            Assert.That(baseline.Head.Width, Is.EqualTo(1f));
        }

        [Test]
        public void SideViewMapsDepthAndContributesToHairVolume()
        {
            LowPolyStylizedHumanReferenceSpecification reference = new()
            {
                Side = new()
                {
                    HeadDepth            = 0.231f,
                    HairDepthToHeadDepth = 1.188f
                }
            };

            LowPolyStylizedHumanSpecification result = new LowPolyStylizedHumanReferenceMapper().Map(reference);

            Assert.That(result.Head.Depth, Is.EqualTo(1.1f).Within(0.0001f));
            Assert.That(result.Hair.Volume, Is.EqualTo(1.05f).Within(0.0001f));
        }

        [Test]
        public void ValidatorRejectsUnsupportedOrUnrepresentableMeasurements()
        {
            LowPolyStylizedHumanReferenceSpecificationValidator validator = new();
            LowPolyStylizedHumanReferenceSpecification          schema    = new()
            {
                Schema = "unsupported/reference"
            };
            Assert.Throws<ShapeValidationException>(() => validator.Validate(schema));

            LowPolyStylizedHumanReferenceSpecification width = new();
            width.Front.HeadWidth = 0.4f;
            Assert.Throws<ShapeValidationException>(() => validator.Validate(width));
        }

        [Test]
        public void PublishedExampleDeserializesAndMaps()
        {
            PackageInfo package = PackageInfo.FindForAssembly(typeof(LowPolyStylizedHumanReferenceMapper).Assembly);
            string      folder  = Path.Combine(package.resolvedPath, "Documentation~", "Templates");
            string      schema  = File.ReadAllText(Path.Combine(folder, "stylized-human-reference-1.0.schema.json"));
            string      example = File.ReadAllText(Path.Combine(folder, "stylized-human-reference.example.json"));

            ShapeJsonSerializer                                 serializer = new();
            LowPolyStylizedHumanReferenceSpecificationValidator validator = new();
            LowPolyStylizedHumanReferenceSpecification         reference = serializer
                .DeserializeSpecification<LowPolyStylizedHumanReferenceSpecification>(example, validator.Validate);
            LowPolyStylizedHumanSpecification                   result = new LowPolyStylizedHumanReferenceMapper()
                .Map(reference);

            Assert.That(schema, Does.Contain(
                $"\"const\": \"{LowPolyStylizedHumanReferenceSpecification.CurrentSchema}\""));
            Assert.That(reference.Side, Is.Not.Null);
            Assert.That(result.Hair.Parting, Is.EqualTo(0.68f));
        }

        [Test]
        public void ExtractionPromptEmbedsSchemaAndForbidsDepthGuessing()
        {
            const string schema = "{\"title\":\"reference\"}";

            string prompt = LowPolyStylizedHumanReferencePrompt.Create(schema);

            Assert.That(prompt, Does.Contain(schema));
            Assert.That(prompt, Does.Contain("never infer depth"));
            Assert.That(prompt, Does.Contain("image-left 0"));
            Assert.That(prompt, Does.EndWith(schema));
            Assert.Throws<System.ArgumentException>(() => LowPolyStylizedHumanReferencePrompt.Create(" "));
        }

        [Test]
        public void PublishedExtractionGuideMatchesRuntimeProtocol()
        {
            PackageInfo package = PackageInfo.FindForAssembly(typeof(LowPolyStylizedHumanReferencePrompt).Assembly);
            string      folder  = Path.Combine(package.resolvedPath, "Documentation~", "Templates");
            string      guide   = File.ReadAllText(Path.Combine(folder, "stylized-human-reference.prompt.md"));

            Assert.That(guide, Does.Contain("top of hair to bottom of feet"));
            Assert.That(guide, Does.Contain("Never estimate depth from the front image"));
            Assert.That(guide, Does.Contain(nameof(LowPolyStylizedHumanReferencePrompt)));
        }

        [Test]
        public void AnalyzerReportsMissingSideCoverageWithoutInventingDiagnostics()
        {
            LowPolyStylizedHumanReferenceReport report = new LowPolyStylizedHumanReferenceAnalyzer().Analyze(new());

            Assert.That(report.HasSideView, Is.False);
            Assert.That(report.HasCompleteGeometryCoverage, Is.False);
            Assert.That(report.Diagnostics, Has.Count.EqualTo(10));
            Assert.That(report.Diagnostics, Has.None.Property("View")
                .EqualTo(LowPolyStylizedHumanReferenceView.Side));
        }

        [Test]
        public void AnalyzerReportsCompleteSideCoverageInStableOrder()
        {
            LowPolyStylizedHumanReferenceSpecification reference = new()
            {
                Side = new()
            };

            LowPolyStylizedHumanReferenceReport report = new LowPolyStylizedHumanReferenceAnalyzer()
                .Analyze(reference);

            Assert.That(report.HasCompleteGeometryCoverage, Is.True);
            Assert.That(report.Diagnostics, Has.Count.EqualTo(12));
            Assert.That(report.Diagnostics[0].Path, Is.EqualTo("front.headWidth"));
            Assert.That(report.Diagnostics[10].Path, Is.EqualTo("side.headDepth"));
            Assert.That(report.Diagnostics[11].Path, Is.EqualTo("side.hairDepthToHeadDepth"));
        }

        [Test]
        public void AnalyzerClassifiesSemanticDeviationMagnitude()
        {
            LowPolyStylizedHumanReferenceSpecification reference = new();
            reference.Front.HeadWidth     = 0.264f;
            reference.Front.ShoulderWidth = 0.408f;

            LowPolyStylizedHumanReferenceReport report = new LowPolyStylizedHumanReferenceAnalyzer()
                .Analyze(reference);

            Assert.That(report.Diagnostics[0].Multiplier, Is.EqualTo(1.1f).Within(0.0001f));
            Assert.That(report.Diagnostics[0].Deviation,
                Is.EqualTo(LowPolyStylizedHumanReferenceDeviation.Moderate));
            Assert.That(report.Diagnostics[2].Multiplier, Is.EqualTo(1.2f).Within(0.0001f));
            Assert.That(report.Diagnostics[2].Deviation,
                Is.EqualTo(LowPolyStylizedHumanReferenceDeviation.Strong));
        }
    }
}
