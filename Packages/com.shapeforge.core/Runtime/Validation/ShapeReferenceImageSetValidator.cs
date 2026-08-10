using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Validates bounded named reference-image manifests without accessing the filesystem.
    /// </summary>
    public sealed class ShapeReferenceImageSetValidator
    {
        /// <summary>Returns deterministic manifest diagnostics.</summary>
        public ShapeDiagnosticReport Analyze(ShapeReferenceImageSet reference)
        {
            List<ShapeDiagnostic> diagnostics = new();
            if (reference == null)
                return new(new[] { Error("shape.reference.images.required", "A reference-image set is required.", "/") });
            if (reference.Schema != ShapeReferenceImageSet.CurrentSchema)
                diagnostics.Add(Error("shape.reference.images.schema", "Unsupported reference-image schema.", "/schema"));
            if (string.IsNullOrWhiteSpace(reference.Id))
                diagnostics.Add(Error("shape.reference.images.id", "A reference-image set requires a stable ID.", "/id"));
            if (reference.Images == null || reference.Images.Count == 0 || reference.Images.Count > 16)
                diagnostics.Add(Error("shape.reference.images.count", "A reference-image set requires between 1 and 16 images.", "/images"));
            else
                ValidateImages(reference.Images, diagnostics);
            return new(diagnostics);
        }

        private static void ValidateImages(
            IList<ShapeReferenceImage> images,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            HashSet<string> views = new(StringComparer.Ordinal);
            for (int index = 0; index < images.Count; index++)
            {
                ShapeReferenceImage image = images[index];
                string path = $"/images/{index}";
                if (image == null)
                {
                    diagnostics.Add(Error("shape.reference.image.required", "Reference images cannot be null.", path));
                    continue;
                }
                if (string.IsNullOrWhiteSpace(image.ViewId))
                    diagnostics.Add(Error("shape.reference.image.view", "Every reference image requires a view ID.", $"{path}/viewId"));
                else if (!views.Add(image.ViewId))
                    diagnostics.Add(Error("shape.reference.image.view.duplicate", $"Duplicate reference view '{image.ViewId}'.", $"{path}/viewId"));
                if (string.IsNullOrWhiteSpace(image.ImagePath))
                    diagnostics.Add(Error("shape.reference.image.path", "Every reference image requires an image path.", $"{path}/imagePath"));
                if (float.IsNaN(image.Weight) || float.IsInfinity(image.Weight) || image.Weight <= 0f)
                    diagnostics.Add(Error("shape.reference.image.weight", "Reference image weight must be finite and positive.", $"{path}/weight"));
            }
        }

        private static ShapeDiagnostic Error(string code, string message, string path) =>
            new(code, ShapeDiagnosticSeverity.Error, message, path: path);
    }
}
