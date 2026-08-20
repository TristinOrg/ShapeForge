using NUnit.Framework;

namespace ShapeForge.Tests
{
    /// <summary>
    /// Verifies category-neutral reference-blueprint validation.
    /// </summary>
    public sealed class ShapeReferenceBlueprintValidatorTests
    {
        [Test]
        public void CharacterAndBuildingBlueprintsUseTheSameContract()
        {
            ShapeReferenceBlueprint character = Blueprint("character/reference");
            ShapeReferenceBlueprint building  = Blueprint("building/reference");
            building.Classification.Category  = "building";
            building.Classification.Confidence = 0.92f;

            ShapeReferenceBlueprintValidator validator = new();

            Assert.That(validator.Analyze(character).IsValid, Is.True);
            Assert.That(validator.Analyze(building).IsValid, Is.True);
        }

        [Test]
        public void InvalidBoundsAndConfidenceFail()
        {
            ShapeReferenceBlueprint blueprint = Blueprint("invalid");
            blueprint.Views[0].ForegroundBounds.Width = 1.2f;
            blueprint.Views[0].Confidence             = float.NaN;

            ShapeDiagnosticReport report = new ShapeReferenceBlueprintValidator().Analyze(blueprint);

            Assert.That(report.Diagnostics, Has.Some.Property("Code").EqualTo("shape.reference.blueprint.view.bounds"));
            Assert.That(report.Diagnostics, Has.Some.Property("Code").EqualTo("shape.reference.blueprint.confidence"));
        }

        private static ShapeReferenceBlueprint Blueprint(string id)
        {
            ShapeReferenceBlueprint result = new() { Id = id, SourceImage = "reference.png" };
            ShapeReferenceBlueprintView view = new()
            {
                ViewId = "source", ImagePath = "source.png", Confidence = 0.8f,
                ForegroundBounds = new() { X = 0.1f, Y = 0.1f, Width = 0.8f, Height = 0.8f }
            };
            view.Silhouette.Add(new(0.1f, 0.1f));
            view.Silhouette.Add(new(0.9f, 0.1f));
            view.Silhouette.Add(new(0.5f, 0.9f));
            result.Views.Add(view);
            result.ReviewQueue.Add(new() { Kind = "hidden-geometry", Reason = "Not visible.", Required = true });
            return result;
        }
    }
}
