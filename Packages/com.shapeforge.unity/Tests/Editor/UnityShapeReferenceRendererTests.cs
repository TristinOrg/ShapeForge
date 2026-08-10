using System.IO;
using NUnit.Framework;
using ShapeForge.Unity.Editor;
using UnityEngine;

namespace ShapeForge.Unity.Tests
{
    /// <summary>
    /// Verifies isolated transparent Editor rendering and layer restoration.
    /// </summary>
    public sealed class UnityShapeReferenceRendererTests
    {
        private const string OutputFolder = "Library/ShapeForgeRenderTests";
        private GameObject root;

        [TearDown]
        public void TearDown()
        {
            if (root != null)
                Object.DestroyImmediate(root);
            if (Directory.Exists(OutputFolder))
                Directory.Delete(OutputFolder, true);
        }

        [Test]
        public void RenderWritesEveryViewAndRestoresLayers()
        {
            root = GameObject.CreatePrimitive(PrimitiveType.Cube);
            root.layer = 7;
            ShapeRenderCaptureRequest request = new() { Id = "capture", CandidateId = "cube", Width = 128, Height = 128 };
            request.Views.Add(new() { Id = "front" });
            request.Views.Add(new() { Id = "side", Azimuth = 90f });

            ShapeRenderCaptureManifest manifest =
                UnityShapeReferenceRenderer.Render(root, request, OutputFolder);

            Assert.That(manifest.Images, Has.Count.EqualTo(2));
            Assert.That(new FileInfo(Path.Combine(OutputFolder, "front.png")).Length, Is.GreaterThan(100));
            Assert.That(new FileInfo(Path.Combine(OutputFolder, "side.png")).Length, Is.GreaterThan(100));
            Assert.That(root.layer, Is.EqualTo(7));
            Assert.That(Object.FindObjectsOfType<Camera>(), Has.None.Property("name").EqualTo("ShapeForge Capture Camera"));
        }
    }
}
