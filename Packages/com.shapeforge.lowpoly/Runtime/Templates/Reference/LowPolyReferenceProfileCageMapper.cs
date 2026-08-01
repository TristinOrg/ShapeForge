using System;
using System.Collections.Generic;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Reconstructs deterministic Low Poly profile cages from aligned multi-view part silhouettes.
    /// </summary>
    public sealed class LowPolyReferenceProfileCageMapper
    {
        private const int MinimumPointCount   = 4;
        private const int MaximumPointCount   = 64;
        private const int MinimumSectionCount = 3;
        private const int MaximumSectionCount = 16;
        private const float SliceInset        = 0.0001f;
        private const float MinimumScale      = 0.01f;

        private readonly ShapeReferenceDefinitionValidator validator = new();

        /// <summary>
        /// Maps one semantic part into normalized profile-cage geometry.
        /// Side image-left maps to negative depth and image-right maps to positive depth.
        /// </summary>
        public LowPolyReferenceProfileCage Map(
            ShapeReferenceDefinition reference,
            string                   partId,
            int                      profilePointCount = 16,
            int                      depthSectionCount = 7)
        {
            validator.Validate(reference);
            if (string.IsNullOrWhiteSpace(partId))
                throw new ArgumentException("A reference part ID cannot be empty.", nameof(partId));

            if (profilePointCount < MinimumPointCount || profilePointCount > MaximumPointCount)
                throw new ArgumentOutOfRangeException(nameof(profilePointCount));

            if (depthSectionCount < MinimumSectionCount || depthSectionCount > MaximumSectionCount)
                throw new ArgumentOutOfRangeException(nameof(depthSectionCount));

            ShapeReferencePart part = FindPart(reference, partId);
            RequireSilhouette(part.Front, partId, "front");
            RequireSilhouette(part.Side, partId, "side");
            if (part.Back != null)
                RequireSilhouette(part.Back, partId, "back");

            ForgeVector2[] front = Resample(part.Front, profilePointCount, false);
            ForgeVector2[] back  = part.Back == null
                ? front
                : Resample(part.Back, profilePointCount, true);
            ShapeProfileCageSection[] sections = CreateSections(
                front,
                back,
                part.Side,
                depthSectionCount);

            float frontCenterX = (part.Front.Minimum.X + part.Front.Maximum.X) * 0.5f;
            float frontCenterY = (part.Front.Minimum.Y + part.Front.Maximum.Y) * 0.5f;
            float sideCenterX  = (part.Side.Minimum.X + part.Side.Maximum.X) * 0.5f;
            return new LowPolyReferenceProfileCage(
                new(frontCenterX - 0.5f, frontCenterY, sideCenterX - 0.5f),
                new(
                    part.Front.Maximum.X - part.Front.Minimum.X,
                    part.Front.Maximum.Y - part.Front.Minimum.Y,
                    part.Side.Maximum.X - part.Side.Minimum.X),
                sections);
        }

        private static ShapeReferencePart FindPart(ShapeReferenceDefinition reference, string partId)
        {
            foreach (ShapeReferencePart part in reference.Parts)
            {
                if (string.Equals(part.Id, partId, StringComparison.Ordinal))
                    return part;
            }

            throw new ShapeValidationException($"Reference part '{partId}' was not found.");
        }

        private static void RequireSilhouette(
            ShapeReferenceViewObservation view,
            string                        partId,
            string                        viewName)
        {
            if (view == null || view.Silhouette.Count < 3)
                throw new ShapeValidationException(
                    $"Reference part '{partId}' requires a measured {viewName} silhouette for profile-cage mapping.");
        }

        private static ForgeVector2[] Resample(
            ShapeReferenceViewObservation view,
            int                           pointCount,
            bool                          mirrorX)
        {
            IList<ForgeVector2> source      = view.Silhouette;
            float[]              cumulative = new float[source.Count + 1];
            for (int index = 0; index < source.Count; index++)
                cumulative[index + 1] = cumulative[index] + Distance(source[index], source[(index + 1) % source.Count]);

            float          perimeter = cumulative[source.Count];
            if (perimeter <= 0f)
                throw new ShapeValidationException("A reference silhouette requires a non-zero perimeter.");

            ForgeVector2[] result    = new ForgeVector2[pointCount];
            for (int index = 0; index < pointCount; index++)
            {
                float distance = perimeter * index / pointCount;
                int   segment  = FindSegment(cumulative, distance);
                float length   = cumulative[segment + 1] - cumulative[segment];
                float amount   = length <= 0f ? 0f : (distance - cumulative[segment]) / length;
                ForgeVector2 point = Lerp(source[segment], source[(segment + 1) % source.Count], amount);
                float normalizedX = (point.X - view.Minimum.X) / (view.Maximum.X - view.Minimum.X) - 0.5f;
                float normalizedY = (point.Y - view.Minimum.Y) / (view.Maximum.Y - view.Minimum.Y) - 0.5f;
                result[index] = new(mirrorX ? -normalizedX : normalizedX, normalizedY);
            }

            return result;
        }

        private static ShapeProfileCageSection[] CreateSections(
            ForgeVector2[]                       front,
            ForgeVector2[]                       back,
            ShapeReferenceViewObservation        side,
            int                                  sectionCount)
        {
            ShapeProfileCageSection[] sections = new ShapeProfileCageSection[sectionCount];
            float sideHeight = side.Maximum.Y - side.Minimum.Y;
            float sideCenter = (side.Minimum.Y + side.Maximum.Y) * 0.5f;
            for (int sectionIndex = 0; sectionIndex < sectionCount; sectionIndex++)
            {
                float depth     = sectionIndex / (sectionCount - 1f);
                float sample    = Lerp(side.Minimum.X + SliceInset, side.Maximum.X - SliceInset, depth);
                GetVerticalSlice(side.Silhouette, sample, out float minimumY, out float maximumY);
                float scaleY    = Math.Max((maximumY - minimumY) / sideHeight, MinimumScale);
                float offsetY   = ((minimumY + maximumY) * 0.5f - sideCenter) / sideHeight;
                float scaleX    = (float)Math.Sqrt(scaleY);
                ForgeVector2[] profile = new ForgeVector2[front.Length];
                for (int pointIndex = 0; pointIndex < profile.Length; pointIndex++)
                {
                    ForgeVector2 source = Lerp(back[pointIndex], front[pointIndex], depth);
                    profile[pointIndex] = new(source.X * scaleX, source.Y * scaleY + offsetY);
                }

                sections[sectionIndex] = new(depth - 0.5f, profile);
            }

            return sections;
        }

        private static void GetVerticalSlice(
            IList<ForgeVector2> silhouette,
            float               x,
            out float           minimumY,
            out float           maximumY)
        {
            minimumY = float.MaxValue;
            maximumY = float.MinValue;
            for (int index = 0; index < silhouette.Count; index++)
            {
                ForgeVector2 start = silhouette[index];
                ForgeVector2 end   = silhouette[(index + 1) % silhouette.Count];
                if ((start.X > x) == (end.X > x))
                    continue;

                float amount = (x - start.X) / (end.X - start.X);
                float y      = Lerp(start.Y, end.Y, amount);
                minimumY     = Math.Min(minimumY, y);
                maximumY     = Math.Max(maximumY, y);
            }

            if (minimumY == float.MaxValue)
                throw new ShapeValidationException("A side silhouette could not produce a closed vertical slice.");
        }

        private static int FindSegment(float[] cumulative, float distance)
        {
            int low  = 0;
            int high = cumulative.Length - 2;
            while (low < high)
            {
                int middle = (low + high) / 2;
                if (cumulative[middle + 1] <= distance)
                    low = middle + 1;
                else
                    high = middle;
            }

            return low;
        }

        private static float Distance(ForgeVector2 left, ForgeVector2 right)
        {
            float x = right.X - left.X;
            float y = right.Y - left.Y;
            return (float)Math.Sqrt((x * x) + (y * y));
        }

        private static ForgeVector2 Lerp(ForgeVector2 left, ForgeVector2 right, float amount)
        {
            return new(Lerp(left.X, right.X, amount), Lerp(left.Y, right.Y, amount));
        }

        private static float Lerp(float left, float right, float amount)
        {
            return left + ((right - left) * amount);
        }
    }
}
