using System;
using System.Collections.Generic;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Produces deterministic coverage and deviation diagnostics for reference measurements.
    /// </summary>
    public sealed class LowPolyStylizedHumanReferenceAnalyzer
    {
        private const float StrongDeviation   = 0.2f;
        private const float ModerateDeviation = 0.1f;

        private readonly LowPolyStylizedHumanReferenceSpecificationValidator validator = new();

        /// <summary>Validates and analyzes a reference specification without compiling geometry.</summary>
        public LowPolyStylizedHumanReferenceReport Analyze(
            LowPolyStylizedHumanReferenceSpecification specification)
        {
            validator.Validate(specification);

            LowPolyStylizedHumanFrontReference front = specification.Front;
            List<LowPolyStylizedHumanReferenceDiagnostic> diagnostics = new(specification.Side == null ? 10 : 12)
            {
                Create("front.headWidth", front.HeadWidth,
                    LowPolyStylizedHumanReferenceBaseline.HeadWidth, LowPolyStylizedHumanReferenceView.Front),
                Create("front.headHeight", front.HeadHeight,
                    LowPolyStylizedHumanReferenceBaseline.HeadHeight, LowPolyStylizedHumanReferenceView.Front),
                Create("front.shoulderWidth", front.ShoulderWidth,
                    LowPolyStylizedHumanReferenceBaseline.ShoulderWidth, LowPolyStylizedHumanReferenceView.Front),
                Create("front.bodyWidth", front.BodyWidth,
                    LowPolyStylizedHumanReferenceBaseline.BodyWidth, LowPolyStylizedHumanReferenceView.Front),
                Create("front.legLength", front.LegLength,
                    LowPolyStylizedHumanReferenceBaseline.LegLength, LowPolyStylizedHumanReferenceView.Front),
                Create("front.jawWidthToHeadWidth", front.JawWidthToHeadWidth,
                    LowPolyStylizedHumanReferenceBaseline.JawWidthToHeadWidth,
                    LowPolyStylizedHumanReferenceView.Front),
                Create("front.hairWidthToHeadWidth", front.HairWidthToHeadWidth,
                    LowPolyStylizedHumanReferenceBaseline.HairWidthToHeadWidth,
                    LowPolyStylizedHumanReferenceView.Front),
                Create("front.parting", front.Parting,
                    LowPolyStylizedHumanReferenceBaseline.Parting, LowPolyStylizedHumanReferenceView.Front),
                Create("front.fringeLength", front.FringeLength,
                    LowPolyStylizedHumanReferenceBaseline.FringeLength, LowPolyStylizedHumanReferenceView.Front),
                Create("front.sideburnLength", front.SideburnLength,
                    LowPolyStylizedHumanReferenceBaseline.SideburnLength, LowPolyStylizedHumanReferenceView.Front)
            };

            if (specification.Side != null)
            {
                diagnostics.Add(Create("side.headDepth", specification.Side.HeadDepth,
                    LowPolyStylizedHumanReferenceBaseline.HeadDepth, LowPolyStylizedHumanReferenceView.Side));
                diagnostics.Add(Create("side.hairDepthToHeadDepth", specification.Side.HairDepthToHeadDepth,
                    LowPolyStylizedHumanReferenceBaseline.HairDepthToHeadDepth,
                    LowPolyStylizedHumanReferenceView.Side));
            }

            return new(diagnostics.AsReadOnly(), specification.Side != null);
        }

        private static LowPolyStylizedHumanReferenceDiagnostic Create(
            string                            path,
            float                             observed,
            float                             baseline,
            LowPolyStylizedHumanReferenceView view)
        {
            float difference = Math.Abs(observed / baseline - 1f);
            LowPolyStylizedHumanReferenceDeviation deviation = difference >= StrongDeviation
                ? LowPolyStylizedHumanReferenceDeviation.Strong
                : difference >= ModerateDeviation
                    ? LowPolyStylizedHumanReferenceDeviation.Moderate
                    : LowPolyStylizedHumanReferenceDeviation.NearBaseline;

            return new(path, view, observed, baseline, deviation);
        }
    }
}
