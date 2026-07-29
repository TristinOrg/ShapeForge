namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Provides a furniture preset composed entirely from reusable Low Poly shapes.
    /// </summary>
    public static class LowPolyTablePreset
    {
        /// <summary>
        /// Gets the style identifier used by the table preset.
        /// </summary>
        public const string StyleId = "lowpoly/table";

        /// <summary>
        /// Creates the engine-agnostic table model definition.
        /// </summary>
        public static ShapeDefinition CreateDefinition()
        {
            return ShapeBuilder
                .Create("Low Poly Table")
                .WithStyle(StyleId)
                .Root("table", "Table", table => table
                    .Shape("table.top", "Top", LowPolyShapeTypes.Cube, top => top
                        .Position(0f, 1.05f, 0f)
                        .Scale(2.4f, 0.18f, 1.4f)
                        .ColorRole("wood.primary"))
                    .Shape("table.leg.front-left", "Front Left Leg", LowPolyShapeTypes.Cube, leg => leg
                        .Position(-0.9f, 0.5f, 0.45f)
                        .Scale(0.18f, 1f, 0.18f)
                        .ColorRole("wood.secondary"))
                    .Shape("table.leg.front-right", "Front Right Leg", LowPolyShapeTypes.Cube, leg => leg
                        .Position(0.9f, 0.5f, 0.45f)
                        .Scale(0.18f, 1f, 0.18f)
                        .ColorRole("wood.secondary"))
                    .Shape("table.leg.back-left", "Back Left Leg", LowPolyShapeTypes.Cube, leg => leg
                        .Position(-0.9f, 0.5f, -0.45f)
                        .Scale(0.18f, 1f, 0.18f)
                        .ColorRole("wood.secondary"))
                    .Shape("table.leg.back-right", "Back Right Leg", LowPolyShapeTypes.Cube, leg => leg
                        .Position(0.9f, 0.5f, -0.45f)
                        .Scale(0.18f, 1f, 0.18f)
                        .ColorRole("wood.secondary")))
                .Build();
        }

        /// <summary>
        /// Creates the engine-agnostic palette used by the table preset.
        /// </summary>
        public static ShapeStyleDefinition CreateStyle()
        {
            ShapeStyleDefinition style = new ShapeStyleDefinition(StyleId);
            style.Palette
                .Set("wood.primary", new ForgeColor(0.45f, 0.22f, 0.08f))
                .Set("wood.secondary", new ForgeColor(0.25f, 0.10f, 0.03f));
            return style;
        }
    }
}
