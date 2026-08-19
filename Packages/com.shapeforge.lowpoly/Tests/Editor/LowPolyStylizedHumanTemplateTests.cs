using System.IO;
using NUnit.Framework;
using ShapeForge.Unity;
using UnityEditor.PackageManager;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies semantic stylized-human validation, discovery, and geometry compilation.
    /// </summary>
    public sealed class LowPolyStylizedHumanTemplateTests
    {
        [Test]
        public void DefaultSpecificationPreservesHeroContract()
        {
            LowPolyStylizedHumanSpecification specification = new();

            ShapeDefinition definition = LowPolyStylizedHumanTemplate.Instance.Compile(specification);

            Assert.That(definition.Name, Is.EqualTo("Pocket Fantasy Hero"));
            Assert.That(definition.Style, Is.EqualTo(LowPolyHeroPreset.StyleId));
            Assert.That(definition.Root.Children, Has.Count.EqualTo(6));
            Assert.That(FindNode(definition.Root, "hero.head"), Is.Not.Null);
            Assert.That(FindNode(definition.Root, "hero.hair"), Is.Not.Null);
            Assert.That(FindNode(definition.Root, "hero.eye.left"), Is.Not.Null);
            Assert.That(FindNode(definition.Root, "hero.eye.right"), Is.Not.Null);
            Assert.That(FindNode(definition.Root, "hero.mouth"), Is.Not.Null);
            Assert.That(FindNode(definition.Root, "hero.spine.pivot"), Is.Not.Null);
        }

        [Test]
        public void SemanticControlsChangeCompiledProportionsAndHairProfile()
        {
            ShapeDefinition baseline = LowPolyStylizedHumanTemplate.Instance.Compile(new());
            LowPolyStylizedHumanSpecification customized = new()
            {
                Name         = "Custom Hero",
                OverallScale = 1.1f,
                Proportions  = new()
                {
                    HeadScale     = 1.2f,
                    ShoulderWidth = 1.25f,
                    BodyWidth     = 0.9f,
                    LegLength     = 1.15f
                },
                Head         = new()
                {
                    Width    = 1.1f,
                    Height   = 0.95f,
                    Depth    = 1.05f,
                    JawWidth = 0.82f
                },
                Hair         = new()
                {
                    Volume         = 1.12f,
                    Parting        = 0.3f,
                    FringeLength   = 0.8f,
                    SideburnLength = 0.2f
                },
                Face         = new()
                {
                    EyeScale    = 1.2f,
                    EyeSpacing  = 0.9f,
                    EyeOpenness = 0.75f,
                    MouthWidth  = 0.8f
                }
            };

            ShapeDefinition result = LowPolyStylizedHumanTemplate.Instance.Compile(customized);

            ShapeNode baselineHair = FindNode(baseline.Root, "hero.hair");
            ShapeNode resultHair   = FindNode(result.Root, "hero.hair");
            ShapeNode resultHead   = FindNode(result.Root, "hero.head");
            ShapeNode shoulder     = FindNode(result.Root, "hero.arm.right.shoulder.pivot");
            ShapeNode pants        = FindNode(result.Root, "hero.leg.left.pants");
            ShapeNode eye          = FindNode(result.Root, "hero.eye.left");

            Assert.That(result.Name, Is.EqualTo("Custom Hero"));
            Assert.That(result.Root.Transform.Scale, Is.EqualTo(new ForgeVector3(1.1f, 1.1f, 1.1f)));
            Assert.That(resultHead.Transform.Scale.X, Is.EqualTo(0.86f * 1.1f).Within(0.0001f));
            Assert.That(shoulder.Transform.Position.X, Is.EqualTo(0.36f * 1.25f * 0.9f).Within(0.0001f));
            Assert.That(pants.Transform.Scale.Y, Is.EqualTo(0.58f * 1.15f).Within(0.0001f));
            Assert.That(resultHair.Transform.Scale, Is.Not.EqualTo(baselineHair.Transform.Scale));
            Assert.That(resultHair.ProfileCageSections, Has.Count.EqualTo(5));
            Assert.That(resultHead.ProfileCageSections, Has.Count.EqualTo(5));
            Assert.That(eye.Transform.Position.X, Is.EqualTo(-0.205f * 0.9f).Within(0.0001f));
            Assert.That(eye.Transform.Scale.Y, Is.EqualTo(0.105f * 1.2f * 0.75f).Within(0.0001f));
        }

        [Test]
        public void ValidatorRejectsUnsupportedSchemaAndOutOfRangeControls()
        {
            LowPolyStylizedHumanSpecificationValidator validator     = new();
            LowPolyStylizedHumanSpecification          invalidSchema = new()
            {
                Schema = "unsupported/human"
            };
            Assert.Throws<ShapeValidationException>(() => validator.Validate(invalidSchema));

            LowPolyStylizedHumanSpecification invalidParting = new();
            invalidParting.Hair.Parting = 1f;
            Assert.Throws<ShapeValidationException>(() => validator.Validate(invalidParting));

            LowPolyStylizedHumanSpecification invalidFace = new();
            invalidFace.Face.EyeOpenness = 0f;
            Assert.Throws<ShapeValidationException>(() => validator.Validate(invalidFace));
        }

        [Test]
        public void LowPolyCatalogDiscoversStylizedHumanTemplate()
        {
            ShapeTemplateCatalog catalog = LowPolyShapeTemplateCatalog.Instance;

            Assert.That(catalog.TryGet("lowpoly/stylized-human/1.0", out IShapeTemplate template), Is.True);
            Assert.That(template, Is.SameAs(LowPolyStylizedHumanTemplate.Instance));
            Assert.That(template.Descriptor.Category, Is.EqualTo("character"));
            Assert.That(template.Descriptor.RequiredShapeTypes, Does.Contain(LowPolyShapeTypes.ProfileLoft));
            Assert.That(template.Descriptor.RequiredShapeTypes, Does.Contain(LowPolyShapeTypes.ProfileCage));
        }

        [Test]
        public void SpecificationRoundTripsThroughGenericJsonBoundary()
        {
            LowPolyStylizedHumanSpecification source = new();
            source.Hair.Parting = 0.32f;
            ShapeJsonSerializer                             serializer = new();
            LowPolyStylizedHumanSpecificationValidator validator  = new();

            string                                    json   = serializer.SerializeSpecification(source);
            LowPolyStylizedHumanSpecification result = serializer
                .DeserializeSpecification<LowPolyStylizedHumanSpecification>(json, validator.Validate);

            Assert.That(json, Does.Contain("\"schema\":\"shapeforge.lowpoly.stylized-human/1.0\""));
            Assert.That(result.Hair.Parting, Is.EqualTo(0.32f));
        }

        [Test]
        public void PublishedSchemaAndExampleMatchRuntimeSpecification()
        {
            PackageInfo package = PackageInfo.FindForAssembly(typeof(LowPolyStylizedHumanTemplate).Assembly);
            string      folder  = Path.Combine(package.resolvedPath, "Documentation~", "Templates");
            string      schema  = File.ReadAllText(Path.Combine(folder, "stylized-human-1.0.schema.json"));
            string      example = File.ReadAllText(Path.Combine(folder, "stylized-human.example.json"));

            ShapeJsonSerializer                             serializer = new();
            LowPolyStylizedHumanSpecificationValidator validator  = new();
            LowPolyStylizedHumanSpecification          result = serializer
                .DeserializeSpecification<LowPolyStylizedHumanSpecification>(example, validator.Validate);

            Assert.That(schema, Does.Contain(
                $"\"const\": \"{LowPolyStylizedHumanSpecification.CurrentSchema}\""));
            Assert.That(result.Name, Is.EqualTo("Asymmetric Fantasy Hero"));
            Assert.That(result.Hair.Parting, Is.EqualTo(0.7f));
        }

        private static ShapeNode FindNode(ShapeNode node, string id)
        {
            if (node.Id == id)
                return node;

            foreach (ShapeNode child in node.Children)
            {
                ShapeNode result = FindNode(child, id);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
