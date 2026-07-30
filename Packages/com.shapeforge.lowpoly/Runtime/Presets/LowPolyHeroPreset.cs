namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Provides a rounded pocket-fantasy character assembled from readable procedural shapes.
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
                    AddTorso(hero);
                    AddHead(hero);
                    AddArm(hero, "left", "Left", -0.43f);
                    AddArm(hero, "right", "Right", 0.43f);
                    AddLeg(hero, "left", "Left", -0.17f);
                    AddLeg(hero, "right", "Right", 0.17f);
                })
                .Build();
        }

        /// <summary>Creates the blue-black modern-fantasy palette used by the character.</summary>
        public static ShapeStyleDefinition CreateStyle()
        {
            ShapeStyleDefinition style = new(StyleId);
            style.Palette
                .Set("skin", new(0.82f, 0.58f, 0.43f))
                .Set("skin.shadow", new(0.66f, 0.41f, 0.3f))
                .Set("hair", new(0.018f, 0.035f, 0.065f))
                .Set("hair.light", new(0.035f, 0.07f, 0.12f))
                .Set("shirt", new(0.008f, 0.01f, 0.014f))
                .Set("jacket", new(0.025f, 0.03f, 0.038f))
                .Set("jacket.light", new(0.055f, 0.06f, 0.072f))
                .Set("pants", new(0.035f, 0.04f, 0.05f))
                .Set("glove", new(0.016f, 0.019f, 0.025f))
                .Set("boot", new(0.012f, 0.016f, 0.024f))
                .Set("metal", new(0.16f, 0.18f, 0.2f))
                .Set("eye", new(0.018f, 0.015f, 0.014f));
            return style;
        }

        private static void AddTorso(ShapeNodeBuilder hero)
        {
            hero.Group("hero.pelvis.pivot", "Pelvis Pivot", pelvis => pelvis
                .Position(0f, 1.43f, 0f)
                .Shape("hero.pelvis", "Narrow Trouser Waist", LowPolyShapeTypes.ProfileLoft, waist => waist
                    .Position(0f, 0.02f, 0f)
                    .Scale(0.42f, 0.24f, 0.34f)
                    .ProfileLoft(
                        RoundedBoxProfile(),
                        Section(-0.5f, 0.88f, 0.9f),
                        Section(0f, 1f, 1f),
                        Section(0.5f, 0.9f, 0.94f))
                    .LoftQuality(2, true)
                    .ProfileSmoothing(1)
                    .ColorRole("pants"))
                .Group("hero.spine.pivot", "Spine Pivot", spine => spine
                    .Position(0f, 0.03f, 0f)
                    .Shape("hero.shirt", "Long Black Shirt", LowPolyShapeTypes.ProfileLoft, shirt => shirt
                        .Position(0f, 0.43f, 0.015f)
                        .Scale(0.49f, 0.78f, 0.34f)
                        .ProfileLoft(
                            TorsoProfile(),
                            Section(-0.5f, 0.86f, 0.94f),
                            Section(0f, 1f, 1f),
                            Section(0.5f, 0.9f, 0.96f))
                        .LoftQuality(3, true)
                        .ProfileSmoothing(1)
                        .ColorRole("shirt"))
                    .Shape("hero.jacket.left", "Left Cropped Jacket", LowPolyShapeTypes.ExtrudedProfile, panel => panel
                        .Position(-0.21f, 0.51f, -0.205f)
                        .Scale(0.31f, 0.62f, 1f)
                        .ExtrudedProfile(0.095f, 0.018f,
                            new(-0.5f, 0.46f), new(0.42f, 0.5f), new(0.5f, 0.1f),
                            new(0.2f, -0.5f), new(-0.48f, -0.4f))
                        .ColorRole("jacket"))
                    .Shape("hero.jacket.right", "Right Cropped Jacket", LowPolyShapeTypes.ExtrudedProfile, panel => panel
                        .Position(0.21f, 0.51f, -0.205f)
                        .Scale(0.31f, 0.62f, 1f)
                        .ExtrudedProfile(0.095f, 0.018f,
                            new(-0.42f, 0.5f), new(0.5f, 0.46f), new(0.48f, -0.4f),
                            new(-0.2f, -0.5f), new(-0.5f, 0.1f))
                        .ColorRole("jacket"))
                    .Shape("hero.collar.left", "Left Standing Collar", LowPolyShapeTypes.ProfileSweep, collar => collar
                        .Position(0f, 0f, -0.205f)
                        .ProfileSweep(CollarProfile(), new ForgeVector3[]
                        {
                            new(-0.04f, 0.76f, 0f), new(-0.13f, 0.84f, 0.01f), new(-0.22f, 0.78f, 0.02f)
                        })
                        .SweepQuality(1, true)
                        .ColorRole("jacket.light"))
                    .Shape("hero.collar.right", "Right Standing Collar", LowPolyShapeTypes.ProfileSweep, collar => collar
                        .Position(0f, 0f, -0.205f)
                        .ProfileSweep(CollarProfile(), new ForgeVector3[]
                        {
                            new(0.04f, 0.76f, 0f), new(0.13f, 0.84f, 0.01f), new(0.22f, 0.78f, 0.02f)
                        })
                        .SweepQuality(1, true)
                        .ColorRole("jacket.light"))
                    .Shape("hero.jacket.pocket.left", "Left Jacket Pocket", LowPolyShapeTypes.ExtrudedProfile, pocket => pocket
                        .Position(-0.24f, 0.5f, -0.265f)
                        .Scale(0.15f, 0.12f, 1f)
                        .ExtrudedProfile(0.025f, 0.005f,
                            new(-0.5f, 0.45f), new(0.5f, 0.45f), new(0.4f, -0.5f), new(-0.4f, -0.5f))
                        .ColorRole("jacket.light")
                        .Mirror(ShapeMirrorAxis.X))));
        }

        private static void AddHead(ShapeNodeBuilder hero)
        {
            hero.Group("hero.head.pivot", "Head Pivot", head =>
            {
                head.Position(0f, 2.34f, 0f)
                    .Scale(0.88f, 0.88f, 0.88f)
                .Shape("hero.neck", "Short Neck", LowPolyShapeTypes.LatheProfile, neck => neck
                    .LatheProfile(16, true,
                        new(0.09f, -0.12f), new(0.12f, -0.04f), new(0.12f, 0.12f), new(0.09f, 0.18f))
                    .ProfileSmoothing(1)
                    .ColorRole("skin.shadow"))
                .Shape("hero.head", "Sculpted Human Head", LowPolyShapeTypes.LatheProfile, face => face
                    .Position(0f, 0.39f, 0f)
                    .Scale(0.82f, 0.86f, 0.68f)
                    .LatheProfile(24, true,
                        new(0.18f, -0.5f), new(0.34f, -0.43f), new(0.47f, -0.25f),
                        new(0.51f, 0.06f), new(0.48f, 0.3f), new(0.38f, 0.47f),
                        new(0.16f, 0.55f), new(0f, 0.57f))
                    .ProfileSmoothing(1)
                    .ColorRole("skin"))
                .Shape("hero.eye.left", "Left Shadowed Eye", LowPolyShapeTypes.Sphere, eye => eye
                    .Position(-0.145f, 0.41f, -0.355f)
                    .Scale(0.035f, 0.022f, 0.012f)
                    .ColorRole("eye")
                    .Mirror(ShapeMirrorAxis.X))
                .Shape("hero.hair", "Unified Polygon Hair Shell", LowPolyShapeTypes.ProfileLoft, hair => hair
                    .Position(0f, 0.47f, -0.005f)
                    .Scale(0.98f, 0.98f, 0.76f)
                    .ProfileLoft(
                        HairShellProfile(),
                        Section(-0.5f, 0.84f, 0.94f, 0f, -0.01f),
                        Section(-0.15f, 1f, 1f),
                        Section(0.22f, 0.98f, 1f),
                        Section(0.5f, 0.78f, 0.9f))
                    .LoftQuality(3, true)
                    .ColorRole("hair"));
            });
        }

        private static void AddArm(ShapeNodeBuilder hero, string side, string label, float x)
        {
            string prefix = $"hero.arm.{side}";
            hero.Group($"{prefix}.shoulder.pivot", $"{label} Shoulder Pivot", shoulder => shoulder
                .Position(x, 2.1f, 0f)
                .Shape($"{prefix}.sleeve", $"{label} Rolled Jacket Sleeve", LowPolyShapeTypes.LatheProfile, sleeve => sleeve
                    .Position(0f, -0.22f, 0f)
                    .Scale(1f, 1f, 0.92f)
                    .LatheProfile(12, true,
                        new(0.135f, -0.2f), new(0.16f, -0.1f), new(0.155f, 0.16f), new(0.12f, 0.23f))
                    .ProfileSmoothing(1)
                    .ColorRole("jacket"))
                .Group($"{prefix}.elbow.pivot", $"{label} Elbow Pivot", elbow => elbow
                    .Position(0f, -0.43f, 0f)
                    .Shape($"{prefix}.forearm", $"{label} Natural Forearm", LowPolyShapeTypes.LatheProfile, arm => arm
                        .Position(0f, -0.22f, 0f)
                        .LatheProfile(12, true,
                            new(0.095f, -0.25f), new(0.12f, -0.14f), new(0.13f, 0.16f), new(0.105f, 0.25f))
                        .ProfileSmoothing(1)
                        .ColorRole("skin"))
                    .Shape($"{prefix}.glove", $"{label} Fingerless Glove", LowPolyShapeTypes.LatheProfile, glove => glove
                        .Position(0f, -0.47f, 0f)
                        .LatheProfile(12, true,
                            new(0.1f, -0.1f), new(0.13f, -0.04f), new(0.135f, 0.12f), new(0.11f, 0.16f))
                        .ProfileSmoothing(1)
                        .ColorRole("glove"))
                    .Shape($"{prefix}.hand", $"{label} Relaxed Hand", LowPolyShapeTypes.Capsule, hand => hand
                        .Position(0f, -0.65f, -0.015f)
                        .Scale(0.105f, 0.17f, 0.085f)
                        .ColorRole("skin"))));
        }

        private static void AddLeg(ShapeNodeBuilder hero, string side, string label, float x)
        {
            string prefix = $"hero.leg.{side}";
            hero.Group($"{prefix}.hip.pivot", $"{label} Hip Pivot", hip => hip
                .Position(x, 1.42f, 0f)
                .Shape($"{prefix}.pants", $"{label} Loose Cropped Trouser Leg", LowPolyShapeTypes.ProfileLoft, pants => pants
                    .Position(0f, -0.3f, 0f)
                    .Scale(0.34f, 0.58f, 0.4f)
                    .ProfileLoft(
                        BaggyLegProfile(),
                        Section(-0.5f, 0.9f, 0.94f),
                        Section(-0.08f, 1f, 1f),
                        Section(0.5f, 0.82f, 0.9f))
                    .LoftQuality(3, true)
                    .ColorRole("pants"))
                .Group($"{prefix}.knee.pivot", $"{label} Knee Pivot", knee => knee
                    .Position(0f, -0.61f, 0f)
                    .Shape($"{prefix}.knee", $"{label} Exposed Knee", LowPolyShapeTypes.Sphere, joint => joint
                        .Position(0f, -0.02f, -0.035f)
                        .Scale(0.105f, 0.085f, 0.12f)
                        .ColorRole("skin"))
                    .Shape($"{prefix}.boot.shaft", $"{label} Fitted Tall Boot", LowPolyShapeTypes.ProfileLoft, boot => boot
                        .Position(0f, -0.32f, 0f)
                        .Scale(0.23f, 0.54f, 0.27f)
                        .ProfileLoft(
                            BootShaftProfile(),
                            Section(-0.5f, 0.9f, 0.92f),
                            Section(0f, 1f, 1f),
                            Section(0.5f, 0.74f, 0.82f))
                        .LoftQuality(3, true)
                        .ProfileSmoothing(1)
                        .ColorRole("boot"))
                    .Shape($"{prefix}.boot", $"{label} Tapered Boot", LowPolyShapeTypes.ProfileLoft, boot => boot
                        .Position(0f, -0.62f, -0.1f)
                        .Scale(0.25f, 0.22f, 0.48f)
                        .ProfileLoft(
                            BootFootProfile(),
                            Section(-0.5f, 0.94f, 0.82f, 0f, -0.06f),
                            Section(0f, 1f, 1f),
                            Section(0.5f, 0.68f, 0.78f))
                        .LoftQuality(2, true)
                        .ProfileSmoothing(1)
                        .ColorRole("boot"))));
        }

        private static ForgeVector2[] RoundedBoxProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.38f, 0.5f), new(0.38f, 0.5f), new(0.5f, 0.34f), new(0.5f, -0.34f),
                new(0.38f, -0.5f), new(-0.38f, -0.5f), new(-0.5f, -0.34f), new(-0.5f, 0.34f)
            };
        }

        private static ForgeVector2[] TorsoProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.34f, 0.5f), new(0.34f, 0.5f), new(0.48f, 0.3f), new(0.42f, -0.5f),
                new(-0.42f, -0.5f), new(-0.48f, 0.3f)
            };
        }

        private static ForgeVector2[] HairShellProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.46f, -0.44f), new(-0.54f, -0.1f), new(-0.55f, 0.25f),
                new(-0.45f, 0.5f), new(-0.24f, 0.67f), new(-0.08f, 0.73f),
                new(0.22f, 0.65f), new(0.45f, 0.48f), new(0.56f, 0.18f),
                new(0.52f, -0.2f), new(0.42f, -0.47f), new(0.36f, -0.25f),
                new(0.3f, 0.08f), new(-0.08f, -0.3f), new(-0.15f, 0.16f),
                new(-0.27f, -0.06f), new(-0.4f, -0.02f)
            };
        }

        private static ForgeVector2[] BaggyLegProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.32f, 0.5f), new(0.32f, 0.5f), new(0.48f, 0.24f), new(0.5f, -0.14f),
                new(0.3f, -0.5f), new(-0.3f, -0.5f), new(-0.5f, -0.14f), new(-0.48f, 0.24f)
            };
        }

        private static ForgeVector2[] BootShaftProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.34f, 0.5f), new(0.34f, 0.5f), new(0.46f, 0.28f), new(0.38f, -0.5f),
                new(-0.38f, -0.5f), new(-0.46f, 0.28f)
            };
        }

        private static ForgeVector2[] BootFootProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.34f, 0.5f), new(0.34f, 0.5f), new(0.5f, 0.22f), new(0.4f, -0.5f),
                new(-0.4f, -0.5f), new(-0.5f, 0.22f)
            };
        }

        private static ForgeVector2[] CollarProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.022f, -0.035f), new(0.022f, -0.035f), new(0.03f, 0.025f), new(0f, 0.045f),
                new(-0.03f, 0.025f)
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
