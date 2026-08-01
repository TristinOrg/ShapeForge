using System;

namespace ShapeForge
{
    /// <summary>
    /// Builds a provider-neutral prompt for extracting structured multi-view part observations.
    /// </summary>
    public static class ShapeReferenceExtractionPrompt
    {
        /// <summary>Creates extraction instructions ending with the authoritative JSON Schema.</summary>
        public static string Create(string jsonSchema)
        {
            if (string.IsNullOrWhiteSpace(jsonSchema))
                throw new ArgumentException("Reference JSON Schema cannot be empty.", nameof(jsonSchema));

            return "Extract only directly visible geometry from the supplied aligned reference views. " +
                   "Use stable semantic part IDs across front, side, and back. Coordinates are normalized " +
                   "to the complete image: x increases left-to-right and y increases bottom-to-top. " +
                   "The side view must face image-left, which maps to negative model depth. Bounds must tightly " +
                   "enclose each visible part. Silhouettes must be clockwise, start at " +
                   "the topmost point, and contain only meaningful contour changes. Omit a view when it is " +
                   "missing or occluded; never infer unseen depth or a hidden silhouette. Confidence describes " +
                   "the observation, not artistic certainty. Return JSON only, conforming exactly to this schema:\n" +
                   jsonSchema;
        }
    }
}
