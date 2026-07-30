namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Provides a compact modern-fantasy character used as the humanoid generation benchmark.
    /// </summary>
    public static class LowPolyHeroPreset
    {
        /// <summary>Gets the style identifier used by the hero preset.</summary>
        public const string StyleId = "lowpoly/fantasy-hero";

        /// <summary>Creates the engine-agnostic pocket fantasy hero definition.</summary>
        public static ShapeDefinition CreateDefinition()
        {
            return ShapeBuilder
                .Create("Pocket Fantasy Hero")
                .WithStyle(StyleId)
                .Root("hero", "Pocket Fantasy Hero", hero =>
                {
                    AddBody(hero);
                    AddHead(hero);
                    AddArm(hero, "left", "Left", -0.36f);
                    AddArm(hero, "right", "Right", 0.36f);
                    AddLeg(hero, "left", "Left", -0.17f);
                    AddLeg(hero, "right", "Right", 0.17f);
                })
                .Build();
        }

        /// <summary>Creates the dark modern-fantasy palette used by the benchmark character.</summary>
        public static ShapeStyleDefinition CreateStyle()
        {
            ShapeStyleDefinition style = new(StyleId);
            style.Palette
                .Set("skin", new(0.76f, 0.52f, 0.38f))
                .Set("skin.shadow", new(0.58f, 0.34f, 0.24f))
                .Set("hair", new(0.025f, 0.045f, 0.075f))
                .Set("hair.highlight", new(0.055f, 0.085f, 0.13f))
                .Set("shirt", new(0.012f, 0.014f, 0.018f))
                .Set("jacket", new(0.035f, 0.04f, 0.05f))
                .Set("jacket.highlight", new(0.075f, 0.08f, 0.09f))
                .Set("trousers", new(0.045f, 0.05f, 0.06f))
                .Set("glove", new(0.025f, 0.028f, 0.035f))
                .Set("boot", new(0.018f, 0.022f, 0.028f))
                .Set("eye", new(0.025f, 0.02f, 0.018f))
                .Set("mouth", new(0.34f, 0.16f, 0.13f));
            return style;
        }

        private static void AddBody(ShapeNodeBuilder hero)
        {
            hero.Group("hero.pelvis.pivot", "Pelvis Pivot", pelvis => pelvis
                .Position(0f, 0.98f, 0f)
                .Shape("hero.pelvis", "Slim Waist", LowPolyShapeTypes.ProfileLoft, waist => waist
                    .Scale(0.44f, 0.3f, 0.34f)
                    .ProfileLoft(
                        BoxProfile(),
                        Section(-0.5f, 0.9f, 0.9f),
                        Section(0f, 1f, 1f),
                        Section(0.5f, 0.86f, 0.92f))
                    .ColorRole("trousers"))
                .Shape("hero.jacket.hem", "Overlapping Jacket Hem", LowPolyShapeTypes.Frustum, hem => hem
                    .Position(0f, 0.14f, -0.01f).Scale(0.62f, 0.12f, 0.36f)
                    .Frustum(0.92f, 0.94f, 1f, 1f).ColorRole("jacket"))
                .Group("hero.spine.pivot", "Spine Pivot", spine => spine
                    .Position(0f, 0.12f, 0f)
                    .Shape("hero.torso", "Jacket Torso", LowPolyShapeTypes.ProfileLoft, torso => torso
                        .Position(0f, 0.4f, 0f)
                        .Scale(0.62f, 0.72f, 0.4f)
                        .ProfileLoft(
                            TorsoProfile(),
                            Section(-0.5f, 0.94f, 0.98f),
                            Section(0f, 1f, 1f),
                            Section(0.5f, 0.84f, 0.94f))
                        .ColorRole("jacket"))
                    .Shape("hero.shirt", "Black Shirt", LowPolyShapeTypes.ExtrudedProfile, shirt => shirt
                        .Position(0f, 0.38f, -0.225f).Scale(0.3f, 0.63f, 1f)
                        .ExtrudedProfile(0.045f, 0.012f,
                            new(-0.46f, 0.5f), new(0.46f, 0.5f), new(0.5f, -0.5f), new(-0.5f, -0.5f))
                        .ColorRole("shirt"))
                    .Shape("hero.jacket.left", "Left Open Jacket Panel", LowPolyShapeTypes.ExtrudedProfile, panel => panel
                        .Position(-0.2f, 0.37f, -0.25f).Scale(0.25f, 0.68f, 1f)
                        .ExtrudedProfile(0.05f, 0.014f,
                            new(-0.5f, 0.5f), new(0.5f, 0.38f), new(0.28f, -0.5f), new(-0.5f, -0.42f))
                        .ColorRole("jacket.highlight"))
                    .Shape("hero.jacket.right", "Right Open Jacket Panel", LowPolyShapeTypes.ExtrudedProfile, panel => panel
                        .Position(0.2f, 0.37f, -0.25f).Scale(0.25f, 0.68f, 1f)
                        .ExtrudedProfile(0.05f, 0.014f,
                            new(-0.5f, 0.38f), new(0.5f, 0.5f), new(0.5f, -0.42f), new(-0.28f, -0.5f))
                        .ColorRole("jacket.highlight"))
                    .Shape("hero.collar.left", "Left Raised Collar", LowPolyShapeTypes.ExtrudedProfile, collar => collar
                        .Position(-0.16f, 0.76f, -0.14f).Rotation(0f, 0f, -16f).Scale(0.24f, 0.3f, 1f)
                        .ExtrudedProfile(0.16f, 0.03f,
                            new(-0.5f, -0.5f), new(0.48f, -0.34f), new(0.18f, 0.5f), new(-0.42f, 0.32f))
                        .ColorRole("jacket"))
                    .Shape("hero.collar.right", "Right Raised Collar", LowPolyShapeTypes.ExtrudedProfile, collar => collar
                        .Position(0.16f, 0.76f, -0.14f).Rotation(0f, 0f, 16f).Scale(0.24f, 0.3f, 1f)
                        .ExtrudedProfile(0.16f, 0.03f,
                            new(-0.48f, -0.34f), new(0.5f, -0.5f), new(0.42f, 0.32f), new(-0.18f, 0.5f))
                        .ColorRole("jacket"))
                    .Shape("hero.pocket.left", "Left Chest Pocket", LowPolyShapeTypes.Cube, pocket => pocket
                        .Position(-0.2f, 0.51f, -0.285f).Scale(0.17f, 0.1f, 0.025f).ColorRole("jacket"))
                    .Shape("hero.pocket.right", "Right Chest Pocket", LowPolyShapeTypes.Cube, pocket => pocket
                        .Position(0.2f, 0.51f, -0.285f).Scale(0.17f, 0.1f, 0.025f).ColorRole("jacket"))));
        }

        private static void AddHead(ShapeNodeBuilder hero)
        {
            hero.Group("hero.head.pivot", "Head Pivot", head => head
                .Position(0f, 1.83f, 0f)
                .Shape("hero.neck", "Neck", LowPolyShapeTypes.Cylinder, neck => neck
                    .Position(0f, -0.08f, 0f).Scale(0.12f, 0.18f, 0.12f).ColorRole("skin.shadow"))
                .Shape("hero.head", "Rounded Face", LowPolyShapeTypes.ProfileLoft, face => face
                    .Position(0f, 0.2f, 0f).Scale(0.58f, 0.62f, 0.52f)
                    .ProfileLoft(
                        FaceProfile(),
                        Section(-0.5f, 0.88f, 0.94f, 0f, -0.02f),
                        Section(-0.1f, 1f, 1f),
                        Section(0.3f, 0.98f, 1f),
                        Section(0.5f, 0.82f, 0.92f))
                    .ColorRole("skin"))
                .Shape("hero.eye.left", "Left Eye", LowPolyShapeTypes.Cube, eye => eye
                    .Position(-0.095f, 0.18f, -0.274f).Scale(0.04f, 0.018f, 0.012f).ColorRole("eye"))
                .Shape("hero.eye.right", "Right Eye", LowPolyShapeTypes.Cube, eye => eye
                    .Position(0.095f, 0.18f, -0.274f).Scale(0.04f, 0.018f, 0.012f).ColorRole("eye"))
                .Shape("hero.mouth", "Subtle Mouth", LowPolyShapeTypes.Cube, mouth => mouth
                    .Position(0f, 0.06f, -0.272f).Scale(0.055f, 0.01f, 0.01f).ColorRole("mouth"))
                .Shape("hero.hair.cap", "Full Layered Hair", LowPolyShapeTypes.ProfileLoft, hair => hair
                    .Position(0f, 0.39f, 0f).Scale(0.72f, 0.64f, 0.66f)
                    .ProfileLoft(
                        HairProfile(),
                        Section(-0.5f, 0.9f, 0.94f),
                        Section(-0.12f, 1f, 1f),
                        Section(0.25f, 0.98f, 1f),
                        Section(0.5f, 0.84f, 0.94f))
                    .ColorRole("hair"))
                .Shape("hero.hair.lock.left", "Left Long Side Lock", LowPolyShapeTypes.ExtrudedProfile, hair => hair
                    .Position(-0.29f, 0.14f, -0.255f).Rotation(0f, 0f, 6f).Scale(0.15f, 0.54f, 1f)
                    .ExtrudedProfile(0.07f, 0.018f,
                        new(-0.5f, 0.5f), new(0.5f, 0.38f), new(0.12f, -0.5f), new(-0.46f, -0.16f))
                    .ColorRole("hair.highlight"))
                .Shape("hero.hair.lock.right", "Right Long Side Lock", LowPolyShapeTypes.ExtrudedProfile, hair => hair
                    .Position(0.29f, 0.14f, -0.255f).Rotation(0f, 0f, -6f).Scale(0.15f, 0.54f, 1f)
                    .ExtrudedProfile(0.07f, 0.018f,
                        new(-0.5f, 0.38f), new(0.5f, 0.5f), new(0.46f, -0.16f), new(-0.12f, -0.5f))
                    .ColorRole("hair.highlight")));
        }

        private static void AddArm(ShapeNodeBuilder hero, string side, string label, float x)
        {
            string prefix = $"hero.arm.{side}";
            hero.Group($"{prefix}.shoulder.pivot", $"{label} Shoulder Pivot", shoulder => shoulder
                .Position(x, 1.53f, 0f)
                .Shape($"{prefix}.upper", $"{label} Jacket Sleeve", LowPolyShapeTypes.Frustum, sleeve => sleeve
                    .Position(0f, -0.2f, 0f).Scale(0.22f, 0.4f, 0.24f)
                    .Frustum(1f, 1f, 0.78f, 0.8f).ColorRole("jacket"))
                .Group($"{prefix}.elbow.pivot", $"{label} Elbow Pivot", elbow => elbow
                    .Position(0f, -0.42f, 0f)
                    .Shape($"{prefix}.forearm", $"{label} Bare Forearm", LowPolyShapeTypes.Frustum, arm => arm
                        .Position(0f, -0.2f, 0f).Scale(0.16f, 0.38f, 0.17f)
                        .Frustum(1f, 1f, 0.78f, 0.8f).ColorRole("skin"))
                    .Shape($"{prefix}.glove", $"{label} Fingerless Glove", LowPolyShapeTypes.Frustum, glove => glove
                        .Position(0f, -0.42f, 0f).Scale(0.18f, 0.2f, 0.18f)
                        .Frustum(1f, 1f, 0.82f, 0.82f).ColorRole("glove"))
                    .Shape($"{prefix}.hand", $"{label} Hand", LowPolyShapeTypes.Capsule, hand => hand
                        .Position(0f, -0.56f, -0.01f).Scale(0.09f, 0.15f, 0.08f).ColorRole("skin"))));
        }

        private static void AddLeg(ShapeNodeBuilder hero, string side, string label, float x)
        {
            string prefix = $"hero.leg.{side}";
            hero.Group($"{prefix}.hip.pivot", $"{label} Hip Pivot", hip => hip
                .Position(x, 0.95f, 0f)
                .Shape($"{prefix}.thigh", $"{label} Baggy Shorts", LowPolyShapeTypes.ProfileLoft, shorts => shorts
                    .Position(0f, -0.3f, 0f).Scale(0.34f, 0.56f, 0.38f)
                    .ProfileLoft(
                        BaggyLegProfile(),
                        Section(-0.5f, 0.92f, 0.96f),
                        Section(0f, 1f, 1f),
                        Section(0.5f, 0.82f, 0.92f))
                    .ColorRole("trousers"))
                .Group($"{prefix}.knee.pivot", $"{label} Knee Pivot", knee => knee
                    .Position(0f, -0.6f, 0f)
                    .Shape($"{prefix}.knee", $"{label} Exposed Knee", LowPolyShapeTypes.Sphere, joint => joint
                        .Position(0f, -0.02f, -0.01f).Scale(0.1f, 0.08f, 0.11f).ColorRole("skin"))
                    .Shape($"{prefix}.boot.shaft", $"{label} Tall Boot", LowPolyShapeTypes.ProfileLoft, boot => boot
                        .Position(0f, -0.31f, 0f).Scale(0.24f, 0.55f, 0.28f)
                        .ProfileLoft(
                            BoxProfile(),
                            Section(-0.5f, 0.9f, 1f),
                            Section(0f, 1f, 0.92f),
                            Section(0.5f, 0.78f, 0.82f))
                        .ColorRole("boot"))
                    .Shape($"{prefix}.boot", $"{label} Boot Foot", LowPolyShapeTypes.ProfileLoft, boot => boot
                        .Position(0f, -0.63f, -0.11f).Scale(0.28f, 0.22f, 0.48f)
                        .ProfileLoft(
                            BoxProfile(),
                            Section(-0.5f, 1f, 0.78f, 0f, -0.08f),
                            Section(0.05f, 0.88f, 1f),
                            Section(0.5f, 0.72f, 0.84f))
                        .ColorRole("boot"))));
        }

        private static ForgeVector2[] BoxProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, -0.5f), new(-0.5f, -0.5f)
            };
        }

        private static ForgeVector2[] TorsoProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.46f, 0.5f), new(0.46f, 0.5f), new(0.36f, -0.5f), new(-0.36f, -0.5f)
            };
        }

        private static ForgeVector2[] FaceProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.34f, 0.5f), new(0.34f, 0.5f), new(0.5f, 0.28f), new(0.46f, -0.24f),
                new(0.24f, -0.5f), new(-0.24f, -0.5f), new(-0.46f, -0.24f), new(-0.5f, 0.28f)
            };
        }

        private static ForgeVector2[] HairProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.5f, -0.02f), new(-0.48f, 0.3f), new(-0.3f, 0.5f), new(0.08f, 0.55f),
                new(0.4f, 0.42f), new(0.5f, 0.14f), new(0.4f, -0.12f), new(0.26f, -0.04f),
                new(0.16f, -0.3f), new(0.02f, -0.08f), new(-0.12f, -0.34f),
                new(-0.2f, -0.06f), new(-0.36f, -0.27f), new(-0.32f, -0.02f)
            };
        }

        private static ForgeVector2[] BaggyLegProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.4f, 0.5f), new(0.4f, 0.5f), new(0.5f, 0.08f), new(0.34f, -0.5f),
                new(-0.34f, -0.5f), new(-0.5f, 0.08f)
            };
        }

        private static ShapeProfileSection Section(
            float z,
            float scaleX,
            float scaleY,
            float offsetX = 0f,
            float offsetY = 0f)
        {
            return new(z, new(scaleX, scaleY), new(offsetX, offsetY));
        }
    }
}
