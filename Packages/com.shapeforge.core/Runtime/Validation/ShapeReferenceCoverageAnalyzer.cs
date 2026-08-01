using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Analyzes deterministic coverage and alignment quality for validated multi-view references.
    /// </summary>
    public sealed class ShapeReferenceCoverageAnalyzer
    {
        private readonly ShapeReferenceDefinitionValidator validator = new();

        /// <summary>Analyzes view coverage and normalized height disagreement.</summary>
        public ShapeReferenceCoverageReport Analyze(ShapeReferenceDefinition definition, float heightTolerance = 0.05f)
        {
            validator.Validate(definition);
            if (float.IsNaN(heightTolerance) || float.IsInfinity(heightTolerance) || heightTolerance < 0f)
                throw new ArgumentOutOfRangeException(nameof(heightTolerance));

            int          completeCount = 0;
            List<string> inconsistent  = new();
            foreach (ShapeReferencePart part in definition.Parts)
            {
                if (part.Front != null && part.Side != null && part.Back != null)
                    completeCount++;

                if (HeightSpread(part) > heightTolerance)
                    inconsistent.Add(part.Id);
            }

            return new ShapeReferenceCoverageReport(definition.Parts.Count, completeCount, inconsistent);
        }

        private static float HeightSpread(ShapeReferencePart part)
        {
            float minimum = float.MaxValue;
            float maximum = float.MinValue;
            IncludeHeight(part.Front, ref minimum, ref maximum);
            IncludeHeight(part.Side, ref minimum, ref maximum);
            IncludeHeight(part.Back, ref minimum, ref maximum);
            return maximum - minimum;
        }

        private static void IncludeHeight(
            ShapeReferenceViewObservation view,
            ref float                     minimum,
            ref float                     maximum)
        {
            if (view == null)
                return;

            float height = view.Maximum.Y - view.Minimum.Y;
            minimum      = Math.Min(minimum, height);
            maximum      = Math.Max(maximum, height);
        }
    }
}
