namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Provides a modular robot preset with transform-ready pivot groups.
    /// </summary>
    public static class LowPolyRobotPreset
    {
        /// <summary>
        /// Gets the style identifier used by the robot preset.
        /// </summary>
        public const string StyleId = "lowpoly/robot";

        /// <summary>
        /// Creates the engine-agnostic robot model definition.
        /// </summary>
        public static ShapeDefinition CreateDefinition()
        {
            return ShapeBuilder
                .Create("Low Poly Robot")
                .WithStyle(StyleId)
                .Root("robot", "Robot", robot => robot
                    .Shape("robot.body", "Body", LowPolyShapeTypes.Cube, body => body
                        .Position(0f, 1.45f, 0f)
                        .Scale(0.9f, 1.2f, 0.5f)
                        .ColorRole("metal.primary"))
                    .Shape("robot.chest", "Chest Panel", LowPolyShapeTypes.Cube, chest => chest
                        .Position(0f, 1.5f, 0.29f)
                        .Scale(0.45f, 0.4f, 0.08f)
                        .ColorRole("accent"))
                    .Group("robot.head.pivot", "Head Pivot", pivot => pivot
                        .Position(0f, 2.2f, 0f)
                        .Shape("robot.head", "Head", LowPolyShapeTypes.Cube, head => head
                            .Position(0f, 0.2f, 0f)
                            .Scale(0.65f, 0.65f, 0.65f)
                            .ColorRole("metal.primary")))
                    .Group("robot.arm.left.pivot", "Left Shoulder Pivot", pivot => pivot
                        .Position(-0.65f, 1.85f, 0f)
                        .Shape("robot.arm.left", "Left Arm", LowPolyShapeTypes.Cube, arm => arm
                            .Position(0f, -0.45f, 0f)
                            .Scale(0.25f, 0.9f, 0.25f)
                            .ColorRole("metal.secondary")))
                    .Group("robot.arm.right.pivot", "Right Shoulder Pivot", pivot => pivot
                        .Position(0.65f, 1.85f, 0f)
                        .Shape("robot.arm.right", "Right Arm", LowPolyShapeTypes.Cube, arm => arm
                            .Position(0f, -0.45f, 0f)
                            .Scale(0.25f, 0.9f, 0.25f)
                            .ColorRole("metal.secondary")))
                    .Group("robot.leg.left.pivot", "Left Hip Pivot", pivot => pivot
                        .Position(-0.28f, 0.85f, 0f)
                        .Shape("robot.leg.left", "Left Leg", LowPolyShapeTypes.Cube, leg => leg
                            .Position(0f, -0.55f, 0f)
                            .Scale(0.3f, 1.1f, 0.35f)
                            .ColorRole("metal.secondary")))
                    .Group("robot.leg.right.pivot", "Right Hip Pivot", pivot => pivot
                        .Position(0.28f, 0.85f, 0f)
                        .Shape("robot.leg.right", "Right Leg", LowPolyShapeTypes.Cube, leg => leg
                            .Position(0f, -0.55f, 0f)
                            .Scale(0.3f, 1.1f, 0.35f)
                            .ColorRole("metal.secondary"))))
                .Build();
        }

        /// <summary>
        /// Creates the engine-agnostic palette used by the robot preset.
        /// </summary>
        public static ShapeStyleDefinition CreateStyle()
        {
            ShapeStyleDefinition style = new ShapeStyleDefinition(StyleId);
            style.Palette
                .Set("metal.primary", new ForgeColor(0.12f, 0.42f, 0.72f))
                .Set("metal.secondary", new ForgeColor(0.16f, 0.20f, 0.26f))
                .Set("accent", new ForgeColor(0.95f, 0.48f, 0.08f));
            return style;
        }
    }
}
