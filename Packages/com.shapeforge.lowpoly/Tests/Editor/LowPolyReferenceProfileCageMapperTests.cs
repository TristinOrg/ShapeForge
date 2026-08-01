using NUnit.Framework;
using ShapeForge.Unity;
using UnityEngine;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies deterministic multi-view silhouette reconstruction into Low Poly profile cages.
    /// </summary>
    public sealed class LowPolyReferenceProfileCageMapperTests
    {
        private GameObject generatedRoot;

        [TearDown]
        public void TearDown()
        {
            if (generatedRoot != null)
                Object.DestroyImmediate(generatedRoot);
        }

        [Test]
        public void MapCombinesFrontShapeAndSideDepthIntoCompatibleSections()
        {
            ShapeReferenceDefinition reference = CreateReference();

            LowPolyReferenceProfileCage cage = new LowPolyReferenceProfileCageMapper()
                .Map(reference, "head", 12, 5);

            AssertVector(cage.Position, new(0f, 0.5f, 0f));
            AssertVector(cage.Scale, new(0.6f, 0.8f, 0.4f));
            Assert.That(cage.Sections.Count, Is.EqualTo(5));
            Assert.That(cage.Sections[0].Z, Is.EqualTo(-0.5f));
            Assert.That(cage.Sections[2].Profile, Has.Count.EqualTo(12));
            Assert.That(cage.Sections[2].Profile[0].Y, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(cage.Sections[0].Profile[0].Y, Is.LessThan(cage.Sections[2].Profile[0].Y));
        }

        private static void AssertVector(ForgeVector3 actual, ForgeVector3 expected)
        {
            Assert.That(actual.X, Is.EqualTo(expected.X).Within(0.0001f));
            Assert.That(actual.Y, Is.EqualTo(expected.Y).Within(0.0001f));
            Assert.That(actual.Z, Is.EqualTo(expected.Z).Within(0.0001f));
        }

        [Test]
        public void MappedSectionsGenerateAProfileCageMesh()
        {
            LowPolyReferenceProfileCage cage = new LowPolyReferenceProfileCageMapper()
                .Map(CreateReference(), "head", 12, 5);
            ShapeNode node = new("head", "Reference Head", LowPolyShapeTypes.ProfileCage);
            node.Transform.Position = cage.Position;
            node.Transform.Scale    = cage.Scale;
            foreach (ShapeProfileCageSection section in cage.Sections)
                node.ProfileCageSections.Add(section);

            generatedRoot = new UnityShapeModelGenerator(new IUnityShapeGenerator[]
            {
                new LowPolyPrimitiveGenerator()
            }).Generate(new ShapeDefinition("Reference Head", node));

            Mesh mesh = generatedRoot.GetComponent<MeshFilter>().sharedMesh;
            Assert.That(mesh.name, Is.EqualTo("Low Poly Profile Cage"));
            Assert.That(mesh.vertexCount, Is.GreaterThan(0));
        }

        [Test]
        public void MapRejectsBoundsWithoutMeasuredSilhouettes()
        {
            ShapeReferenceDefinition reference = CreateReference();
            reference.Parts[0].Side.Silhouette.Clear();

            Assert.Throws<ShapeValidationException>(() =>
                new LowPolyReferenceProfileCageMapper().Map(reference, "head"));
        }

        private static ShapeReferenceDefinition CreateReference()
        {
            ShapeReferenceViewObservation front = View(0.2f, 0.1f, 0.8f, 0.9f);
            front.Silhouette.Add(new(0.5f, 0.9f));
            front.Silhouette.Add(new(0.8f, 0.7f));
            front.Silhouette.Add(new(0.8f, 0.3f));
            front.Silhouette.Add(new(0.5f, 0.1f));
            front.Silhouette.Add(new(0.2f, 0.3f));
            front.Silhouette.Add(new(0.2f, 0.7f));

            ShapeReferenceViewObservation side = View(0.3f, 0.1f, 0.7f, 0.9f);
            side.Silhouette.Add(new(0.5f, 0.9f));
            side.Silhouette.Add(new(0.7f, 0.5f));
            side.Silhouette.Add(new(0.5f, 0.1f));
            side.Silhouette.Add(new(0.3f, 0.5f));

            ShapeReferenceDefinition reference = new()
            {
                Name = "Character"
            };
            reference.Parts.Add(new ShapeReferencePart
            {
                Id    = "head",
                Front = front,
                Side  = side
            });
            return reference;
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
