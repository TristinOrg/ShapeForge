namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Compiles semantic stylized-human controls into an articulated Low Poly shape hierarchy.
    /// </summary>
    public sealed class LowPolyStylizedHumanTemplate : ShapeTemplate<LowPolyStylizedHumanSpecification>
    {
        private static readonly ShapeTemplateDescriptor TemplateDescriptor = new(
            "lowpoly/stylized-human/1.0",
            "Builds an articulated stylized human from readable proportions, head, and hair controls.",
            "character",
            LowPolyStylizedHumanSpecification.CurrentSchema,
            new[]
            {
                ShapeTypes.Group,
                LowPolyShapeTypes.Sphere,
                LowPolyShapeTypes.Capsule,
                LowPolyShapeTypes.ExtrudedProfile,
                LowPolyShapeTypes.ProfileLoft,
                LowPolyShapeTypes.LatheProfile,
                LowPolyShapeTypes.ProfileSweep
            },
            "human",
            "character",
            "stylized",
            "articulated");
        private readonly LowPolyStylizedHumanSpecificationValidator validator = new();

        /// <summary>Gets the shared stateless stylized-human template.</summary>
        public static LowPolyStylizedHumanTemplate Instance { get; } = new();

        /// <inheritdoc />
        public override ShapeTemplateDescriptor Descriptor => TemplateDescriptor;

        /// <inheritdoc />
        public override ShapeDefinition Compile(LowPolyStylizedHumanSpecification specification)
        {
            validator.Validate(specification);
            return LowPolyHeroPreset.CreateDefinition(specification);
        }
    }
}
