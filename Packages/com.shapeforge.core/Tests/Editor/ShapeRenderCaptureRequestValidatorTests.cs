using NUnit.Framework;

namespace ShapeForge.Tests
{
    /// <summary>
    /// Verifies bounded multi-view render-capture requests.
    /// </summary>
    public sealed class ShapeRenderCaptureRequestValidatorTests
    {
        [Test]
        public void ValidMultiViewRequestPasses()
        {
            ShapeRenderCaptureRequest request = Create();
            request.Views.Add(new() { Id = "side", Azimuth = 90f, FramingScale = 1.2f });

            Assert.That(new ShapeRenderCaptureRequestValidator().Analyze(request).IsValid, Is.True);
        }

        [Test]
        public void DuplicateViewsAndUnsafeResolutionFail()
        {
            ShapeRenderCaptureRequest request = Create();
            request.Width = 8192;
            request.Views.Add(new() { Id = "front" });

            ShapeDiagnosticReport report = new ShapeRenderCaptureRequestValidator().Analyze(request);

            Assert.That(report.IsValid, Is.False);
            Assert.That(report.Diagnostics, Has.Some.Property("Code").EqualTo("shape.capture.width.invalid"));
            Assert.That(report.Diagnostics, Has.Some.Property("Code").EqualTo("shape.capture.view.id.duplicate"));
        }

        private static ShapeRenderCaptureRequest Create()
        {
            ShapeRenderCaptureRequest request = new() { Id = "capture", CandidateId = "candidate" };
            request.Views.Add(new() { Id = "front" });
            return request;
        }
    }
}
