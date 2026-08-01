using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Validates normalized multi-view reference observations before geometry mapping.
    /// </summary>
    public sealed class ShapeReferenceDefinitionValidator
    {
        /// <summary>Validates schema identity, part identity, bounds, confidence, and silhouettes.</summary>
        public void Validate(ShapeReferenceDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (!string.Equals(definition.Schema, ShapeReferenceDefinition.CurrentSchema, StringComparison.Ordinal))
                throw new ShapeValidationException($"Unsupported reference schema '{definition.Schema}'.");

            if (string.IsNullOrWhiteSpace(definition.Name))
                throw new ShapeValidationException("A reference definition requires a name.");

            if (definition.Parts == null || definition.Parts.Count == 0)
                throw new ShapeValidationException("A reference definition requires at least one semantic part.");

            HashSet<string> partIds = new(StringComparer.Ordinal);
            foreach (ShapeReferencePart part in definition.Parts)
                ValidatePart(part, partIds);
        }

        private static void ValidatePart(ShapeReferencePart part, HashSet<string> partIds)
        {
            if (part == null || string.IsNullOrWhiteSpace(part.Id))
                throw new ShapeValidationException("Every reference part requires a stable ID.");

            if (!partIds.Add(part.Id))
                throw new ShapeValidationException($"Duplicate reference part ID '{part.Id}'.");

            if (part.Front == null && part.Side == null && part.Back == null)
                throw new ShapeValidationException($"Reference part '{part.Id}' requires at least one view.");

            ValidateView(part.Front, part.Id, "front");
            ValidateView(part.Side, part.Id, "side");
            ValidateView(part.Back, part.Id, "back");
        }

        private static void ValidateView(ShapeReferenceViewObservation view, string partId, string viewName)
        {
            if (view == null)
                return;

            ValidatePoint(view.Minimum, partId, viewName);
            ValidatePoint(view.Maximum, partId, viewName);
            if (view.Minimum.X >= view.Maximum.X || view.Minimum.Y >= view.Maximum.Y)
                throw new ShapeValidationException(
                    $"Reference part '{partId}' {viewName} bounds require minimum values below maximum values.");

            if (!IsUnitValue(view.Confidence))
                throw new ShapeValidationException(
                    $"Reference part '{partId}' {viewName} confidence must be from zero to one.");

            if (view.Silhouette == null)
                throw new ShapeValidationException(
                    $"Reference part '{partId}' {viewName} requires a silhouette collection.");

            if (view.Silhouette.Count > 0 && view.Silhouette.Count < 3)
                throw new ShapeValidationException(
                    $"Reference part '{partId}' {viewName} silhouette requires at least three points.");

            foreach (ForgeVector2 point in view.Silhouette)
            {
                ValidatePoint(point, partId, viewName);
                if (point.X < view.Minimum.X || point.X > view.Maximum.X ||
                    point.Y < view.Minimum.Y || point.Y > view.Maximum.Y)
                    throw new ShapeValidationException(
                        $"Reference part '{partId}' {viewName} silhouette must remain inside its bounds.");
            }
        }

        private static void ValidatePoint(ForgeVector2 point, string partId, string viewName)
        {
            if (!IsUnitValue(point.X) || !IsUnitValue(point.Y))
                throw new ShapeValidationException(
                    $"Reference part '{partId}' {viewName} coordinates must be from zero to one.");
        }

        private static bool IsUnitValue(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value >= 0f && value <= 1f;
        }
    }
}
