using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Validates bounded engine-neutral render-capture requests.
    /// </summary>
    public sealed class ShapeRenderCaptureRequestValidator
    {
        /// <summary>Returns deterministic request diagnostics.</summary>
        public ShapeDiagnosticReport Analyze(ShapeRenderCaptureRequest request)
        {
            List<ShapeDiagnostic> diagnostics = new();
            if (request == null)
                return Report(Error("shape.capture.required", "A render-capture request is required.", "/"));
            if (request.Schema != ShapeRenderCaptureRequest.CurrentSchema)
                diagnostics.Add(Error("shape.capture.schema.unsupported", "Unsupported render-capture schema.", "/schema"));
            if (string.IsNullOrWhiteSpace(request.Id))
                diagnostics.Add(Error("shape.capture.id.required", "A render capture requires a stable ID.", "/id"));
            if (string.IsNullOrWhiteSpace(request.CandidateId))
                diagnostics.Add(Error("shape.capture.candidate.required", "A render capture requires a candidate ID.", "/candidateId"));
            if (request.Width < 64 || request.Width > 4096)
                diagnostics.Add(Error("shape.capture.width.invalid", "Capture width must be between 64 and 4096.", "/width"));
            if (request.Height < 64 || request.Height > 4096)
                diagnostics.Add(Error("shape.capture.height.invalid", "Capture height must be between 64 and 4096.", "/height"));
            if (request.Views == null || request.Views.Count == 0 || request.Views.Count > 16)
                diagnostics.Add(Error("shape.capture.views.invalid", "A capture requires between 1 and 16 views.", "/views"));
            else
                ValidateViews(request.Views, diagnostics);
            return new(diagnostics);
        }

        private static void ValidateViews(
            IList<ShapeRenderCaptureView> views,
            ICollection<ShapeDiagnostic> diagnostics)
        {
            HashSet<string> ids = new(StringComparer.Ordinal);
            for (int index = 0; index < views.Count; index++)
            {
                ShapeRenderCaptureView view = views[index];
                string path = $"/views/{index}";
                if (view == null)
                {
                    diagnostics.Add(Error("shape.capture.view.required", "Capture views cannot be null.", path));
                    continue;
                }
                if (string.IsNullOrWhiteSpace(view.Id))
                    diagnostics.Add(Error("shape.capture.view.id.required", "Every capture view requires an ID.", $"{path}/id"));
                else if (!ids.Add(view.Id))
                    diagnostics.Add(Error("shape.capture.view.id.duplicate", $"Duplicate capture view '{view.Id}'.", $"{path}/id"));
                if (!Finite(view.Azimuth) || view.Azimuth < -180f || view.Azimuth > 180f)
                    diagnostics.Add(Error("shape.capture.view.azimuth.invalid", "View azimuth must be between -180 and 180.", $"{path}/azimuth"));
                if (!Finite(view.Elevation) || view.Elevation < -89f || view.Elevation > 89f)
                    diagnostics.Add(Error("shape.capture.view.elevation.invalid", "View elevation must be between -89 and 89.", $"{path}/elevation"));
                if (!Finite(view.FramingScale) || view.FramingScale < 1f || view.FramingScale > 3f)
                    diagnostics.Add(Error("shape.capture.view.framing.invalid", "View framing scale must be between 1 and 3.", $"{path}/framingScale"));
            }
        }

        private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        private static ShapeDiagnostic Error(string code, string message, string path) =>
            new(code, ShapeDiagnosticSeverity.Error, message, path: path);
        private static ShapeDiagnosticReport Report(ShapeDiagnostic diagnostic) => new(new[] { diagnostic });
    }
}
