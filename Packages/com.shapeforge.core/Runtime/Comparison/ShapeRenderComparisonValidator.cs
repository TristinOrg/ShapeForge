using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Validates provider-neutral render comparison observations.
    /// </summary>
    public sealed class ShapeRenderComparisonValidator
    {
        /// <summary>Returns all deterministic comparison diagnostics without throwing.</summary>
        public ShapeDiagnosticReport Analyze(ShapeRenderComparison comparison)
        {
            List<ShapeDiagnostic> diagnostics = new();
            if (comparison == null)
                return Report(Error("shape.compare.required", "A render comparison is required."));
            if (!string.Equals(comparison.Schema, ShapeRenderComparison.CurrentSchema, StringComparison.Ordinal))
                diagnostics.Add(Error("shape.compare.schema.unsupported", "Unsupported render-comparison schema.", "/schema"));
            if (string.IsNullOrWhiteSpace(comparison.ReferenceId))
                diagnostics.Add(Error("shape.compare.reference.required", "A comparison requires a reference ID.", "/referenceId"));
            if (string.IsNullOrWhiteSpace(comparison.CandidateId))
                diagnostics.Add(Error("shape.compare.candidate.required", "A comparison requires a candidate ID.", "/candidateId"));
            HashSet<string> viewIds = ValidateViews(comparison.Views, diagnostics);
            ValidateDiscrepancies(comparison.Discrepancies, viewIds, diagnostics);
            return new(diagnostics);
        }

        private static HashSet<string> ValidateViews(
            IList<ShapeViewComparison> views,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            HashSet<string> viewIds = new(StringComparer.Ordinal);
            if (views == null || views.Count == 0)
            {
                diagnostics.Add(Error("shape.compare.views.required", "A comparison requires at least one view.", "/views"));
                return viewIds;
            }

            for (int index = 0; index < views.Count; index++)
            {
                ShapeViewComparison view = views[index];
                string path = $"/views/{index}";
                if (view == null)
                {
                    diagnostics.Add(Error("shape.compare.view.required", "Comparison views cannot be null.", path));
                    continue;
                }
                if (string.IsNullOrWhiteSpace(view.ViewId))
                    diagnostics.Add(Error("shape.compare.view.id.required", "Every comparison view requires a stable ID.", $"{path}/viewId"));
                else if (!viewIds.Add(view.ViewId))
                    diagnostics.Add(Error("shape.compare.view.id.duplicate", $"Duplicate comparison view ID '{view.ViewId}'.", $"{path}/viewId"));
                if (!Finite(view.Weight) || view.Weight <= 0f)
                    diagnostics.Add(Error("shape.compare.view.weight.invalid", $"View '{view.ViewId}' weight must be finite and positive.", $"{path}/weight"));
                if (!Normalized(view.Confidence))
                    diagnostics.Add(Error("shape.compare.view.confidence.invalid", $"View '{view.ViewId}' confidence must be between zero and one.", $"{path}/confidence"));
                ValidateScores(view.Scores, path, diagnostics);
            }
            return viewIds;
        }

        private static void ValidateScores(
            ShapeComparisonScores scores,
            string                viewPath,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            if (scores == null)
            {
                diagnostics.Add(Error("shape.compare.scores.required", "Every comparison view requires scores.", $"{viewPath}/scores"));
                return;
            }
            ValidateScore(scores.Silhouette, "silhouette", viewPath, diagnostics);
            ValidateScore(scores.Proportion, "proportion", viewPath, diagnostics);
            ValidateScore(scores.Color, "color", viewPath, diagnostics);
            ValidateScore(scores.Detail, "detail", viewPath, diagnostics);
        }

        private static void ValidateScore(
            float value,
            string property,
            string viewPath,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            if (!Normalized(value))
                diagnostics.Add(Error("shape.compare.score.invalid", $"Comparison score '{property}' must be between zero and one.", $"{viewPath}/scores/{property}"));
        }

        private static void ValidateDiscrepancies(
            IList<ShapeVisualDiscrepancy> discrepancies,
            ISet<string>                  viewIds,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            if (discrepancies == null)
            {
                diagnostics.Add(Error("shape.compare.discrepancies.required", "A comparison requires a discrepancy collection.", "/discrepancies"));
                return;
            }

            HashSet<string> ids = new(StringComparer.Ordinal);
            for (int index = 0; index < discrepancies.Count; index++)
            {
                ShapeVisualDiscrepancy discrepancy = discrepancies[index];
                string path = $"/discrepancies/{index}";
                if (discrepancy == null)
                {
                    diagnostics.Add(Error("shape.compare.discrepancy.required", "Comparison discrepancies cannot be null.", path));
                    continue;
                }
                if (string.IsNullOrWhiteSpace(discrepancy.Id))
                    diagnostics.Add(Error("shape.compare.discrepancy.id.required", "Every discrepancy requires a stable ID.", $"{path}/id"));
                else if (!ids.Add(discrepancy.Id))
                    diagnostics.Add(Error("shape.compare.discrepancy.id.duplicate", $"Duplicate discrepancy ID '{discrepancy.Id}'.", $"{path}/id"));
                if (!string.IsNullOrWhiteSpace(discrepancy.ViewId) && !viewIds.Contains(discrepancy.ViewId))
                    diagnostics.Add(Error("shape.compare.discrepancy.view.unknown", $"Discrepancy '{discrepancy.Id}' targets unknown view '{discrepancy.ViewId}'.", $"{path}/viewId"));
                if (!Enum.IsDefined(typeof(ShapeVisualDiscrepancySeverity), discrepancy.Severity))
                    diagnostics.Add(Error("shape.compare.discrepancy.severity.invalid", $"Discrepancy '{discrepancy.Id}' has unsupported severity.", $"{path}/severity"));
                if (string.IsNullOrWhiteSpace(discrepancy.Message))
                    diagnostics.Add(Error("shape.compare.discrepancy.message.required", $"Discrepancy '{discrepancy.Id}' requires a message.", $"{path}/message"));
            }
        }

        private static bool Normalized(float value) => Finite(value) && value >= 0f && value <= 1f;

        private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

        private static ShapeDiagnostic Error(string code, string message, string path = null) =>
            new(code, ShapeDiagnosticSeverity.Error, message, path: path);

        private static ShapeDiagnosticReport Report(ShapeDiagnostic diagnostic) => new(new[] { diagnostic });
    }
}
