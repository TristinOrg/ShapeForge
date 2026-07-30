using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Creates and shares immutable procedural meshes for equivalent Low Poly parameters.
    /// </summary>
    internal static class LowPolyProceduralMeshCache
    {
        private static readonly Dictionary<MeshKey, Mesh> Meshes = new();

        public static Mesh GetWedge()
        {
            MeshKey key = new(LowPolyShapeTypes.Wedge, 0f, 0f, 0f, 0f);
            return GetOrCreate(key, CreateWedge);
        }

        public static Mesh GetFrustum(float topWidth, float topDepth, float bottomWidth, float bottomDepth)
        {
            MeshKey key = new(
                LowPolyShapeTypes.Frustum,
                topWidth,
                topDepth,
                bottomWidth,
                bottomDepth);
            return GetOrCreate(key, () => CreateFrustum(topWidth, topDepth, bottomWidth, bottomDepth));
        }

        private static Mesh GetOrCreate(MeshKey key, Func<Mesh> create)
        {
            if (Meshes.TryGetValue(key, out Mesh mesh))
                return mesh;

            mesh = create();
            mesh.hideFlags = HideFlags.HideAndDontSave;
            Meshes.Add(key, mesh);
            return mesh;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            Meshes.Clear();
        }

        private static Mesh CreateWedge()
        {
            Vector3 bottomFrontLeft  = new(-0.5f, -0.5f, -0.5f);
            Vector3 bottomFrontRight = new(0.5f, -0.5f, -0.5f);
            Vector3 bottomBackRight  = new(0.5f, -0.5f, 0.5f);
            Vector3 bottomBackLeft   = new(-0.5f, -0.5f, 0.5f);
            Vector3 topBackRight     = new(0.5f, 0.5f, 0.5f);
            Vector3 topBackLeft      = new(-0.5f, 0.5f, 0.5f);
            MeshBuilder builder      = new();

            builder.AddQuad(bottomFrontLeft, bottomFrontRight, bottomBackRight, bottomBackLeft);
            builder.AddQuad(bottomFrontRight, bottomFrontLeft, topBackLeft, topBackRight);
            builder.AddQuad(bottomBackLeft, bottomBackRight, topBackRight, topBackLeft);
            builder.AddTriangle(bottomFrontLeft, bottomBackLeft, topBackLeft);
            builder.AddTriangle(bottomBackRight, bottomFrontRight, topBackRight);
            return builder.Build("Low Poly Wedge");
        }

        private static Mesh CreateFrustum(
            float topWidth,
            float topDepth,
            float bottomWidth,
            float bottomDepth)
        {
            float topHalfWidth    = topWidth * 0.5f;
            float topHalfDepth    = topDepth * 0.5f;
            float bottomHalfWidth = bottomWidth * 0.5f;
            float bottomHalfDepth = bottomDepth * 0.5f;
            Vector3 bottomFrontLeft  = new(-bottomHalfWidth, -0.5f, -bottomHalfDepth);
            Vector3 bottomFrontRight = new(bottomHalfWidth, -0.5f, -bottomHalfDepth);
            Vector3 bottomBackRight  = new(bottomHalfWidth, -0.5f, bottomHalfDepth);
            Vector3 bottomBackLeft   = new(-bottomHalfWidth, -0.5f, bottomHalfDepth);
            Vector3 topFrontLeft     = new(-topHalfWidth, 0.5f, -topHalfDepth);
            Vector3 topFrontRight    = new(topHalfWidth, 0.5f, -topHalfDepth);
            Vector3 topBackRight     = new(topHalfWidth, 0.5f, topHalfDepth);
            Vector3 topBackLeft      = new(-topHalfWidth, 0.5f, topHalfDepth);
            MeshBuilder builder      = new();

            builder.AddQuad(bottomFrontLeft, bottomFrontRight, bottomBackRight, bottomBackLeft);
            builder.AddQuad(topBackLeft, topBackRight, topFrontRight, topFrontLeft);
            builder.AddQuad(bottomFrontRight, bottomFrontLeft, topFrontLeft, topFrontRight);
            builder.AddQuad(bottomBackLeft, bottomBackRight, topBackRight, topBackLeft);
            builder.AddQuad(bottomFrontLeft, bottomBackLeft, topBackLeft, topFrontLeft);
            builder.AddQuad(bottomBackRight, bottomFrontRight, topFrontRight, topBackRight);
            return builder.Build("Low Poly Frustum");
        }

        /// <summary>
        /// Builds flat-shaded meshes with duplicated face vertices.
        /// </summary>
        private sealed class MeshBuilder
        {
            private readonly List<Vector3> vertices  = new();
            private readonly List<int>     triangles = new();

            public void AddTriangle(Vector3 first, Vector3 second, Vector3 third)
            {
                int start = vertices.Count;
                vertices.Add(first);
                vertices.Add(second);
                vertices.Add(third);
                triangles.Add(start);
                triangles.Add(start + 1);
                triangles.Add(start + 2);
            }

            public void AddQuad(Vector3 first, Vector3 second, Vector3 third, Vector3 fourth)
            {
                int start = vertices.Count;
                vertices.Add(first);
                vertices.Add(second);
                vertices.Add(third);
                vertices.Add(fourth);
                triangles.Add(start);
                triangles.Add(start + 1);
                triangles.Add(start + 2);
                triangles.Add(start);
                triangles.Add(start + 2);
                triangles.Add(start + 3);
            }

            public Mesh Build(string name)
            {
                Mesh mesh = new() { name = name };
                mesh.SetVertices(vertices);
                mesh.SetTriangles(triangles, 0, true);
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                return mesh;
            }
        }

        private readonly struct MeshKey : IEquatable<MeshKey>
        {
            private readonly string type;
            private readonly float  first;
            private readonly float  second;
            private readonly float  third;
            private readonly float  fourth;

            public MeshKey(string type, float first, float second, float third, float fourth)
            {
                this.type   = type;
                this.first  = first;
                this.second = second;
                this.third  = third;
                this.fourth = fourth;
            }

            public bool Equals(MeshKey other)
            {
                return string.Equals(type, other.type, StringComparison.Ordinal) &&
                       first.Equals(other.first) &&
                       second.Equals(other.second) &&
                       third.Equals(other.third) &&
                       fourth.Equals(other.fourth);
            }

            public override bool Equals(object obj)
            {
                return obj is MeshKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = StringComparer.Ordinal.GetHashCode(type);
                    hash = (hash * 397) ^ first.GetHashCode();
                    hash = (hash * 397) ^ second.GetHashCode();
                    hash = (hash * 397) ^ third.GetHashCode();
                    return (hash * 397) ^ fourth.GetHashCode();
                }
            }
        }
    }
}
