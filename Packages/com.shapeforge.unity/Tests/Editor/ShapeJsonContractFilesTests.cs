using System.IO;
using NUnit.Framework;
using UnityEditor.PackageManager;

namespace ShapeForge.Unity.Tests
{
    /// <summary>
    /// Verifies the published cross-engine JSON Schema documents and examples.
    /// </summary>
    public sealed class ShapeJsonContractFilesTests
    {
        [Test]
        public void PublishedSchemasUseCurrentDocumentIdentifiers()
        {
            string shapeSchema   = ReadText("Schemas/shapeforge.shape-1.0.schema.json");
            string styleSchema   = ReadText("Schemas/shapeforge.style-1.0.schema.json");
            string patchSchema   = ReadText("Schemas/shapeforge.patch-1.0.schema.json");
            string qualitySchema = ReadText("Schemas/shapeforge.quality-1.0.schema.json");
            string assessmentSchema = ReadText("Schemas/shapeforge.reference-assessment-1.0.schema.json");
            string inventorySchema = ReadText("Schemas/shapeforge.detail-inventory-1.0.schema.json");

            Assert.That(shapeSchema, Does.Contain($"\"const\": \"{ShapeDefinition.CurrentSchema}\""));
            Assert.That(styleSchema, Does.Contain($"\"const\": \"{ShapeStyleDefinition.CurrentSchema}\""));
            Assert.That(patchSchema, Does.Contain($"\"const\": \"{ShapePatchDocument.CurrentSchema}\""));
            Assert.That(qualitySchema, Does.Contain($"\"const\": \"{ShapeQualityPolicy.CurrentSchema}\""));
            Assert.That(assessmentSchema, Does.Contain($"\"const\": \"{ShapeReferenceAssessment.CurrentSchema}\""));
            Assert.That(inventorySchema, Does.Contain($"\"const\": \"{ShapeDetailInventory.CurrentSchema}\""));
            Assert.That(shapeSchema, Does.Contain("https://json-schema.org/draft/2020-12/schema"));
            Assert.That(styleSchema, Does.Contain("https://json-schema.org/draft/2020-12/schema"));
            Assert.That(patchSchema, Does.Contain("https://json-schema.org/draft/2020-12/schema"));
            Assert.That(qualitySchema, Does.Contain("https://json-schema.org/draft/2020-12/schema"));
        }

        [Test]
        public void PublishedExamplesDeserializeThroughReferenceAdapter()
        {
            ShapeJsonSerializer serializer = new();
            string              shapeJson   = ReadText("Examples/minimal-shape.json");
            string              styleJson   = ReadText("Examples/minimal-style.json");
            string              patchJson   = ReadText("Examples/minimal-patch.json");
            string              qualityJson = ReadText("Examples/minimal-quality-policy.json");
            string              assessmentJson = ReadText("Examples/minimal-reference-assessment.json");
            string              inventoryJson = ReadText("Examples/minimal-detail-inventory.json");

            ShapeDefinition      shape = serializer.DeserializeShape(shapeJson);
            ShapeStyleDefinition style = serializer.DeserializeStyle(styleJson);
            ShapePatchDocument   patch   = serializer.DeserializePatch(patchJson);
            ShapeQualityPolicy   quality = serializer.DeserializeQualityPolicy(qualityJson);
            ShapeReferenceAssessment assessment = serializer.DeserializeReferenceAssessment(assessmentJson);
            ShapeDetailInventory inventory = serializer.DeserializeDetailInventory(inventoryJson);

            Assert.That(shape.Root.Id, Is.EqualTo("model"));
            Assert.That(style.Id, Is.EqualTo("example/default"));
            Assert.That(patch.Operations[0].NodeId, Is.EqualTo("model"));
            Assert.That(quality.Id, Is.EqualTo("example/runtime-prop"));
            Assert.That(assessment.VisibleFeatures[0], Is.EqualTo("shoulder armor"));
            Assert.That(inventory.Details[0].Id, Is.EqualTo("body"));
        }

        private static string ReadText(string relativePath)
        {
            PackageInfo package = PackageInfo.FindForAssembly(typeof(ShapeDefinition).Assembly);
            string      path    = Path.Combine(package.resolvedPath, "Documentation~", relativePath);
            return File.ReadAllText(path);
        }
    }
}
