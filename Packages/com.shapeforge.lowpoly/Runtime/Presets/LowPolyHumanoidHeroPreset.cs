namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Provides a rigid-part biped in a canonical T-Pose for Humanoid Avatar validation.
    /// </summary>
    public static class LowPolyHumanoidHeroPreset
    {
        /// <summary>
        /// Gets the shared fantasy-hero style identifier.
        /// </summary>
        public const string StyleId = LowPolyHeroPreset.StyleId;

        /// <summary>
        /// Creates a complete biped hierarchy with arms extended in its authored rest pose.
        /// </summary>
        public static ShapeDefinition CreateDefinition()
        {
            return ShapeBuilder
                .Create("Low Poly Humanoid Hero")
                .WithStyle(StyleId)
                .WithRig("humanoid/full",
                    new ShapeRigJoint(ShapeRigRoles.Root, "humanoid-hero"),
                    Joint(ShapeRigRoles.Hips, "humanoid-hero.hips"),
                    Joint(ShapeRigRoles.Spine, "humanoid-hero.spine"),
                    Joint(ShapeRigRoles.Chest, "humanoid-hero.chest"),
                    Joint(ShapeRigRoles.Neck, "humanoid-hero.neck"),
                    Joint(ShapeRigRoles.Head, "humanoid-hero.head"),
                    Joint(ShapeRigRoles.LeftUpperArm, "humanoid-hero.left-upper-arm"),
                    Joint(ShapeRigRoles.LeftLowerArm, "humanoid-hero.left-lower-arm"),
                    Joint(ShapeRigRoles.LeftHand, "humanoid-hero.left-hand"),
                    Joint(ShapeRigRoles.RightUpperArm, "humanoid-hero.right-upper-arm"),
                    Joint(ShapeRigRoles.RightLowerArm, "humanoid-hero.right-lower-arm"),
                    Joint(ShapeRigRoles.RightHand, "humanoid-hero.right-hand"),
                    Joint(ShapeRigRoles.LeftUpperLeg, "humanoid-hero.left-upper-leg"),
                    Joint(ShapeRigRoles.LeftLowerLeg, "humanoid-hero.left-lower-leg"),
                    Joint(ShapeRigRoles.LeftFoot, "humanoid-hero.left-foot"),
                    Joint(ShapeRigRoles.RightUpperLeg, "humanoid-hero.right-upper-leg"),
                    Joint(ShapeRigRoles.RightLowerLeg, "humanoid-hero.right-lower-leg"),
                    Joint(ShapeRigRoles.RightFoot, "humanoid-hero.right-foot"))
                .Root("humanoid-hero", "Low Poly Humanoid Hero", root => root
                    .Group("humanoid-hero.hips", "Hips", hips => hips
                        .Position(0f, 1.1f, 0f)
                        .Shape("humanoid-hero.pelvis", "Pelvis", LowPolyShapeTypes.Cube, pelvis => pelvis
                            .Scale(0.5f, 0.22f, 0.28f)
                            .ColorRole("pants"))
                        .Shape("humanoid-hero.belt", "Utility Belt", LowPolyShapeTypes.Cube, belt => belt
                            .Position(0f, 0.08f, -0.02f)
                            .Scale(0.54f, 0.07f, 0.3f)
                            .ColorRole("glove"))
                        .Group("humanoid-hero.spine", "Spine", spine => spine
                            .Position(0f, 0.25f, 0f)
                            .Shape("humanoid-hero.abdomen", "Abdomen", LowPolyShapeTypes.Capsule, abdomen => abdomen
                                .Position(0f, 0.2f, 0f)
                                .Scale(0.28f, 0.34f, 0.2f)
                                .ColorRole("shirt"))
                            .Group("humanoid-hero.chest", "Chest", chest => chest
                                .Position(0f, 0.48f, 0f)
                                .Shape("humanoid-hero.torso", "Torso", LowPolyShapeTypes.Frustum, torso => torso
                                    .Scale(0.72f, 0.48f, 0.32f)
                                    .Parameter(LowPolyShapeParameters.TopWidth, 1f)
                                    .Parameter(LowPolyShapeParameters.TopDepth, 1f)
                                    .Parameter(LowPolyShapeParameters.BottomWidth, 0.78f)
                                    .Parameter(LowPolyShapeParameters.BottomDepth, 0.88f)
                                    .ColorRole("jacket"))
                                .Shape("humanoid-hero.jacket-left", "Left Open Short Jacket",
                                    LowPolyShapeTypes.ExtrudedProfile, panel => panel
                                        .Position(-0.2f, 0.02f, -0.18f)
                                        .Scale(0.3f, 0.44f, 1f)
                                        .ExtrudedProfile(0.07f, 0.015f,
                                            new(-0.5f, 0.4f), new(-0.3f, 0.5f), new(0.48f, 0.42f),
                                            new(0.36f, -0.5f), new(-0.42f, -0.42f))
                                        .ColorRole("jacket.light"))
                                .Shape("humanoid-hero.jacket-right", "Right Open Short Jacket",
                                    LowPolyShapeTypes.ExtrudedProfile, panel => panel
                                        .Position(0.2f, 0.02f, -0.18f)
                                        .Scale(0.3f, 0.44f, 1f)
                                        .ExtrudedProfile(0.07f, 0.015f,
                                            new(-0.48f, 0.42f), new(0.3f, 0.5f), new(0.5f, 0.4f),
                                            new(0.42f, -0.42f), new(-0.36f, -0.5f))
                                        .ColorRole("jacket.light"))
                                .Shape("humanoid-hero.collar", "Standing Collar",
                                    LowPolyShapeTypes.ExtrudedProfile, collar => collar
                                        .Position(-0.11f, 0.31f, -0.18f)
                                        .Rotation(-8f, 8f, -8f)
                                        .Scale(0.16f, 0.2f, 1f)
                                        .ExtrudedProfile(0.12f, 0.012f,
                                            new(-0.5f, -0.5f), new(-0.3f, 0.48f),
                                            new(0.18f, 0.5f), new(0.5f, -0.34f))
                                        .ColorRole("jacket.light")
                                        .Mirror(ShapeMirrorAxis.X))
                                .Shape("humanoid-hero.hood", "Folded Back Hood", LowPolyShapeTypes.ProfileSweep,
                                    hood => hood
                                        .ProfileSweep(
                                            new ForgeVector2[]
                                            {
                                                new(-0.04f, 0f), new(0f, 0.04f),
                                                new(0.04f, 0f), new(0f, -0.04f)
                                            },
                                            new ForgeVector3[]
                                            {
                                                new(-0.3f, 0.3f, 0.15f), new(0f, 0.4f, 0.2f),
                                                new(0.3f, 0.3f, 0.15f)
                                            })
                                        .ColorRole("jacket"))
                                .Shape("humanoid-hero.pendant", "Chest Pendant",
                                    LowPolyShapeTypes.ExtrudedProfile, pendant => pendant
                                        .Position(0f, 0.06f, -0.2f)
                                        .Scale(0.08f, 0.1f, 1f)
                                        .ExtrudedProfile(0.025f, 0.004f,
                                            new(0f, 0.5f), new(0.45f, 0f),
                                            new(0f, -0.5f), new(-0.45f, 0f))
                                        .ColorRole("metal"))
                                .Group("humanoid-hero.neck", "Neck", neck => neck
                                    .Position(0f, 0.45f, 0f)
                                    .Shape("humanoid-hero.neck-mesh", "Neck Mesh", LowPolyShapeTypes.Cylinder,
                                        neckMesh => neckMesh
                                            .Scale(0.13f, 0.13f, 0.13f)
                                            .ColorRole("skin"))
                                    .Group("humanoid-hero.head", "Head", head => head
                                        .Position(0f, 0.2f, 0f)
                                        .Shape("humanoid-hero.head-mesh", "Head Mesh", LowPolyShapeTypes.Sphere,
                                            headMesh => headMesh
                                                .Position(0f, 0.25f, 0f)
                                                .Scale(0.34f, 0.4f, 0.3f)
                                                .ColorRole("skin"))
                                        .Shape("humanoid-hero.hair", "Hair", LowPolyShapeTypes.Sphere, hair => hair
                                            .Position(0f, 0.43f, 0.02f)
                                            .Scale(0.36f, 0.2f, 0.32f)
                                            .ColorRole("hair"))
                                        .Shape("humanoid-hero.ear", "Left Ear", LowPolyShapeTypes.Capsule, ear => ear
                                            .Position(-0.34f, 0.25f, 0f)
                                            .Scale(0.055f, 0.085f, 0.04f)
                                            .ColorRole("skin.shadow")
                                            .Mirror(ShapeMirrorAxis.X))
                                        .Shape("humanoid-hero.fringe-left", "Diagonal Primary Fringe",
                                            LowPolyShapeTypes.ExtrudedProfile, fringe => fringe
                                                .Position(-0.08f, 0.39f, -0.29f)
                                                .Rotation(0f, 0f, 10f)
                                                .Scale(0.28f, 0.24f, 1f)
                                                .ExtrudedProfile(0.05f, 0.008f,
                                                    new(-0.5f, 0.4f), new(-0.22f, 0.5f),
                                                    new(0.5f, 0.42f), new(0.2f, -0.5f),
                                                    new(-0.18f, -0.24f))
                                                .ColorRole("hair.shadow"))
                                        .Shape("humanoid-hero.fringe-right", "Diagonal Secondary Fringe",
                                            LowPolyShapeTypes.ExtrudedProfile, fringe => fringe
                                                .Position(0.17f, 0.4f, -0.285f)
                                                .Rotation(0f, 0f, -9f)
                                                .Scale(0.18f, 0.21f, 1f)
                                                .ExtrudedProfile(0.045f, 0.008f,
                                                    new(-0.5f, 0.42f), new(-0.12f, 0.5f),
                                                    new(0.5f, 0.36f), new(0.08f, -0.5f),
                                                    new(-0.3f, -0.18f))
                                                .ColorRole("hair"))))
                                .Group("humanoid-hero.left-upper-arm", "Left Upper Arm", arm =>
                                    AddArm(arm, "humanoid-hero.left", "Left", -1f))
                                .Group("humanoid-hero.right-upper-arm", "Right Upper Arm", arm =>
                                    AddArm(arm, "humanoid-hero.right", "Right", 1f))))
                        .Group("humanoid-hero.left-upper-leg", "Left Upper Leg", leg =>
                            AddLeg(leg, "humanoid-hero.left", "Left", -0.21f))
                        .Group("humanoid-hero.right-upper-leg", "Right Upper Leg", leg =>
                            AddLeg(leg, "humanoid-hero.right", "Right", 0.21f))))
                    .Build();
        }

        /// <summary>
        /// Creates the shared fantasy-hero palette used by the Humanoid validation preset.
        /// </summary>
        public static ShapeStyleDefinition CreateStyle()
        {
            return LowPolyHeroPreset.CreateStyle();
        }

        private static void AddArm(ShapeNodeBuilder arm, string prefix, string side, float direction)
        {
            arm.Position(0.5f * direction, 0.25f, 0f)
                .Shape($"{prefix}-upper-mesh", "Upper Arm Mesh", LowPolyShapeTypes.Capsule, upper => upper
                    .Position(0.3f * direction, 0f, 0f)
                    .Rotation(0f, 0f, 90f)
                    .Scale(0.13f, 0.34f, 0.13f)
                    .ColorRole("jacket"))
                .Group($"{prefix}-lower-arm", $"{side} Lower Arm", lower => lower
                    .Position(0.68f * direction, 0f, 0f)
                    .Shape($"{prefix}-lower-mesh", "Lower Arm Mesh", LowPolyShapeTypes.Capsule, forearm => forearm
                        .Position(0.27f * direction, 0f, 0f)
                        .Rotation(0f, 0f, 90f)
                        .Scale(0.11f, 0.3f, 0.11f)
                        .ColorRole("skin"))
                    .Shape($"{prefix}-glove", $"{side} Fingerless Glove", LowPolyShapeTypes.Cylinder, glove => glove
                        .Position(0.49f * direction, 0f, 0f)
                        .Rotation(0f, 0f, 90f)
                        .Scale(0.13f, 0.12f, 0.13f)
                        .ColorRole("glove"))
                    .Group($"{prefix}-hand", $"{side} Hand", hand => hand
                        .Position(0.58f * direction, 0f, 0f)
                        .Shape($"{prefix}-hand-mesh", "Hand Mesh", LowPolyShapeTypes.Cube, handMesh => handMesh
                            .Position(0.12f * direction, 0f, 0f)
                            .Scale(0.18f, 0.14f, 0.12f)
                            .ColorRole("skin"))));
        }

        private static void AddLeg(ShapeNodeBuilder leg, string prefix, string side, float x)
        {
            leg.Position(x, -0.42f, 0f)
                .Shape($"{prefix}-upper-leg-mesh", "Upper Leg Mesh", LowPolyShapeTypes.Capsule, upper => upper
                    .Position(0f, -0.33f, 0f)
                    .Scale(0.16f, 0.4f, 0.16f)
                    .ColorRole("pants"))
                .Group($"{prefix}-lower-leg", $"{side} Lower Leg", lower => lower
                    .Position(0f, -0.78f, 0f)
                    .Shape($"{prefix}-lower-leg-mesh", "Lower Leg Mesh", LowPolyShapeTypes.Capsule, shin => shin
                        .Position(0f, -0.3f, 0f)
                        .Scale(0.13f, 0.36f, 0.13f)
                        .ColorRole("boot"))
                    .Group($"{prefix}-foot", $"{side} Foot", foot => foot
                        .Position(0f, -0.68f, 0.12f)
                        .Shape($"{prefix}-foot-mesh", "Foot Mesh", LowPolyShapeTypes.Cube, footMesh => footMesh
                            .Position(0f, -0.06f, 0.14f)
                            .Scale(0.18f, 0.12f, 0.34f)
                            .ColorRole("boot"))
                        .Shape($"{prefix}-sole", $"{side} Red Boot Sole", LowPolyShapeTypes.Cube, sole => sole
                            .Position(0f, -0.19f, 0.15f)
                            .Scale(0.2f, 0.035f, 0.36f)
                            .ColorRole("sole"))));
        }

        private static ShapeRigJoint Joint(string role, string nodeId)
        {
            return new(role, nodeId);
        }
    }
}
