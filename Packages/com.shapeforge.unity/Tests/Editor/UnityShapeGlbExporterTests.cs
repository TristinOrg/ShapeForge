using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ShapeForge.Unity.Editor;
using UnityEngine;

namespace ShapeForge.Unity.Tests
{
    /// <summary>
    /// Verifies self-contained glTF geometry, hierarchy, and ShapeForge identity export.
    /// </summary>
    public sealed class UnityShapeGlbExporterTests
    {
        private const string OutputPath = "Library/ShapeForgeGlbTests/cube.glb";
        private GameObject root;

        [TearDown]
        public void TearDown()
        {
            if (root != null)
                UnityEngine.Object.DestroyImmediate(root);
            if (Directory.Exists(Path.GetDirectoryName(OutputPath)))
                Directory.Delete(Path.GetDirectoryName(OutputPath), true);
        }

        [Test]
        public void ExportWritesValidGlbWithGeometryAndStableIds()
        {
            ShapeNode definitionRoot = new("root", "Root", ShapeTypes.Group);
            definitionRoot.Add(new("cube", "Cube", TestGenerator.Type));
            ShapeDefinition definition = new("Cube", definitionRoot);
            root = new UnityShapeModelGenerator(new IUnityShapeGenerator[] { new TestGenerator() }).Generate(definition);

            UnityShapeGlbExportResult result = UnityShapeGlbExporter.Export(root, OutputPath);
            JObject document = ReadDocument(OutputPath);

            Assert.That(result.VertexCount, Is.EqualTo(24));
            Assert.That(result.TriangleCount, Is.EqualTo(12));
            Assert.That(document["asset"]?["version"]?.Value<string>(), Is.EqualTo("2.0"));
            Assert.That(document["meshes"], Has.Count.EqualTo(1));
            Assert.That(document["buffers"]?[0]?["byteLength"]?.Value<int>(), Is.GreaterThan(0));
            Assert.That(document.ToString(), Does.Contain("SHAPEFORGE_node"));
            Assert.That(document.ToString(), Does.Contain("\"cube\""));
        }

        private static JObject ReadDocument(string path)
        {
            using BinaryReader reader = new(File.OpenRead(path));
            Assert.That(reader.ReadUInt32(), Is.EqualTo(0x46546C67));
            Assert.That(reader.ReadUInt32(), Is.EqualTo(2));
            reader.ReadUInt32();
            int jsonLength = reader.ReadInt32();
            Assert.That(reader.ReadUInt32(), Is.EqualTo(0x4E4F534A));
            return JObject.Parse(Encoding.UTF8.GetString(reader.ReadBytes(jsonLength)).TrimEnd(' '));
        }

        private sealed class TestGenerator : IUnityShapeGenerator
        {
            public const string Type = "test/cube";
            public bool CanGenerate(ShapeNode node) => node?.Type == Type;
            public GameObject Generate(ShapeNode node, ShapeGenerationContext context) =>
                GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
    }
}
