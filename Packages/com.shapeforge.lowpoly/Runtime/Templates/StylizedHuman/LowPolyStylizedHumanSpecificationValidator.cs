using System;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Validates semantic stylized-human inputs before geometry compilation.
    /// </summary>
    public sealed class LowPolyStylizedHumanSpecificationValidator
    {
        /// <summary>Validates schema identity, required groups, and bounded normalized controls.</summary>
        public void Validate(LowPolyStylizedHumanSpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            if (!string.Equals(
                    specification.Schema,
                    LowPolyStylizedHumanSpecification.CurrentSchema,
                    StringComparison.Ordinal))
                throw new ShapeValidationException(
                    $"Unsupported stylized-human schema '{specification.Schema}'.");

            if (string.IsNullOrWhiteSpace(specification.Name))
                throw new ShapeValidationException("A stylized human requires a model name.");

            if (string.IsNullOrWhiteSpace(specification.Style))
                throw new ShapeValidationException("A stylized human requires a style ID.");

            if (specification.Proportions == null || specification.Head == null ||
                specification.Face == null || specification.Hair == null)
                throw new ShapeValidationException("A stylized human requires proportions, head, face, and hair data.");

            ValidateRange(specification.OverallScale, 0.5f, 2f, "overallScale");
            ValidateRange(specification.Proportions.HeadScale, 0.7f, 1.4f, "proportions.headScale");
            ValidateRange(specification.Proportions.ShoulderWidth, 0.7f, 1.4f, "proportions.shoulderWidth");
            ValidateRange(specification.Proportions.BodyWidth, 0.7f, 1.4f, "proportions.bodyWidth");
            ValidateRange(specification.Proportions.LegLength, 0.7f, 1.4f, "proportions.legLength");
            ValidateRange(specification.Head.Width, 0.75f, 1.3f, "head.width");
            ValidateRange(specification.Head.Height, 0.75f, 1.3f, "head.height");
            ValidateRange(specification.Head.Depth, 0.75f, 1.3f, "head.depth");
            ValidateRange(specification.Head.JawWidth, 0.7f, 1.25f, "head.jawWidth");
            ValidateRange(specification.Face.EyeScale, 0.6f, 1.5f, "face.eyeScale");
            ValidateRange(specification.Face.EyeSpacing, 0.7f, 1.3f, "face.eyeSpacing");
            ValidateRange(specification.Face.EyeOpenness, 0.35f, 1.4f, "face.eyeOpenness");
            ValidateRange(specification.Face.MouthWidth, 0.5f, 1.5f, "face.mouthWidth");
            ValidateRange(specification.Hair.Volume, 0.8f, 1.3f, "hair.volume");
            ValidateRange(specification.Hair.Parting, 0.15f, 0.85f, "hair.parting");
            ValidateRange(specification.Hair.FringeLength, 0f, 1f, "hair.fringeLength");
            ValidateRange(specification.Hair.SideburnLength, 0f, 1f, "hair.sideburnLength");
        }

        private static void ValidateRange(float value, float minimum, float maximum, string path)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < minimum || value > maximum)
                throw new ShapeValidationException(
                    $"Stylized-human value '{path}' must be from {minimum} to {maximum}.");
        }
    }
}
