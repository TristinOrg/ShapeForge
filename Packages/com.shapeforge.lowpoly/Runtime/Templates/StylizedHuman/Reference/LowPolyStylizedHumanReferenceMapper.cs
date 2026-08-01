using System;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Deterministically maps normalized image observations to semantic template controls.
    /// </summary>
    public sealed class LowPolyStylizedHumanReferenceMapper
    {
        private readonly LowPolyStylizedHumanReferenceSpecificationValidator referenceValidator = new();
        private readonly LowPolyStylizedHumanSpecificationValidator          resultValidator    = new();

        /// <summary>
        /// Maps observations onto a copy of the supplied base specification.
        /// Unobserved side-view values remain unchanged.
        /// </summary>
        public LowPolyStylizedHumanSpecification Map(
            LowPolyStylizedHumanReferenceSpecification reference,
            LowPolyStylizedHumanSpecification baseSpecification = null)
        {
            referenceValidator.Validate(reference);
            if (baseSpecification != null)
                resultValidator.Validate(baseSpecification);

            LowPolyStylizedHumanSpecification result = Copy(baseSpecification ?? new());
            LowPolyStylizedHumanFrontReference front  = reference.Front;

            result.Proportions.ShoulderWidth = front.ShoulderWidth /
                                               LowPolyStylizedHumanReferenceBaseline.ShoulderWidth;
            result.Proportions.BodyWidth     = front.BodyWidth / LowPolyStylizedHumanReferenceBaseline.BodyWidth;
            result.Proportions.LegLength     = front.LegLength / LowPolyStylizedHumanReferenceBaseline.LegLength;
            result.Head.Width                = front.HeadWidth / LowPolyStylizedHumanReferenceBaseline.HeadWidth;
            result.Head.Height               = front.HeadHeight / LowPolyStylizedHumanReferenceBaseline.HeadHeight;
            result.Head.JawWidth             = front.JawWidthToHeadWidth /
                                               LowPolyStylizedHumanReferenceBaseline.JawWidthToHeadWidth;
            result.Hair.Volume               = front.HairWidthToHeadWidth /
                                               LowPolyStylizedHumanReferenceBaseline.HairWidthToHeadWidth;
            result.Hair.Parting              = front.Parting;
            result.Hair.FringeLength         = front.FringeLength;
            result.Hair.SideburnLength       = front.SideburnLength;

            if (reference.Side != null)
            {
                result.Head.Depth = reference.Side.HeadDepth /
                                    LowPolyStylizedHumanReferenceBaseline.HeadDepth;
                float sideVolume  = reference.Side.HairDepthToHeadDepth /
                                    LowPolyStylizedHumanReferenceBaseline.HairDepthToHeadDepth;
                result.Hair.Volume = (result.Hair.Volume + sideVolume) * 0.5f;
            }

            resultValidator.Validate(result);
            return result;
        }

        private static LowPolyStylizedHumanSpecification Copy(LowPolyStylizedHumanSpecification source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new LowPolyStylizedHumanSpecification
            {
                Schema       = source.Schema,
                Name         = source.Name,
                Style        = source.Style,
                OverallScale = source.OverallScale,
                Proportions  = source.Proportions == null
                    ? null
                    : new LowPolyStylizedHumanProportions
                    {
                        HeadScale     = source.Proportions.HeadScale,
                        ShoulderWidth = source.Proportions.ShoulderWidth,
                        BodyWidth     = source.Proportions.BodyWidth,
                        LegLength     = source.Proportions.LegLength
                    },
                Head         = source.Head == null
                    ? null
                    : new LowPolyStylizedHumanHead
                    {
                        Width    = source.Head.Width,
                        Height   = source.Head.Height,
                        Depth    = source.Head.Depth,
                        JawWidth = source.Head.JawWidth
                    },
                Hair         = source.Hair == null
                    ? null
                    : new LowPolyStylizedHumanHair
                    {
                        Volume         = source.Hair.Volume,
                        Parting        = source.Hair.Parting,
                        FringeLength   = source.Hair.FringeLength,
                        SideburnLength = source.Hair.SideburnLength
                    }
            };
        }
    }
}
