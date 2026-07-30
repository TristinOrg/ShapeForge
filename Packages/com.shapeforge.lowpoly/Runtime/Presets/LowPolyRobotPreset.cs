namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Provides a detailed modular robot preset with transform-ready articulated pivots.
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
                .Create("Low Poly Sentinel Robot")
                .WithStyle(StyleId)
                .Root("robot", "Sentinel Robot", robot =>
                {
                    robot
                        .Shape("robot.body", "Armored Torso", LowPolyShapeTypes.Cube, body => body
                            .Position(0f, 1.75f, 0f)
                            .Scale(1.05f, 1.1f, 0.58f)
                            .ColorRole("metal.primary"))
                        .Shape("robot.chest", "Chest Plate", LowPolyShapeTypes.Cube, chest => chest
                            .Position(0f, 1.86f, 0.34f)
                            .Scale(0.72f, 0.5f, 0.1f)
                            .ColorRole("accent"))
                        .Shape("robot.reactor", "Chest Reactor", LowPolyShapeTypes.Sphere, reactor => reactor
                            .Position(0f, 1.88f, 0.44f)
                            .Scale(0.22f, 0.22f, 0.12f)
                            .ColorRole("glow"))
                        .Shape("robot.waist", "Waist", LowPolyShapeTypes.Cylinder, waist => waist
                            .Position(0f, 1.05f, 0f)
                            .Scale(0.48f, 0.18f, 0.48f)
                            .ColorRole("metal.dark"));

                    AddHead(robot);
                    AddArm(robot, "left", "Left", -0.72f);
                    AddArm(robot, "right", "Right", 0.72f);
                    AddLeg(robot, "left", "Left", -0.3f);
                    AddLeg(robot, "right", "Right", 0.3f);
                })
                .Build();
        }

        /// <summary>
        /// Creates the engine-agnostic palette used by the robot preset.
        /// </summary>
        public static ShapeStyleDefinition CreateStyle()
        {
            ShapeStyleDefinition style = new(StyleId);
            style.Palette
                .Set("metal.primary", new(0.08f, 0.34f, 0.62f))
                .Set("metal.secondary", new(0.18f, 0.25f, 0.34f))
                .Set("metal.dark", new(0.045f, 0.06f, 0.085f))
                .Set("accent", new(0.95f, 0.32f, 0.06f))
                .Set("glow", new(0.1f, 0.9f, 1f));
            return style;
        }

        private static void AddHead(ShapeNodeBuilder robot)
        {
            robot.Group("robot.head.pivot", "Head Pivot", pivot => pivot
                .Position(0f, 2.48f, 0f)
                .Shape("robot.neck", "Neck", LowPolyShapeTypes.Cylinder, neck => neck
                    .Scale(0.24f, 0.16f, 0.24f)
                    .ColorRole("metal.dark"))
                .Shape("robot.head", "Helmet", LowPolyShapeTypes.Cube, head => head
                    .Position(0f, 0.34f, 0f)
                    .Scale(0.72f, 0.58f, 0.62f)
                    .ColorRole("metal.primary"))
                .Shape("robot.visor", "Visor", LowPolyShapeTypes.Cube, visor => visor
                    .Position(0f, 0.38f, 0.34f)
                    .Scale(0.52f, 0.14f, 0.08f)
                    .ColorRole("glow"))
                .Shape("robot.antenna", "Antenna", LowPolyShapeTypes.Cylinder, antenna => antenna
                    .Position(0.22f, 0.85f, 0f)
                    .Scale(0.045f, 0.28f, 0.045f)
                    .ColorRole("metal.dark"))
                .Shape("robot.antenna.tip", "Antenna Tip", LowPolyShapeTypes.Sphere, tip => tip
                    .Position(0.22f, 1.14f, 0f)
                    .Scale(0.1f, 0.1f, 0.1f)
                    .ColorRole("accent")));
        }

        private static void AddArm(ShapeNodeBuilder robot, string side, string label, float x)
        {
            string prefix = $"robot.arm.{side}";
            robot.Group($"{prefix}.shoulder.pivot", $"{label} Shoulder Pivot", shoulder => shoulder
                .Position(x, 2.08f, 0f)
                .Shape($"{prefix}.shoulder", $"{label} Shoulder Armor", LowPolyShapeTypes.Sphere, armor => armor
                    .Scale(0.34f, 0.32f, 0.38f)
                    .ColorRole("metal.primary"))
                .Shape($"{prefix}.upper", $"{label} Upper Arm", LowPolyShapeTypes.Capsule, upper => upper
                    .Position(0f, -0.48f, 0f)
                    .Scale(0.2f, 0.38f, 0.2f)
                    .ColorRole("metal.secondary"))
                .Group($"{prefix}.elbow.pivot", $"{label} Elbow Pivot", elbow => elbow
                    .Position(0f, -0.9f, 0f)
                    .Shape($"{prefix}.elbow", $"{label} Elbow", LowPolyShapeTypes.Sphere, joint => joint
                        .Scale(0.22f, 0.22f, 0.22f)
                        .ColorRole("accent"))
                    .Shape($"{prefix}.forearm", $"{label} Forearm", LowPolyShapeTypes.Capsule, forearm => forearm
                        .Position(0f, -0.45f, 0f)
                        .Scale(0.24f, 0.4f, 0.24f)
                        .ColorRole("metal.primary"))
                    .Shape($"{prefix}.hand", $"{label} Hand", LowPolyShapeTypes.Cube, hand => hand
                        .Position(0f, -0.92f, 0.06f)
                        .Scale(0.28f, 0.22f, 0.34f)
                        .ColorRole("metal.dark"))));
        }

        private static void AddLeg(ShapeNodeBuilder robot, string side, string label, float x)
        {
            string prefix = $"robot.leg.{side}";
            robot.Group($"{prefix}.hip.pivot", $"{label} Hip Pivot", hip => hip
                .Position(x, 1.05f, 0f)
                .Shape($"{prefix}.upper", $"{label} Thigh", LowPolyShapeTypes.Capsule, thigh => thigh
                    .Position(0f, -0.28f, 0f)
                    .Scale(0.25f, 0.25f, 0.25f)
                    .ColorRole("metal.secondary"))
                .Group($"{prefix}.knee.pivot", $"{label} Knee Pivot", knee => knee
                    .Position(0f, -0.55f, 0f)
                    .Shape($"{prefix}.knee", $"{label} Knee", LowPolyShapeTypes.Sphere, joint => joint
                        .Scale(0.25f, 0.22f, 0.28f)
                        .ColorRole("accent"))
                    .Shape($"{prefix}.shin", $"{label} Shin", LowPolyShapeTypes.Capsule, shin => shin
                        .Position(0f, -0.25f, 0f)
                        .Scale(0.27f, 0.25f, 0.27f)
                        .ColorRole("metal.primary"))
                    .Shape($"{prefix}.foot", $"{label} Foot", LowPolyShapeTypes.Cube, foot => foot
                        .Position(0f, -0.48f, 0.14f)
                        .Scale(0.42f, 0.16f, 0.7f)
                        .ColorRole("metal.dark"))));
        }
    }
}
