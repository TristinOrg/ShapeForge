namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Provides a realistically proportioned fantasy hero with articulated human joints.
    /// </summary>
    public static class LowPolyHeroPreset
    {
        /// <summary>Gets the style identifier used by the hero preset.</summary>
        public const string StyleId = "lowpoly/fantasy-hero";

        /// <summary>Creates the engine-agnostic fantasy hero definition.</summary>
        public static ShapeDefinition CreateDefinition()
        {
            return ShapeBuilder
                .Create("Low Poly Fantasy Hero")
                .WithStyle(StyleId)
                .Root("hero", "Fantasy Hero", hero =>
                {
                    hero.Group("hero.pelvis.pivot", "Pelvis Pivot", pelvis => pelvis
                        .Position(0f, 1.02f, 0f)
                        .Shape("hero.pelvis", "Pelvis", LowPolyShapeTypes.Frustum, shape => shape
                            .Scale(0.5f, 0.3f, 0.3f)
                            .Frustum(0.82f, 0.82f, 1f, 1f)
                            .ColorRole("trousers"))
                        .Group("hero.spine.pivot", "Spine Pivot", spine => spine
                            .Position(0f, 0.18f, 0f)
                            .Shape("hero.torso", "Fitted Coat", LowPolyShapeTypes.Frustum, torso => torso
                                .Position(0f, 0.43f, 0f)
                                .Scale(0.74f, 0.82f, 0.44f)
                                .Frustum(1f, 0.86f, 0.66f, 0.7f)
                                .ColorRole("coat"))
                            .Shape("hero.chest", "Chest Panel", LowPolyShapeTypes.Cube, chest => chest
                                .Position(0f, 0.48f, -0.23f)
                                .Scale(0.46f, 0.5f, 0.08f)
                                .ColorRole("shirt"))
                            .Shape("hero.belt", "Leather Belt", LowPolyShapeTypes.Cube, belt => belt
                                .Position(0f, 0.08f, -0.03f)
                                .Scale(0.56f, 0.1f, 0.32f)
                                .ColorRole("leather"))
                            .Shape("hero.belt.buckle", "Belt Buckle", LowPolyShapeTypes.Cube, buckle => buckle
                                .Position(0f, 0.08f, -0.22f)
                                .Scale(0.12f, 0.1f, 0.04f)
                                .ColorRole("metal"))
                            .Shape("hero.coat.tail.left", "Left Coat Tail", LowPolyShapeTypes.Wedge, tail => tail
                                .Position(-0.18f, -0.18f, 0.06f)
                                .Rotation(-8f, 90f, 4f)
                                .Scale(0.25f, 0.62f, 0.16f)
                                .ColorRole("coat"))
                            .Shape("hero.coat.tail.right", "Right Coat Tail", LowPolyShapeTypes.Wedge, tail => tail
                                .Position(0.18f, -0.18f, 0.06f)
                                .Rotation(-8f, -90f, -4f)
                                .Scale(0.25f, 0.62f, 0.16f)
                                .ColorRole("coat"))
                            .Shape("hero.shoulder.guard", "Shoulder Harness", LowPolyShapeTypes.Frustum, harness => harness
                                .Position(0f, 0.73f, 0f)
                                .Scale(0.82f, 0.12f, 0.34f)
                                .Frustum(0.82f, 0.88f, 1f, 1f)
                                .ColorRole("leather"))));

                    AddHead(hero);
                    AddArm(hero, "left", "Left", -0.43f);
                    AddArm(hero, "right", "Right", 0.43f);
                    AddLeg(hero, "left", "Left", -0.2f);
                    AddLeg(hero, "right", "Right", 0.2f);
                })
                .Build();
        }

        /// <summary>Creates the engine-agnostic palette used by the hero preset.</summary>
        public static ShapeStyleDefinition CreateStyle()
        {
            ShapeStyleDefinition style = new(StyleId);
            style.Palette
                .Set("skin", new(0.74f, 0.48f, 0.34f))
                .Set("skin.shadow", new(0.54f, 0.3f, 0.22f))
                .Set("hair", new(0.055f, 0.065f, 0.085f))
                .Set("coat", new(0.08f, 0.11f, 0.17f))
                .Set("shirt", new(0.64f, 0.68f, 0.7f))
                .Set("trousers", new(0.07f, 0.075f, 0.095f))
                .Set("leather", new(0.2f, 0.09f, 0.045f))
                .Set("metal", new(0.52f, 0.57f, 0.62f))
                .Set("accent", new(0.48f, 0.08f, 0.075f))
                .Set("eye", new(0.18f, 0.48f, 0.62f));
            return style;
        }

        private static void AddHead(ShapeNodeBuilder hero)
        {
            hero.Group("hero.head.pivot", "Head Pivot", head => head
                .Position(0f, 1.96f, 0f)
                .Shape("hero.neck", "Neck", LowPolyShapeTypes.Cylinder, neck => neck
                    .Position(0f, -0.12f, 0f).Scale(0.13f, 0.16f, 0.13f).ColorRole("skin.shadow"))
                .Shape("hero.head", "Head", LowPolyShapeTypes.Sphere, face => face
                    .Position(0f, 0.18f, 0f).Scale(0.3f, 0.38f, 0.28f).ColorRole("skin"))
                .Shape("hero.face.jaw", "Jaw", LowPolyShapeTypes.Frustum, jaw => jaw
                    .Position(0f, 0.05f, -0.12f).Scale(0.28f, 0.24f, 0.24f)
                    .Frustum(1f, 0.9f, 0.68f, 0.72f).ColorRole("skin"))
                .Shape("hero.eye.left", "Left Eye", LowPolyShapeTypes.Sphere, eye => eye
                    .Position(-0.1f, 0.23f, -0.255f).Scale(0.035f, 0.025f, 0.02f).ColorRole("eye"))
                .Shape("hero.eye.right", "Right Eye", LowPolyShapeTypes.Sphere, eye => eye
                    .Position(0.1f, 0.23f, -0.255f).Scale(0.035f, 0.025f, 0.02f).ColorRole("eye"))
                .Shape("hero.hair.crown", "Layered Hair Crown", LowPolyShapeTypes.Sphere, hair => hair
                    .Position(0f, 0.43f, 0.02f).Scale(0.34f, 0.24f, 0.32f).ColorRole("hair"))
                .Shape("hero.hair.fringe.left", "Left Fringe", LowPolyShapeTypes.Wedge, hair => hair
                    .Position(-0.12f, 0.31f, -0.27f).Rotation(0f, -12f, 24f).Scale(0.13f, 0.3f, 0.1f).ColorRole("hair"))
                .Shape("hero.hair.fringe.right", "Right Fringe", LowPolyShapeTypes.Wedge, hair => hair
                    .Position(0.1f, 0.33f, -0.275f).Rotation(0f, 14f, -18f).Scale(0.12f, 0.27f, 0.1f).ColorRole("hair"))
                .Shape("hero.hair.back", "Back Hair", LowPolyShapeTypes.Wedge, hair => hair
                    .Position(0f, 0.24f, 0.23f).Rotation(0f, 180f, 0f).Scale(0.34f, 0.42f, 0.16f).ColorRole("hair")));
        }

        private static void AddArm(ShapeNodeBuilder hero, string side, string label, float x)
        {
            string prefix = $"hero.arm.{side}";
            hero.Group($"{prefix}.shoulder.pivot", $"{label} Shoulder Pivot", shoulder => shoulder
                .Position(x, 1.7f, 0f)
                .Shape($"{prefix}.upper", $"{label} Upper Arm", LowPolyShapeTypes.Frustum, upper => upper
                    .Position(0f, -0.3f, 0f).Scale(0.25f, 0.56f, 0.25f)
                    .Frustum(1f, 1f, 0.72f, 0.72f).ColorRole("coat"))
                .Shape($"{prefix}.guard", $"{label} Shoulder Guard", LowPolyShapeTypes.Wedge, guard => guard
                    .Position(0f, -0.02f, -0.02f).Rotation(0f, 180f, 0f)
                    .Scale(0.3f, 0.22f, 0.3f).ColorRole("leather"))
                .Group($"{prefix}.elbow.pivot", $"{label} Elbow Pivot", elbow => elbow
                    .Position(0f, -0.59f, 0f)
                    .Shape($"{prefix}.forearm", $"{label} Forearm", LowPolyShapeTypes.Frustum, forearm => forearm
                        .Position(0f, -0.27f, 0f).Scale(0.23f, 0.5f, 0.23f)
                        .Frustum(1f, 1f, 0.68f, 0.68f).ColorRole("coat"))
                    .Shape($"{prefix}.glove", $"{label} Glove", LowPolyShapeTypes.Frustum, glove => glove
                        .Position(0f, -0.55f, -0.01f).Scale(0.18f, 0.2f, 0.16f)
                        .Frustum(0.82f, 0.82f, 1f, 1f).ColorRole("leather"))));
        }

        private static void AddLeg(ShapeNodeBuilder hero, string side, string label, float x)
        {
            string prefix = $"hero.leg.{side}";
            hero.Group($"{prefix}.hip.pivot", $"{label} Hip Pivot", hip => hip
                .Position(x, 0.98f, 0f)
                .Shape($"{prefix}.thigh", $"{label} Thigh", LowPolyShapeTypes.Frustum, thigh => thigh
                    .Position(0f, -0.36f, 0f).Scale(0.3f, 0.68f, 0.32f)
                    .Frustum(1f, 1f, 0.68f, 0.72f).ColorRole("trousers"))
                .Group($"{prefix}.knee.pivot", $"{label} Knee Pivot", knee => knee
                    .Position(0f, -0.72f, 0f)
                    .Shape($"{prefix}.knee", $"{label} Knee", LowPolyShapeTypes.Sphere, joint => joint
                        .Scale(0.15f, 0.14f, 0.16f).ColorRole("trousers"))
                    .Shape($"{prefix}.shin", $"{label} Shin", LowPolyShapeTypes.Frustum, shin => shin
                        .Position(0f, -0.32f, 0f).Scale(0.27f, 0.6f, 0.28f)
                        .Frustum(1f, 1f, 0.66f, 0.7f).ColorRole("trousers"))
                    .Shape($"{prefix}.boot", $"{label} Boot", LowPolyShapeTypes.Wedge, boot => boot
                        .Position(0f, -0.66f, -0.1f).Scale(0.32f, 0.24f, 0.52f).ColorRole("leather"))
                    .Shape($"{prefix}.boot.cuff", $"{label} Boot Cuff", LowPolyShapeTypes.Frustum, cuff => cuff
                        .Position(0f, -0.49f, 0f).Scale(0.3f, 0.18f, 0.31f)
                        .Frustum(1f, 1f, 0.8f, 0.82f).ColorRole("accent"))));
        }
    }
}
