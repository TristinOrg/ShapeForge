using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Validates bounded category-neutral reference blueprints without interpreting asset semantics.
    /// </summary>
    public sealed class ShapeReferenceBlueprintValidator
    {
        /// <summary>Returns deterministic blueprint diagnostics.</summary>
        public ShapeDiagnosticReport Analyze(ShapeReferenceBlueprint blueprint)
        {
            List<ShapeDiagnostic> diagnostics = new();
            if (blueprint == null)
                return new(new[] { Error("shape.reference.blueprint.required", "A reference blueprint is required.", "/") });
            if (blueprint.Schema != ShapeReferenceBlueprint.CurrentSchema)
                diagnostics.Add(Error("shape.reference.blueprint.schema", "Unsupported reference-blueprint schema.", "/schema"));
            if (string.IsNullOrWhiteSpace(blueprint.Id))
                diagnostics.Add(Error("shape.reference.blueprint.id", "A reference blueprint requires a stable ID.", "/id"));
            if (blueprint.Views == null || blueprint.Views.Count == 0 || blueprint.Views.Count > 16)
                diagnostics.Add(Error("shape.reference.blueprint.views", "A blueprint requires between 1 and 16 views.", "/views"));
            else
                ValidateViews(blueprint.Views, diagnostics);
            ValidateEvidence(blueprint.EvidenceRegions, diagnostics);
            ValidatePalette(blueprint.Palette, diagnostics);
            ValidateConfidence(blueprint.Classification?.Confidence ?? float.NaN,
                "/classification/confidence", diagnostics);
            ValidateReviewQueue(blueprint.ReviewQueue, diagnostics);
            return new(diagnostics);
        }

        private static void ValidateEvidence(IList<ShapeReferenceEvidenceRegion> regions,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            if (regions == null || regions.Count > 256)
            {
                diagnostics.Add(Error("shape.reference.blueprint.evidence.count", "Evidence supports at most 256 regions.", "/evidenceRegions"));
                return;
            }
            HashSet<string> ids = new(StringComparer.Ordinal);
            for (int index = 0; index < regions.Count; index++)
            {
                ShapeReferenceEvidenceRegion region = regions[index];
                string path = $"/evidenceRegions/{index}";
                if (region == null || string.IsNullOrWhiteSpace(region.Id) || !ids.Add(region.Id) ||
                    string.IsNullOrWhiteSpace(region.Kind) || string.IsNullOrWhiteSpace(region.ImagePath))
                    diagnostics.Add(Error("shape.reference.blueprint.evidence.item", "Evidence requires a unique ID, kind, and image path.", path));
                else
                {
                    ValidateBounds(region.Bounds, $"{path}/bounds", diagnostics);
                    ValidateConfidence(region.Confidence, $"{path}/confidence", diagnostics);
                }
            }
        }

        private static void ValidatePalette(IList<ShapeReferencePaletteSample> palette,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            if (palette == null || palette.Count > 256)
            {
                diagnostics.Add(Error("shape.reference.blueprint.palette.count", "Palettes support at most 256 samples.", "/palette"));
                return;
            }
            for (int index = 0; index < palette.Count; index++)
            {
                ShapeReferencePaletteSample sample = palette[index];
                if (sample == null || string.IsNullOrWhiteSpace(sample.Hex) ||
                    !System.Text.RegularExpressions.Regex.IsMatch(sample.Hex, "^#[0-9A-Fa-f]{6}$"))
                    diagnostics.Add(Error("shape.reference.blueprint.palette.hex", "Palette colors require #RRGGBB values.", $"/palette/{index}/hex"));
                else
                    ValidateConfidence(sample.Confidence, $"/palette/{index}/confidence", diagnostics);
            }
        }

        private static void ValidateViews(IList<ShapeReferenceBlueprintView> views,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            HashSet<string> ids = new(StringComparer.Ordinal);
            for (int index = 0; index < views.Count; index++)
            {
                ShapeReferenceBlueprintView view = views[index];
                string path = $"/views/{index}";
                if (view == null)
                {
                    diagnostics.Add(Error("shape.reference.blueprint.view.required", "Blueprint views cannot be null.", path));
                    continue;
                }
                if (string.IsNullOrWhiteSpace(view.ViewId) || !ids.Add(view.ViewId))
                    diagnostics.Add(Error("shape.reference.blueprint.view.id", "Every view requires a unique ID.", $"{path}/viewId"));
                if (string.IsNullOrWhiteSpace(view.ImagePath))
                    diagnostics.Add(Error("shape.reference.blueprint.view.image", "Every view requires an image path.", $"{path}/imagePath"));
                ValidateBounds(view.ForegroundBounds, $"{path}/foregroundBounds", diagnostics);
                if (view.Silhouette == null || view.Silhouette.Count < 3 || view.Silhouette.Count > 512)
                    diagnostics.Add(Error("shape.reference.blueprint.view.silhouette", "Silhouettes require 3 to 512 points.", $"{path}/silhouette"));
                else
                    ValidatePoints(view.Silhouette, $"{path}/silhouette", diagnostics);
                ValidateConfidence(view.Confidence, $"{path}/confidence", diagnostics);
            }
        }

        private static void ValidateBounds(ShapeReferenceBounds bounds, string path,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            if (bounds == null || !Normalized(bounds.X) || !Normalized(bounds.Y) ||
                !PositiveNormalized(bounds.Width) || !PositiveNormalized(bounds.Height) ||
                bounds.X + bounds.Width > 1.0001f || bounds.Y + bounds.Height > 1.0001f)
                diagnostics.Add(Error("shape.reference.blueprint.view.bounds", "Foreground bounds must fit normalized image space.", path));
        }

        private static void ValidatePoints(IList<ForgeVector2> points, string path,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            for (int index = 0; index < points.Count; index++)
            {
                if (!Normalized(points[index].X) || !Normalized(points[index].Y))
                    diagnostics.Add(Error("shape.reference.blueprint.view.point", "Silhouette points must be normalized.", $"{path}/{index}"));
            }
        }

        private static void ValidateReviewQueue(IList<ShapeReferenceReviewItem> items,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            if (items == null || items.Count > 128)
            {
                diagnostics.Add(Error("shape.reference.blueprint.review.count", "Review queues support at most 128 items.", "/reviewQueue"));
                return;
            }
            for (int index = 0; index < items.Count; index++)
            {
                if (items[index] == null || string.IsNullOrWhiteSpace(items[index].Kind) ||
                    string.IsNullOrWhiteSpace(items[index].Reason))
                    diagnostics.Add(Error("shape.reference.blueprint.review.item", "Review items require kind and reason.", $"/reviewQueue/{index}"));
            }
        }

        private static void ValidateConfidence(float value, string path,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            if (!Normalized(value))
                diagnostics.Add(Error("shape.reference.blueprint.confidence", "Confidence must be finite and normalized.", path));
        }

        private static bool Normalized(float value) =>
            !float.IsNaN(value) && !float.IsInfinity(value) && value >= 0f && value <= 1f;

        private static bool PositiveNormalized(float value) => Normalized(value) && value > 0f;

        private static ShapeDiagnostic Error(string code, string message, string path) =>
            new(code, ShapeDiagnosticSeverity.Error, message, path: path);
    }
}
