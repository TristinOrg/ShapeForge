using System;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Validates normalized reference observations before semantic mapping.
    /// </summary>
    public sealed class LowPolyStylizedHumanReferenceSpecificationValidator
    {
        /// <summary>Validates schema identity, required views, and supported measurement ranges.</summary>
        public void Validate(LowPolyStylizedHumanReferenceSpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            if (!string.Equals(
                    specification.Schema,
                    LowPolyStylizedHumanReferenceSpecification.CurrentSchema,
                    StringComparison.Ordinal))
                throw new ShapeValidationException(
                    $"Unsupported stylized-human reference schema '{specification.Schema}'.");

            if (specification.Front == null)
                throw new ShapeValidationException("A stylized-human reference requires front-view measurements.");

            LowPolyStylizedHumanFrontReference front = specification.Front;
            ValidateRange(front.HeadWidth, 0.18f, 0.312f, "front.headWidth");
            ValidateRange(front.HeadHeight, 0.195f, 0.338f, "front.headHeight");
            ValidateRange(front.ShoulderWidth, 0.238f, 0.476f, "front.shoulderWidth");
            ValidateRange(front.BodyWidth, 0.119f, 0.238f, "front.bodyWidth");
            ValidateRange(front.LegLength, 0.343f, 0.686f, "front.legLength");
            ValidateRange(front.JawWidthToHeadWidth, 0.546f, 0.975f, "front.jawWidthToHeadWidth");
            ValidateRange(front.HairWidthToHeadWidth, 0.92f, 1.495f, "front.hairWidthToHeadWidth");
            ValidateRange(front.Parting, 0.15f, 0.85f, "front.parting");
            ValidateRange(front.FringeLength, 0f, 1f, "front.fringeLength");
            ValidateRange(front.SideburnLength, 0f, 1f, "front.sideburnLength");

            if (specification.Side == null)
                return;

            ValidateRange(specification.Side.HeadDepth, 0.1575f, 0.273f, "side.headDepth");
            ValidateRange(specification.Side.HairDepthToHeadDepth, 0.864f, 1.404f, "side.hairDepthToHeadDepth");
        }

        private static void ValidateRange(float value, float minimum, float maximum, string path)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < minimum || value > maximum)
                throw new ShapeValidationException(
                    $"Stylized-human reference value '{path}' must be from {minimum} to {maximum}.");
        }
    }
}
