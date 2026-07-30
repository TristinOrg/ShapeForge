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
        private static readonly Dictionary<int, List<ProfileMeshEntry>> ProfileMeshes = new();

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

        public static Mesh GetExtrudedProfile(IList<ForgeVector2> profile, float depth)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            if (profile.Count < 3)
                throw new ArgumentException("Extruded profiles require at least three points.", nameof(profile));

            if (depth <= 0f)
                throw new ArgumentOutOfRangeException(nameof(depth), depth, "Profile depth must be positive.");

            int hash = GetProfileHash(profile, depth);
            if (ProfileMeshes.TryGetValue(hash, out List<ProfileMeshEntry> entries))
            {
                foreach (ProfileMeshEntry entry in entries)
                {
                    if (entry.Matches(profile, depth))
                        return entry.Mesh;
                }
            }
            else
            {
                entries = new();
                ProfileMeshes.Add(hash, entries);
            }

            ForgeVector2[] points = CopyProfile(profile);
            Mesh           mesh   = CreateExtrudedProfile(points, depth);
            entries.Add(new(points, depth, mesh));
            return mesh;
        }

        private static int GetProfileHash(IList<ForgeVector2> profile, float depth)
        {
            int hash = depth.GetHashCode();
            for (int index = 0; index < profile.Count; index++)
                hash = (hash * 397) ^ profile[index].GetHashCode();

            return hash;
        }

        private static ForgeVector2[] CopyProfile(IList<ForgeVector2> profile)
        {
            ForgeVector2[] points = new ForgeVector2[profile.Count];
            for (int index = 0; index < profile.Count; index++)
                points[index] = profile[index];

            return points;
        }

        private static Mesh GetOrCreate(MeshKey key, Func<Mesh> create)
        {
            if (Meshes.TryGetValue(key, out Mesh mesh))
                return mesh;

            mesh = create();
            Meshes.Add(key, mesh);
            return mesh;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            Meshes.Clear();
            ProfileMeshes.Clear();
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

        private static Mesh CreateExtrudedProfile(ForgeVector2[] profile, float depth)
        {
            ForgeVector2[] points   = EnsureCounterClockwise(profile);
            List<int>     triangles = Triangulate(points);
            float         halfDepth = depth * 0.5f;
            MeshBuilder   builder   = new();

            for (int index = 0; index < triangles.Count; index += 3)
            {
                Vector3 first  = ToVector3(points[triangles[index]], -halfDepth);
                Vector3 second = ToVector3(points[triangles[index + 1]], -halfDepth);
                Vector3 third  = ToVector3(points[triangles[index + 2]], -halfDepth);
                builder.AddTriangle(third, second, first);

                first  = ToVector3(points[triangles[index]], halfDepth);
                second = ToVector3(points[triangles[index + 1]], halfDepth);
                third  = ToVector3(points[triangles[index + 2]], halfDepth);
                builder.AddTriangle(first, second, third);
            }

            for (int index = 0; index < points.Length; index++)
            {
                int next = (index + 1) % points.Length;
                builder.AddQuad(
                    ToVector3(points[index], -halfDepth),
                    ToVector3(points[next], -halfDepth),
                    ToVector3(points[next], halfDepth),
                    ToVector3(points[index], halfDepth));
            }

            return builder.Build("Low Poly Extruded Profile");
        }

        private static ForgeVector2[] EnsureCounterClockwise(ForgeVector2[] profile)
        {
            ForgeVector2[] points = (ForgeVector2[])profile.Clone();
            float          area   = SignedArea(points);
            if (Mathf.Approximately(area, 0f))
                throw new ArgumentException("Extruded profile points must enclose an area.", nameof(profile));

            if (area < 0f)
                Array.Reverse(points);

            return points;
        }

        private static float SignedArea(IList<ForgeVector2> points)
        {
            float area = 0f;
            for (int index = 0; index < points.Count; index++)
            {
                ForgeVector2 current = points[index];
                ForgeVector2 next    = points[(index + 1) % points.Count];
                area += (current.X * next.Y) - (next.X * current.Y);
            }

            return area * 0.5f;
        }

        private static List<int> Triangulate(IList<ForgeVector2> points)
        {
            List<int> remaining = new(points.Count);
            List<int> result    = new((points.Count - 2) * 3);
            for (int index = 0; index < points.Count; index++)
                remaining.Add(index);

            while (remaining.Count > 3)
            {
                bool clipped = false;
                for (int index = 0; index < remaining.Count; index++)
                {
                    int previous = remaining[(index + remaining.Count - 1) % remaining.Count];
                    int current  = remaining[index];
                    int next     = remaining[(index + 1) % remaining.Count];
                    if (!IsEar(points, remaining, previous, current, next))
                        continue;

                    result.Add(previous);
                    result.Add(current);
                    result.Add(next);
                    remaining.RemoveAt(index);
                    clipped = true;
                    break;
                }

                if (!clipped)
                    throw new ArgumentException("Extruded profiles must be simple non-self-intersecting polygons.", nameof(points));
            }

            result.Add(remaining[0]);
            result.Add(remaining[1]);
            result.Add(remaining[2]);
            return result;
        }

        private static bool IsEar(
            IList<ForgeVector2> points,
            IList<int>          remaining,
            int                 previous,
            int                 current,
            int                 next)
        {
            ForgeVector2 first  = points[previous];
            ForgeVector2 second = points[current];
            ForgeVector2 third  = points[next];
            if (Cross(first, second, third) <= 0f)
                return false;

            foreach (int candidate in remaining)
            {
                if (candidate == previous || candidate == current || candidate == next)
                    continue;

                if (IsInsideTriangle(points[candidate], first, second, third))
                    return false;
            }

            return true;
        }

        private static float Cross(ForgeVector2 first, ForgeVector2 second, ForgeVector2 third)
        {
            return ((second.X - first.X) * (third.Y - first.Y)) -
                   ((second.Y - first.Y) * (third.X - first.X));
        }

        private static bool IsInsideTriangle(
            ForgeVector2 point,
            ForgeVector2 first,
            ForgeVector2 second,
            ForgeVector2 third)
        {
            return Cross(first, second, point) >= 0f &&
                   Cross(second, third, point) >= 0f &&
                   Cross(third, first, point) >= 0f;
        }

        private static Vector3 ToVector3(ForgeVector2 point, float z)
        {
            return new(point.X, point.Y, z);
        }

        /// <summary>
        /// Builds flat-shaded meshes with duplicated face vertices.
        /// </summary>
        private sealed class MeshBuilder
        {
            private readonly List<Vector3> vertices  = new();
            private readonly List<Vector3> normals   = new();
            private readonly List<int>     triangles = new();

            public void AddTriangle(Vector3 first, Vector3 second, Vector3 third)
            {
                int start = vertices.Count;
                vertices.Add(first);
                vertices.Add(second);
                vertices.Add(third);
                Vector3 normal = Vector3.Cross(second - first, third - first).normalized;
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);
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
                Vector3 normal = Vector3.Cross(second - first, third - first).normalized;
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);
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
                mesh.SetNormals(normals);
                mesh.SetTriangles(triangles, 0, true);
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

        /// <summary>
        /// Owns an immutable profile snapshot so cache keys cannot change after insertion.
        /// </summary>
        private sealed class ProfileMeshEntry
        {
            private readonly float depth;

            public ProfileMeshEntry(ForgeVector2[] points, float depth, Mesh mesh)
            {
                this.depth = depth;
                Points     = points;
                Mesh       = mesh;
            }

            public ForgeVector2[] Points { get; }

            public Mesh Mesh { get; }

            public bool Matches(IList<ForgeVector2> profile, float candidateDepth)
            {
                if (!depth.Equals(candidateDepth) || Points.Length != profile.Count)
                    return false;

                for (int index = 0; index < Points.Length; index++)
                {
                    if (!Points[index].Equals(profile[index]))
                        return false;
                }

                return true;
            }
        }
    }
}
