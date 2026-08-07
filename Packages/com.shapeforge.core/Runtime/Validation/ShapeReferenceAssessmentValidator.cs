using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Validates reference assessments before they guide asset generation.
    /// </summary>
    public sealed class ShapeReferenceAssessmentValidator
    {
        /// <summary>Returns all deterministic assessment diagnostics without throwing.</summary>
        public ShapeDiagnosticReport Analyze(ShapeReferenceAssessment assessment)
        {
            List<ShapeDiagnostic> diagnostics = new();
            if (assessment == null)
                return Report(Error("shape.assessment.required", "A reference assessment is required."));
            if (!string.Equals(assessment.Schema, ShapeReferenceAssessment.CurrentSchema, StringComparison.Ordinal))
                diagnostics.Add(Error("shape.assessment.schema.unsupported", "Unsupported reference-assessment schema.", "/schema"));
            if (string.IsNullOrWhiteSpace(assessment.Subject))
                diagnostics.Add(Error("shape.assessment.subject.required", "An assessment requires a subject.", "/subject"));
            if (!Finite(assessment.CameraAzimuth) || assessment.CameraAzimuth < -180f || assessment.CameraAzimuth > 180f)
                diagnostics.Add(Error("shape.assessment.camera.azimuth.invalid", "Camera azimuth must be between -180 and 180 degrees.", "/cameraAzimuth"));
            if (!Finite(assessment.CameraElevation) || assessment.CameraElevation < -90f || assessment.CameraElevation > 90f)
                diagnostics.Add(Error("shape.assessment.camera.elevation.invalid", "Camera elevation must be between -90 and 90 degrees.", "/cameraElevation"));
            if (!Finite(assessment.Confidence) || assessment.Confidence < 0f || assessment.Confidence > 1f)
                diagnostics.Add(Error("shape.assessment.confidence.invalid", "Confidence must be between zero and one.", "/confidence"));
            ValidateList(assessment.VisibleFeatures, "visibleFeatures", diagnostics);
            ValidateList(assessment.Uncertainties, "uncertainties", diagnostics);
            return new(diagnostics);
        }

        private static void ValidateList(IList<string> values, string property, ICollection<ShapeDiagnostic> diagnostics)
        {
            if (values == null)
            {
                diagnostics.Add(Error("shape.assessment.collection.required", $"Assessment property '{property}' requires a collection.", $"/{property}"));
                return;
            }

            HashSet<string> unique = new(StringComparer.Ordinal);
            for (int index = 0; index < values.Count; index++)
            {
                if (string.IsNullOrWhiteSpace(values[index]))
                    diagnostics.Add(Error("shape.assessment.item.invalid", $"Assessment property '{property}' contains an empty item.", $"/{property}/{index}"));
                else if (!unique.Add(values[index]))
                    diagnostics.Add(Error("shape.assessment.item.duplicate", $"Assessment property '{property}' contains duplicate '{values[index]}'.", $"/{property}/{index}"));
            }
        }

        private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

        private static ShapeDiagnostic Error(string code, string message, string path = null) =>
            new(code, ShapeDiagnosticSeverity.Error, message, path: path);

        private static ShapeDiagnosticReport Report(ShapeDiagnostic diagnostic) => new(new[] { diagnostic });
    }
}
