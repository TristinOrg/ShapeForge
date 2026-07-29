using NUnit.Framework;

namespace ShapeForge.Tests
{
    /// <summary>
    /// Verifies the engine-agnostic fluent model authoring API.
    /// </summary>
    public sealed class ShapeBuilderTests
    {
        [Test]
        public void BuildCreatesNestedModelWithoutEngineTypes()
        {
            ShapeDefinition definition = ShapeBuilder
                .Create("House")
                .WithStyle("lowpoly/village")
                .Root("house", "House", root => root
                    .Group("walls", "Walls", walls => walls
                        .Shape("front", "Front Wall", "example/cube", shape => shape
                            .Position(0f, 1f, 0f)
                            .Scale(4f, 2f, 0.2f)
                            .ColorRole("wall"))))
                .Build();

            ShapeNode wall = definition.Root.Children[0].Children[0];

            Assert.That(definition.Style, Is.EqualTo("lowpoly/village"));
            Assert.That(wall.Id, Is.EqualTo("front"));
            Assert.That(wall.Transform.Position, Is.EqualTo(new ForgeVector3(0f, 1f, 0f)));
            Assert.That(wall.Appearance.ColorRole, Is.EqualTo("wall"));
        }

        [Test]
        public void BuildRejectsDuplicateIds()
        {
            ShapeBuilder builder = ShapeBuilder
                .Create("Invalid")
                .Root("root", "Root", root => root
                    .Shape("part", "Part A", "example/cube")
                    .Shape("part", "Part B", "example/cube"));

            Assert.Throws<ShapeValidationException>(() => builder.Build());
        }

        [Test]
        public void BuildRejectsColorOutsideSpecificationRange()
        {
            ShapeBuilder builder = ShapeBuilder
                .Create("Invalid Color")
                .Root("root", "Root", root => root
                    .Shape("part", "Part", "example/cube", shape => shape
                        .Color(1.2f, 0f, 0f)));

            Assert.Throws<ShapeValidationException>(() => builder.Build());
        }
    }
}
