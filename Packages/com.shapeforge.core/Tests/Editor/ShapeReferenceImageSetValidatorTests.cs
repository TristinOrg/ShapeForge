using NUnit.Framework;

namespace ShapeForge.Tests
{
    /// <summary>
    /// Verifies bounded named reference-image manifests.
    /// </summary>
    public sealed class ShapeReferenceImageSetValidatorTests
    {
        [Test]
        public void ValidViewsPass()
        {
            ShapeReferenceImageSet reference = new() { Id = "hero/reference" };
            reference.Images.Add(new() { ViewId = "front", ImagePath = "front.png", Weight = 2f });
            reference.Images.Add(new() { ViewId = "side", ImagePath = "side.png" });

            Assert.That(new ShapeReferenceImageSetValidator().Analyze(reference).IsValid, Is.True);
        }

        [Test]
        public void DuplicateViewAndEmptyPathFail()
        {
            ShapeReferenceImageSet reference = new() { Id = "hero/reference" };
            reference.Images.Add(new() { ViewId = "front", ImagePath = "front.png" });
            reference.Images.Add(new() { ViewId = "front", ImagePath = "" });

            ShapeDiagnosticReport report = new ShapeReferenceImageSetValidator().Analyze(reference);

            Assert.That(report.IsValid, Is.False);
            Assert.That(report.Diagnostics, Has.Some.Property("Code").EqualTo("shape.reference.image.view.duplicate"));
            Assert.That(report.Diagnostics, Has.Some.Property("Code").EqualTo("shape.reference.image.path"));
        }
    }
}
