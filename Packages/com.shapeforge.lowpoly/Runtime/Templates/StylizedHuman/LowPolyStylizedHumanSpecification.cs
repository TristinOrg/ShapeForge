using System;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Defines a readable, versioned stylized-human specification for LLM and tool authoring.
    /// </summary>
    [Serializable]
    public sealed class LowPolyStylizedHumanSpecification
    {
        /// <summary>Identifies the current Low Poly stylized-human specification.</summary>
        public const string CurrentSchema = "shapeforge.lowpoly.stylized-human/1.0";

        /// <summary>Gets or sets the specification schema identifier.</summary>
        public string Schema { get; set; } = CurrentSchema;

        /// <summary>Gets or sets the generated model name.</summary>
        public string Name { get; set; } = "Pocket Fantasy Hero";

        /// <summary>Gets or sets the ShapeForge style resolved after compilation.</summary>
        public string Style { get; set; } = LowPolyHeroPreset.StyleId;

        /// <summary>Gets or sets uniform scale applied to the complete model.</summary>
        public float OverallScale { get; set; } = 1f;

        /// <summary>Gets or sets semantic body proportions.</summary>
        public LowPolyStylizedHumanProportions Proportions { get; set; } = new();

        /// <summary>Gets or sets semantic head proportions.</summary>
        public LowPolyStylizedHumanHead Head { get; set; } = new();

        /// <summary>Gets or sets semantic hair controls.</summary>
        public LowPolyStylizedHumanHair Hair { get; set; } = new();
    }

    /// <summary>
    /// Defines normalized body proportions around the template defaults.
    /// </summary>
    [Serializable]
    public sealed class LowPolyStylizedHumanProportions
    {
        /// <summary>Gets or sets the head size multiplier.</summary>
        public float HeadScale { get; set; } = 1f;

        /// <summary>Gets or sets shoulder spacing without changing arm thickness.</summary>
        public float ShoulderWidth { get; set; } = 1f;

        /// <summary>Gets or sets torso and pelvis width.</summary>
        public float BodyWidth { get; set; } = 1f;

        /// <summary>Gets or sets upper- and lower-leg length without changing width.</summary>
        public float LegLength { get; set; } = 1f;
    }

    /// <summary>
    /// Defines normalized face-volume controls around the template defaults.
    /// </summary>
    [Serializable]
    public sealed class LowPolyStylizedHumanHead
    {
        /// <summary>Gets or sets head width.</summary>
        public float Width { get; set; } = 1f;

        /// <summary>Gets or sets head height.</summary>
        public float Height { get; set; } = 1f;

        /// <summary>Gets or sets front-to-back head depth.</summary>
        public float Depth { get; set; } = 1f;

        /// <summary>Gets or sets lower-face and jaw width.</summary>
        public float JawWidth { get; set; } = 1f;
    }

    /// <summary>
    /// Defines normalized hair silhouette controls and asymmetric fringe direction.
    /// </summary>
    [Serializable]
    public sealed class LowPolyStylizedHumanHair
    {
        /// <summary>Gets or sets overall hair-shell volume.</summary>
        public float Volume { get; set; } = 1f;

        /// <summary>Gets or sets the part location from left zero to right one.</summary>
        public float Parting { get; set; } = 0.7f;

        /// <summary>Gets or sets the fringe drop around the forehead.</summary>
        public float FringeLength { get; set; } = 0.5f;

        /// <summary>Gets or sets sideburn length around the ears.</summary>
        public float SideburnLength { get; set; } = 0.5f;
    }
}
