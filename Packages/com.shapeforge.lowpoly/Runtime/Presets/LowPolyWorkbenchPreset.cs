namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Provides a detailed inventor workbench composed from reusable Low Poly shapes.
    /// </summary>
    public static class LowPolyWorkbenchPreset
    {
        /// <summary>
        /// Gets the style identifier used by the workbench preset.
        /// </summary>
        public const string StyleId = "lowpoly/workbench";

        /// <summary>
        /// Creates the engine-agnostic workbench model definition.
        /// </summary>
        public static ShapeDefinition CreateDefinition()
        {
            return ShapeBuilder
                .Create("Low Poly Inventor Workbench")
                .WithStyle(StyleId)
                .Root("workbench", "Inventor Workbench", table =>
                {
                    table
                        .Shape("workbench.top", "Worktop", LowPolyShapeTypes.Cube, top => top
                            .Position(0f, 1.15f, 0f)
                            .Scale(2.8f, 0.2f, 1.35f)
                            .ColorRole("wood.primary"))
                        .Shape("workbench.top.front", "Front Edge", LowPolyShapeTypes.Cube, edge => edge
                            .Position(0f, 1.11f, -0.68f)
                            .Scale(2.9f, 0.3f, 0.1f)
                            .ColorRole("metal.dark"))
                        .Shape("workbench.shelf.lower", "Lower Shelf", LowPolyShapeTypes.Cube, shelf => shelf
                            .Position(0f, 0.32f, 0f)
                            .Scale(2.35f, 0.14f, 0.95f)
                            .ColorRole("wood.secondary"))
                        .Shape("workbench.brace.back", "Back Crossbar", LowPolyShapeTypes.Cube, brace => brace
                            .Position(0f, 0.72f, 0.52f)
                            .Scale(2.35f, 0.16f, 0.14f)
                            .ColorRole("metal.dark"));

                    AddLeg(table, "front-left", "Front Left", -1.08f, -0.47f);
                    AddLeg(table, "front-right", "Front Right", 1.08f, -0.47f);
                    AddLeg(table, "back-left", "Back Left", -1.08f, 0.47f);
                    AddLeg(table, "back-right", "Back Right", 1.08f, 0.47f);
                    AddDrawer(table);
                    AddToolBoard(table);
                    AddLamp(table);
                    AddMug(table);
                })
                .Build();
        }

        /// <summary>
        /// Creates the engine-agnostic palette used by the workbench preset.
        /// </summary>
        public static ShapeStyleDefinition CreateStyle()
        {
            ShapeStyleDefinition style = new(StyleId);
            style.Palette
                .Set("wood.primary", new(0.5f, 0.24f, 0.07f))
                .Set("wood.secondary", new(0.28f, 0.11f, 0.025f))
                .Set("metal.dark", new(0.08f, 0.1f, 0.12f))
                .Set("metal.accent", new(0.82f, 0.3f, 0.05f))
                .Set("ceramic", new(0.12f, 0.55f, 0.62f))
                .Set("light", new(1f, 0.72f, 0.18f));
            return style;
        }

        private static void AddLeg(ShapeNodeBuilder table, string id, string label, float x, float z)
        {
            float footZ = z + (z < 0f ? -0.08f : 0.08f);
            table
                .Shape($"workbench.leg.{id}", $"{label} Leg", LowPolyShapeTypes.Cube, leg => leg
                    .Position(x, 0.55f, z)
                    .Scale(0.22f, 1.1f, 0.22f)
                    .ColorRole("metal.dark"))
                .Shape($"workbench.foot.{id}", $"{label} Foot", LowPolyShapeTypes.Cube, foot => foot
                    .Position(x, 0.08f, footZ)
                    .Scale(0.42f, 0.12f, 0.55f)
                    .ColorRole("metal.accent"));
        }

        private static void AddDrawer(ShapeNodeBuilder table)
        {
            table.Group("workbench.drawer", "Drawer", drawer => drawer
                .Position(0.62f, 0.88f, -0.35f)
                .Shape("workbench.drawer.body", "Drawer Body", LowPolyShapeTypes.Cube, body => body
                    .Scale(1.05f, 0.34f, 0.62f)
                    .ColorRole("wood.secondary"))
                .Shape("workbench.drawer.face", "Drawer Face", LowPolyShapeTypes.Cube, face => face
                    .Position(0f, 0f, -0.34f)
                    .Scale(1.12f, 0.4f, 0.1f)
                    .ColorRole("wood.primary"))
                .Shape("workbench.drawer.handle", "Drawer Handle", LowPolyShapeTypes.Cylinder, handle => handle
                    .Position(0f, 0f, -0.43f)
                    .Rotation(90f, 0f, 0f)
                    .Scale(0.09f, 0.08f, 0.09f)
                    .ColorRole("metal.accent")));
        }

        private static void AddToolBoard(ShapeNodeBuilder table)
        {
            table.Group("workbench.toolboard", "Tool Board", board => board
                .Position(0f, 1.78f, 0.58f)
                .Shape("workbench.toolboard.panel", "Back Board", LowPolyShapeTypes.Cube, panel => panel
                    .Scale(2.45f, 1.05f, 0.12f)
                    .ColorRole("wood.secondary"))
                .Shape("workbench.toolboard.shelf", "Upper Shelf", LowPolyShapeTypes.Cube, shelf => shelf
                    .Position(0f, 0.58f, -0.22f)
                    .Scale(2.1f, 0.12f, 0.48f)
                    .ColorRole("wood.primary"))
                .Shape("workbench.tool.hammer", "Hammer", LowPolyShapeTypes.Cube, hammer => hammer
                    .Position(-0.62f, 0.05f, -0.12f)
                    .Rotation(0f, 0f, -18f)
                    .Scale(0.12f, 0.62f, 0.1f)
                    .ColorRole("metal.dark"))
                .Shape("workbench.tool.hammer.head", "Hammer Head", LowPolyShapeTypes.Cube, head => head
                    .Position(-0.73f, 0.34f, -0.12f)
                    .Rotation(0f, 0f, -18f)
                    .Scale(0.42f, 0.14f, 0.16f)
                    .ColorRole("metal.accent"))
                .Shape("workbench.tool.gauge", "Round Gauge", LowPolyShapeTypes.Cylinder, gauge => gauge
                    .Position(0.52f, 0.1f, -0.12f)
                    .Rotation(90f, 0f, 0f)
                    .Scale(0.28f, 0.08f, 0.28f)
                    .ColorRole("ceramic")));
        }

        private static void AddLamp(ShapeNodeBuilder table)
        {
            table.Group("workbench.lamp", "Task Lamp", lamp => lamp
                .Position(-0.95f, 1.32f, -0.18f)
                .Shape("workbench.lamp.base", "Lamp Base", LowPolyShapeTypes.Cylinder, lampBase => lampBase
                    .Scale(0.28f, 0.08f, 0.28f)
                    .ColorRole("metal.dark"))
                .Shape("workbench.lamp.stem", "Lamp Stem", LowPolyShapeTypes.Cylinder, stem => stem
                    .Position(0f, 0.48f, 0f)
                    .Rotation(0f, 0f, -12f)
                    .Scale(0.07f, 0.5f, 0.07f)
                    .ColorRole("metal.accent"))
                .Shape("workbench.lamp.shade", "Lamp Shade", LowPolyShapeTypes.Sphere, shade => shade
                    .Position(0.16f, 0.93f, -0.05f)
                    .Scale(0.34f, 0.24f, 0.34f)
                    .ColorRole("metal.dark"))
                .Shape("workbench.lamp.bulb", "Lamp Bulb", LowPolyShapeTypes.Sphere, bulb => bulb
                    .Position(0.16f, 0.82f, -0.18f)
                    .Scale(0.17f, 0.17f, 0.17f)
                    .ColorRole("light")));
        }

        private static void AddMug(ShapeNodeBuilder table)
        {
            table.Group("workbench.mug", "Workshop Mug", mug => mug
                .Position(0.08f, 1.36f, -0.18f)
                .Shape("workbench.mug.body", "Mug Body", LowPolyShapeTypes.Cylinder, body => body
                    .Scale(0.2f, 0.22f, 0.2f)
                    .ColorRole("ceramic"))
                .Shape("workbench.mug.handle", "Mug Handle", LowPolyShapeTypes.Cube, handle => handle
                    .Position(0.23f, 0f, 0f)
                    .Scale(0.18f, 0.22f, 0.08f)
                    .ColorRole("ceramic")));
        }
    }
}
