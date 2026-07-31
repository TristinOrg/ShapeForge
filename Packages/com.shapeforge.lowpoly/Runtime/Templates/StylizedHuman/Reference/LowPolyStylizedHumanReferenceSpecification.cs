using System;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Defines normalized observations extracted from stylized-human reference images.
    /// </summary>
    [Serializable]
    public sealed class LowPolyStylizedHumanReferenceSpecification
    {
        /// <summary>Identifies the current reference-measurement specification.</summary>
        public const string CurrentSchema = "shapeforge.lowpoly.stylized-human-reference/1.0";

        /// <summary>Gets or sets the reference-measurement schema identifier.</summary>
        public string Schema { get; set; } = CurrentSchema;

        /// <summary>Gets or sets observations visible in the front view.</summary>
        public LowPolyStylizedHumanFrontReference Front { get; set; } = new();

        /// <summary>Gets or sets optional observations visible only in the side view.</summary>
        public LowPolyStylizedHumanSideReference Side { get; set; }
    }

    /// <summary>
    /// Defines front-view measurements normalized by figure height or their named parent dimension.
    /// </summary>
    [Serializable]
    public sealed class LowPolyStylizedHumanFrontReference
    {
        /// <summary>Gets or sets head width divided by total figure height.</summary>
        public float HeadWidth { get; set; } = 0.24f;

        /// <summary>Gets or sets head height divided by total figure height.</summary>
        public float HeadHeight { get; set; } = 0.26f;

        /// <summary>Gets or sets shoulder width divided by total figure height.</summary>
        public float ShoulderWidth { get; set; } = 0.34f;

        /// <summary>Gets or sets torso width divided by total figure height.</summary>
        public float BodyWidth { get; set; } = 0.17f;

        /// <summary>Gets or sets leg length divided by total figure height.</summary>
        public float LegLength { get; set; } = 0.49f;

        /// <summary>Gets or sets jaw width divided by head width.</summary>
        public float JawWidthToHeadWidth { get; set; } = 0.78f;

        /// <summary>Gets or sets hair width divided by head width.</summary>
        public float HairWidthToHeadWidth { get; set; } = 1.15f;

        /// <summary>Gets or sets the visible hair part from image-left zero to image-right one.</summary>
        public float Parting { get; set; } = 0.7f;

        /// <summary>Gets or sets fringe length normalized from hairline zero to jaw one.</summary>
        public float FringeLength { get; set; } = 0.5f;

        /// <summary>Gets or sets sideburn length normalized from temple zero to jaw one.</summary>
        public float SideburnLength { get; set; } = 0.5f;
    }

    /// <summary>
    /// Defines optional side-view measurements that must not be inferred from a front view.
    /// </summary>
    [Serializable]
    public sealed class LowPolyStylizedHumanSideReference
    {
        /// <summary>Gets or sets head depth divided by total figure height.</summary>
        public float HeadDepth { get; set; } = 0.21f;

        /// <summary>Gets or sets hair depth divided by head depth.</summary>
        public float HairDepthToHeadDepth { get; set; } = 1.08f;
    }
}
