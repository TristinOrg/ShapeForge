using System;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Builds an AI-provider-neutral prompt for extracting normalized reference measurements.
    /// </summary>
    public static class LowPolyStylizedHumanReferencePrompt
    {
        /// <summary>Gets the versioned extraction protocol shared by external vision models.</summary>
        public const string Protocol =
            "You are measuring a stylized human reference for ShapeForge. " +
            "Use the front image for image-plane silhouette and proportion measurements. " +
            "Measure widths at their widest visible points and divide figure measurements by the full " +
            "top-of-hair to bottom-of-feet height. Exclude weapons, loose accessories, shadows, and perspective " +
            "padding. Measure jaw width relative to head width, hair width relative to head width, fringe from " +
            "hairline to jaw, and sideburn from temple to jaw. Parting uses image-left 0 and image-right 1. " +
            "Only emit side measurements when a distinct side image is supplied; never infer depth from the front " +
            "image. If the full figure or required front landmarks are occluded, request a clearer image instead " +
            "of fabricating values. Return one JSON object conforming exactly to the supplied Schema, with no " +
            "Markdown or explanation.";

        /// <summary>Creates a complete extraction prompt containing the authoritative JSON Schema.</summary>
        public static string Create(string jsonSchema)
        {
            if (string.IsNullOrWhiteSpace(jsonSchema))
                throw new ArgumentException("Reference JSON Schema cannot be empty.", nameof(jsonSchema));

            return $"{Protocol}\n\nJSON Schema:\n{jsonSchema}";
        }
    }
}
