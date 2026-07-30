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
        private static readonly Dictionary<MeshKey, Mesh>                    Meshes        = new();
        private static readonly Dictionary<int, List<ProfileMeshEntry>>      ProfileMeshes = new();
        private static readonly Dictionary<int, List<LoftMeshEntry>>         LoftMeshes    = new();

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

        public static Mesh GetExtrudedProfile(IList<ForgeVector2> profile, float depth, float bevel)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            if (profile.Count < 3)
                throw new ArgumentException("Extruded profiles require at least three points.", nameof(profile));

            if (depth <= 0f)
                throw new ArgumentOutOfRangeException(nameof(depth), depth, "Profile depth must be positive.");

            if (bevel < 0f || bevel * 2f >= depth)
                throw new ArgumentOutOfRangeException(
                    nameof(bevel),
                    bevel,
                    "Profile bevel must be non-negative and less than half the profile depth.");

            int hash = GetProfileHash(profile, depth, bevel);
            if (ProfileMeshes.TryGetValue(hash, out List<ProfileMeshEntry> entries))
            {
                for (int index = entries.Count - 1; index >= 0; index--)
                {
                    ProfileMeshEntry entry = entries[index];
                    if (entry.Mesh == null)
                    {
                        entries.RemoveAt(index);
                        continue;
                    }

                    if (entry.Matches(profile, depth, bevel))
                        return entry.Mesh;
                }
            }
            else
            {
                entries = new();
                ProfileMeshes.Add(hash, entries);
            }

            ForgeVector2[] points = CopyProfile(profile);
            Mesh           mesh   = CreateExtrudedProfile(points, depth, bevel);
            entries.Add(new(points, depth, bevel, mesh));
            return mesh;
        }

        public static Mesh GetProfileLoft(
            IList<ForgeVector2>       profile,
            IList<ShapeProfileSection> sections)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            if (profile.Count < 3)
                throw new ArgumentException("Profile lofts require at least three points.", nameof(profile));

            ValidateLoftSections(sections);
            int hash = GetLoftHash(profile, sections);
            if (LoftMeshes.TryGetValue(hash, out List<LoftMeshEntry> entries))
            {
                for (int index = entries.Count - 1; index >= 0; index--)
                {
                    LoftMeshEntry entry = entries[index];
                    if (entry.Mesh == null)
                    {
                        entries.RemoveAt(index);
                        continue;
                    }

                    if (entry.Matches(profile, sections))
                        return entry.Mesh;
                }
            }
            else
            {
                entries = new();
                LoftMeshes.Add(hash, entries);
            }

            ForgeVector2[] points   = CopyProfile(profile);
            LoftSection[] loft      = CopySections(sections);
            Mesh          mesh      = CreateProfileLoft(points, loft);
            entries.Add(new(points, loft, mesh));
            return mesh;
        }

        private static void ValidateLoftSections(IList<ShapeProfileSection> sections)
        {
            if (sections == null)
                throw new ArgumentNullException(nameof(sections));

            if (sections.Count < 2)
                throw new ArgumentException("Profile lofts require at least two sections.", nameof(sections));

            float previousZ = float.NegativeInfinity;
            foreach (ShapeProfileSection section in sections)
            {
                if (section == null)
                    throw new ArgumentException("Profile loft sections cannot contain null entries.", nameof(sections));

                if (section.Scale.X <= 0f || section.Scale.Y <= 0f)
                    throw new ArgumentOutOfRangeException(nameof(sections), "Profile loft section scales must be positive.");

                if (section.Z <= previousZ)
                    throw new ArgumentException("Profile loft sections must be ordered by increasing depth.", nameof(sections));

                previousZ = section.Z;
            }
        }

        private static int GetProfileHash(IList<ForgeVector2> profile, float depth, float bevel)
        {
            int hash = (depth.GetHashCode() * 397) ^ bevel.GetHashCode();
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

        private static int GetLoftHash(
            IList<ForgeVector2>       profile,
            IList<ShapeProfileSection> sections)
        {
            int hash = GetProfileHash(profile, 0f, 0f);
            foreach (ShapeProfileSection section in sections)
            {
                hash = (hash * 397) ^ section.Z.GetHashCode();
                hash = (hash * 397) ^ section.Scale.GetHashCode();
                hash = (hash * 397) ^ section.Offset.GetHashCode();
            }

            return hash;
        }

        private static LoftSection[] CopySections(IList<ShapeProfileSection> sections)
        {
            LoftSection[] result = new LoftSection[sections.Count];
            for (int index = 0; index < sections.Count; index++)
            {
                ShapeProfileSection section = sections[index];
                result[index] = new(
                    section.Z,
                    section.Scale.X,
                    section.Scale.Y,
                    section.Offset.X,
                    section.Offset.Y);
            }

            return result;
        }

        private static Mesh GetOrCreate(MeshKey key, Func<Mesh> create)
        {
            if (Meshes.TryGetValue(key, out Mesh mesh))
            {
                if (mesh != null)
                    return mesh;

                Meshes.Remove(key);
            }

            mesh = create();
            Meshes.Add(key, mesh);
            return mesh;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            Meshes.Clear();
            ProfileMeshes.Clear();
            LoftMeshes.Clear();
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

        private static Mesh CreateExtrudedProfile(ForgeVector2[] profile, float depth, float bevel)
        {
            ForgeVector2[] points      = EnsureCounterClockwise(profile);
            ForgeVector2[] facePoints  = bevel > 0f ? InsetProfile(points, bevel) : points;
            List<int>     triangles    = Triangulate(facePoints);
            float         halfDepth    = depth * 0.5f;
            float         frontRim     = -halfDepth + bevel;
            float         backRim      = halfDepth - bevel;
            MeshBuilder   builder      = new();

            for (int index = 0; index < triangles.Count; index += 3)
            {
                Vector3 first  = ToVector3(facePoints[triangles[index]], -halfDepth);
                Vector3 second = ToVector3(facePoints[triangles[index + 1]], -halfDepth);
                Vector3 third  = ToVector3(facePoints[triangles[index + 2]], -halfDepth);
                builder.AddTriangle(third, second, first);

                first  = ToVector3(facePoints[triangles[index]], halfDepth);
                second = ToVector3(facePoints[triangles[index + 1]], halfDepth);
                third  = ToVector3(facePoints[triangles[index + 2]], halfDepth);
                builder.AddTriangle(first, second, third);
            }

            for (int index = 0; index < points.Length; index++)
            {
                int next = (index + 1) % points.Length;
                if (bevel > 0f)
                {
                    builder.AddQuad(
                        ToVector3(facePoints[index], -halfDepth),
                        ToVector3(facePoints[next], -halfDepth),
                        ToVector3(points[next], frontRim),
                        ToVector3(points[index], frontRim));
                    builder.AddQuad(
                        ToVector3(points[index], backRim),
                        ToVector3(points[next], backRim),
                        ToVector3(facePoints[next], halfDepth),
                        ToVector3(facePoints[index], halfDepth));
                }

                builder.AddQuad(
                    ToVector3(points[index], frontRim),
                    ToVector3(points[next], frontRim),
                    ToVector3(points[next], backRim),
                    ToVector3(points[index], backRim));
            }

            return builder.Build("Low Poly Extruded Profile");
        }

        private static Mesh CreateProfileLoft(ForgeVector2[] profile, LoftSection[] sections)
        {
            ForgeVector2[] points    = EnsureCounterClockwise(profile);
            ForgeVector2[][] rings   = new ForgeVector2[sections.Length][];
            MeshBuilder    builder   = new();
            for (int sectionIndex = 0; sectionIndex < sections.Length; sectionIndex++)
            {
                LoftSection section = sections[sectionIndex];
                ForgeVector2[] ring = new ForgeVector2[points.Length];
                for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
                {
                    ForgeVector2 point = points[pointIndex];
                    ring[pointIndex] = new(
                        (point.X * section.ScaleX) + section.OffsetX,
                        (point.Y * section.ScaleY) + section.OffsetY);
                }

                rings[sectionIndex] = ring;
            }

            List<int> frontTriangles = Triangulate(rings[0]);
            List<int> backTriangles  = Triangulate(rings[rings.Length - 1]);
            AddLoftCap(builder, rings[0], sections[0].Z, frontTriangles, true);
            AddLoftCap(
                builder,
                rings[rings.Length - 1],
                sections[sections.Length - 1].Z,
                backTriangles,
                false);

            for (int sectionIndex = 0; sectionIndex < rings.Length - 1; sectionIndex++)
            {
                ForgeVector2[] front = rings[sectionIndex];
                ForgeVector2[] back  = rings[sectionIndex + 1];
                for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
                {
                    int next = (pointIndex + 1) % points.Length;
                    builder.AddQuad(
                        ToVector3(front[pointIndex], sections[sectionIndex].Z),
                        ToVector3(front[next], sections[sectionIndex].Z),
                        ToVector3(back[next], sections[sectionIndex + 1].Z),
                        ToVector3(back[pointIndex], sections[sectionIndex + 1].Z));
                }
            }

            return builder.Build("Low Poly Profile Loft");
        }

        private static void AddLoftCap(
            MeshBuilder         builder,
            IList<ForgeVector2> ring,
            float               z,
            IList<int>          triangles,
            bool                reverse)
        {
            for (int index = 0; index < triangles.Count; index += 3)
            {
                Vector3 first  = ToVector3(ring[triangles[index]], z);
                Vector3 second = ToVector3(ring[triangles[index + 1]], z);
                Vector3 third  = ToVector3(ring[triangles[index + 2]], z);
                if (reverse)
                    builder.AddTriangle(third, second, first);
                else
                    builder.AddTriangle(first, second, third);
            }
        }

        private static ForgeVector2[] InsetProfile(IList<ForgeVector2> points, float inset)
        {
            ForgeVector2[] result = new ForgeVector2[points.Count];
            for (int index = 0; index < points.Count; index++)
            {
                ForgeVector2 previous = points[(index + points.Count - 1) % points.Count];
                ForgeVector2 current  = points[index];
                ForgeVector2 next     = points[(index + 1) % points.Count];
                Vector2      before   = new(current.X - previous.X, current.Y - previous.Y);
                Vector2      after    = new(next.X - current.X, next.Y - current.Y);
                before.Normalize();
                after.Normalize();
                Vector2 beforeNormal = new(-before.y, before.x);
                Vector2 afterNormal  = new(-after.y, after.x);
                Vector2 miter        = (beforeNormal + afterNormal).normalized;
                float   denominator  = Vector2.Dot(miter, afterNormal);
                float   distance     = Mathf.Abs(denominator) > 0.0001f ? inset / denominator : inset;
                distance             = Mathf.Clamp(distance, -inset * 4f, inset * 4f);
                result[index]        = new(current.X + (miter.x * distance), current.Y + (miter.y * distance));
            }

            if (Mathf.Approximately(SignedArea(result), 0f))
                throw new ArgumentException("Profile bevel collapses the supplied outline.", nameof(points));

            return result;
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
            private readonly float bevel;

            public ProfileMeshEntry(ForgeVector2[] points, float depth, float bevel, Mesh mesh)
            {
                this.depth = depth;
                this.bevel = bevel;
                Points     = points;
                Mesh       = mesh;
            }

            public ForgeVector2[] Points { get; }

            public Mesh Mesh { get; }

            public bool Matches(IList<ForgeVector2> profile, float candidateDepth, float candidateBevel)
            {
                if (!depth.Equals(candidateDepth) ||
                    !bevel.Equals(candidateBevel) ||
                    Points.Length != profile.Count)
                    return false;

                for (int index = 0; index < Points.Length; index++)
                {
                    if (!Points[index].Equals(profile[index]))
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Owns immutable profile-loft data and its generated Unity mesh.
        /// </summary>
        private sealed class LoftMeshEntry
        {
            private readonly LoftSection[] sections;

            public LoftMeshEntry(ForgeVector2[] points, LoftSection[] sections, Mesh mesh)
            {
                Points        = points;
                this.sections = sections;
                Mesh          = mesh;
            }

            public ForgeVector2[] Points { get; }

            public Mesh Mesh { get; }

            public bool Matches(
                IList<ForgeVector2>       profile,
                IList<ShapeProfileSection> candidateSections)
            {
                if (Points.Length != profile.Count || sections.Length != candidateSections.Count)
                    return false;

                for (int index = 0; index < Points.Length; index++)
                {
                    if (!Points[index].Equals(profile[index]))
                        return false;
                }

                for (int index = 0; index < sections.Length; index++)
                {
                    if (!sections[index].Matches(candidateSections[index]))
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Stores one immutable numeric loft section without retaining mutable schema objects.
        /// </summary>
        private readonly struct LoftSection
        {
            public LoftSection(float z, float scaleX, float scaleY, float offsetX, float offsetY)
            {
                Z       = z;
                ScaleX  = scaleX;
                ScaleY  = scaleY;
                OffsetX = offsetX;
                OffsetY = offsetY;
            }

            public float Z { get; }

            public float ScaleX { get; }

            public float ScaleY { get; }

            public float OffsetX { get; }

            public float OffsetY { get; }

            public bool Matches(ShapeProfileSection section)
            {
                return section != null &&
                       Z.Equals(section.Z) &&
                       ScaleX.Equals(section.Scale.X) &&
                       ScaleY.Equals(section.Scale.Y) &&
                       OffsetX.Equals(section.Offset.X) &&
                       OffsetY.Equals(section.Offset.Y);
            }
        }
    }
}
