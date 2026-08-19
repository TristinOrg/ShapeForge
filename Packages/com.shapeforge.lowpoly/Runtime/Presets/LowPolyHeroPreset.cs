namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Provides an authored pocket-fantasy character with articulated semantic pivots.
    /// </summary>
    public static class LowPolyHeroPreset
    {
        private const float HeadAssemblyScale = 0.67f;
        private const string HairReferencePartId = "character/hair";
        private const string HeadReferencePartId = "character/head";
        private const float ReferenceHeight   = 3.5f;

        /// <summary>Gets the style identifier used by the hero preset.</summary>
        public const string StyleId = "lowpoly/fantasy-hero";

        /// <summary>Creates the authored pocket fantasy hero definition.</summary>
        public static ShapeDefinition CreateDefinition()
        {
            return LowPolyStylizedHumanTemplate.Instance.Compile(new());
        }

        internal static ShapeDefinition CreateDefinition(
            LowPolyStylizedHumanSpecification specification,
            ShapeReferenceDefinition          reference)
        {
            LowPolyStylizedHumanProportions proportions = specification.Proportions;
            LowPolyReferenceProfileCage     headCage;
            LowPolyReferenceProfileCage     hairCage;
            if (reference == null)
            {
                headCage = CreateAuthoredHeadCage();
                hairCage = CreateAuthoredHairCage();
            }
            else
            {
                LowPolyReferenceProfileCageMapper mapper = new();
                headCage = mapper.Map(reference, HeadReferencePartId, 24, 9);
                hairCage = mapper.Map(reference, HairReferencePartId, 32, 11);
            }

            return ShapeBuilder
                .Create(specification.Name)
                .WithStyle(specification.Style)
                .WithRig("humanoid/basic",
                    new ShapeRigJoint(ShapeRigRoles.Root, "hero"),
                    Joint(ShapeRigRoles.Hips, "hero.pelvis.pivot", -10f, 10f, -15f, 15f, -10f, 10f),
                    Joint(ShapeRigRoles.Spine, "hero.spine.pivot", -15f, 20f, -25f, 25f, -12f, 12f),
                    Joint(ShapeRigRoles.Head, "hero.head.pivot", -25f, 35f, -60f, 60f, -25f, 25f),
                    Joint(ShapeRigRoles.LeftShoulder, "hero.arm.left.shoulder.pivot",
                        -70f, 70f, -35f, 35f, -25f, 110f),
                    Joint(ShapeRigRoles.LeftElbow, "hero.arm.left.elbow.pivot", -10f, 120f, 0f, 0f, 0f, 0f),
                    Joint(ShapeRigRoles.RightShoulder, "hero.arm.right.shoulder.pivot",
                        -70f, 70f, -35f, 35f, -110f, 25f),
                    Joint(ShapeRigRoles.RightElbow, "hero.arm.right.elbow.pivot", -10f, 120f, 0f, 0f, 0f, 0f),
                    Joint(ShapeRigRoles.LeftHip, "hero.leg.left.hip.pivot", -45f, 45f, -20f, 20f, -15f, 15f),
                    Joint(ShapeRigRoles.LeftKnee, "hero.leg.left.knee.pivot", -120f, 5f, 0f, 0f, 0f, 0f),
                    Joint(ShapeRigRoles.RightHip, "hero.leg.right.hip.pivot", -45f, 45f, -20f, 20f, -15f, 15f),
                    Joint(ShapeRigRoles.RightKnee, "hero.leg.right.knee.pivot", -120f, 5f, 0f, 0f, 0f, 0f))
                .Root("hero", specification.Name, hero =>
                {
                    hero.Scale(specification.OverallScale, specification.OverallScale, specification.OverallScale);
                    AddBody(hero, proportions, specification.Outfit);
                    AddHead(
                        hero, headCage, hairCage, proportions.HeadScale,
                        specification.Head, specification.Face, specification.Hair);
                    float shoulderX = 0.36f * proportions.ShoulderWidth * proportions.BodyWidth;
                    AddArm(hero, "left", "Left", -shoulderX, specification.Outfit);
                    AddArm(hero, "right", "Right", shoulderX, specification.Outfit);
                    AddLeg(hero, "left", "Left", -0.18f * proportions.BodyWidth, proportions.LegLength, specification.Outfit);
                    AddLeg(hero, "right", "Right", 0.18f * proportions.BodyWidth, proportions.LegLength, specification.Outfit);
                })
                .Build();
        }

        /// <summary>Creates the measured blue-black and warm-skin reference palette.</summary>
        public static ShapeStyleDefinition CreateStyle()
        {
            ShapeStyleDefinition style = new(StyleId);
            style.Palette
                .Set("skin", new(0.941f, 0.843f, 0.761f))
                .Set("skin.shadow", new(0.78f, 0.61f, 0.5f))
                .Set("hair", new(0.11f, 0.176f, 0.267f))
                .Set("hair.light", new(0.18f, 0.278f, 0.408f))
                .Set("hair.shadow", new(0.067f, 0.102f, 0.157f))
                .Set("shirt", new(0.102f, 0.11f, 0.129f))
                .Set("jacket", new(0.102f, 0.11f, 0.129f))
                .Set("jacket.light", new(0.15f, 0.16f, 0.18f))
                .Set("pants", new(0.082f, 0.09f, 0.106f))
                .Set("glove", new(0.067f, 0.075f, 0.09f))
                .Set("boot", new(0.055f, 0.059f, 0.071f))
                .Set("sole", new(0.69f, 0.165f, 0.184f))
                .Set("metal", new(0.28f, 0.3f, 0.33f))
                .Set("eye", new(0.055f, 0.075f, 0.09f))
                .Set("mouth", new(0.42f, 0.27f, 0.24f));
            return style;
        }

        private static void AddBody(
            ShapeNodeBuilder proportionsParent,
            LowPolyStylizedHumanProportions proportions,
            LowPolyStylizedHumanOutfit      outfit)
        {
            float bodyWidth = proportions.BodyWidth;
            float detail    = outfit.DetailScale;
            proportionsParent.Group("hero.pelvis.pivot", "Pelvis Pivot", pelvis => pelvis
                .Position(0f, 1.45f, 0f)
                .Shape("hero.pelvis", "Tailored Shorts Waist", LowPolyShapeTypes.ProfileCage, waist => waist
                    .Scale(0.58f * bodyWidth, 0.28f, 0.4f)
                    .ProfileCage(
                        Cage(-0.5f, RoundedProfile(0.8f, 0.86f)),
                        Cage(0f, RoundedProfile(1f, 1f)),
                        Cage(0.5f, RoundedProfile(0.84f, 0.9f)))
                    .CageQuality(2, 1, true)
                    .ColorRole("pants"))
                .Group("hero.spine.pivot", "Spine Pivot", spine => spine
                    .Position(0f, 0.02f, 0f)
                    .Shape("hero.shirt", "Fitted Black Shirt", LowPolyShapeTypes.ProfileCage, shirt => shirt
                        .Position(0f, 0.48f, 0f)
                        .Scale(0.7f * bodyWidth, 0.88f, 0.38f)
                        .ProfileCage(
                            Cage(-0.5f, TorsoProfile(0.86f)),
                            Cage(-0.12f, TorsoProfile(1f)),
                            Cage(0.5f, TorsoProfile(0.9f)))
                        .CageQuality(2, 1, true)
                        .ColorRole("shirt"))
                    .Shape("hero.jacket.left", "Left Open Short Jacket", LowPolyShapeTypes.ExtrudedProfile, panel => panel
                        .Position(-0.21f * bodyWidth, 0.53f, -0.23f)
                        .Scale(0.34f * bodyWidth, 0.7f, 1f)
                        .ExtrudedProfile(0.075f, 0.018f,
                            new(-0.5f, 0.34f), new(-0.34f, 0.5f), new(0.48f, 0.43f),
                            new(0.38f, -0.5f), new(-0.42f, -0.43f))
                        .ProfileSmoothing(1)
                        .ColorRole("jacket"))
                    .Shape("hero.jacket.right", "Right Open Short Jacket", LowPolyShapeTypes.ExtrudedProfile, panel => panel
                        .Position(0.21f * bodyWidth, 0.53f, -0.23f)
                        .Scale(0.34f * bodyWidth, 0.7f, 1f)
                        .ExtrudedProfile(0.075f, 0.018f,
                            new(-0.48f, 0.43f), new(0.34f, 0.5f), new(0.5f, 0.34f),
                            new(0.42f, -0.43f), new(-0.38f, -0.5f))
                        .ProfileSmoothing(1)
                        .ColorRole("jacket"))
                    .Shape("hero.collar.left", "Left Standing Collar", LowPolyShapeTypes.ExtrudedProfile, collar => collar
                        .Position(-0.12f, 0.88f, -0.19f)
                        .Rotation(-8f, 8f, -8f)
                        .Scale(0.17f, 0.22f, 1f)
                        .ExtrudedProfile(0.14f, 0.015f,
                            new(-0.5f, -0.5f), new(-0.32f, 0.48f), new(0.18f, 0.5f), new(0.5f, -0.34f))
                        .ColorRole("jacket.light")
                        .Mirror(ShapeMirrorAxis.X))
                    .Shape("hero.jacket.pocket.left", "Left Jacket Pocket", LowPolyShapeTypes.ExtrudedProfile, pocket => pocket
                        .Position(-0.25f, 0.55f, -0.285f)
                        .Scale(0.16f, 0.14f, 1f)
                        .ExtrudedProfile(0.02f, 0.004f,
                            new(-0.5f, 0.5f), new(0.5f, 0.42f), new(0.4f, -0.5f), new(-0.42f, -0.42f))
                        .ColorRole("jacket.light")
                        .Mirror(ShapeMirrorAxis.X))
                    .Shape("hero.hood", "Folded Back Hood", LowPolyShapeTypes.ProfileSweep, hood => hood
                        .ProfileSweep(SmallDiamondProfile(), new ForgeVector3[]
                        {
                            new(-0.34f, 0.78f, 0.16f), new(-0.24f, 0.92f, 0.2f),
                            new(0f, 0.98f, 0.23f), new(0.24f, 0.92f, 0.2f), new(0.34f, 0.78f, 0.16f)
                        })
                        .SweepQuality(2, true)
                        .ColorRole("jacket"))
                    .Shape("hero.pendant", "Chest Pendant", LowPolyShapeTypes.ExtrudedProfile, pendant => pendant
                        .Position(0f, 0.67f, -0.235f)
                        .Scale(0.08f, 0.11f, 1f)
                        .ExtrudedProfile(0.025f, 0.004f,
                            new(0f, 0.5f), new(0.45f, 0f), new(0f, -0.5f), new(-0.45f, 0f))
                        .ColorRole("metal"))
                    .Shape("hero.shirt.neckline", "Layered Shirt Neckline", LowPolyShapeTypes.ProfileSweep, neckline => neckline
                        .ProfileSweep(SmallDiamondProfile(), new ForgeVector3[]
                        {
                            new(-0.16f, 0.84f, -0.25f), new(0f, 0.77f, -0.27f), new(0.16f, 0.84f, -0.25f)
                        })
                        .Scale(detail, detail, detail)
                        .SweepQuality(1, true)
                        .ColorRole("jacket.light"))
                    .Shape("hero.jacket.epaulette.left", "Left Jacket Epaulette", LowPolyShapeTypes.ExtrudedProfile, tab => tab
                        .Position(-0.31f * bodyWidth, 0.84f, -0.08f)
                        .Rotation(78f, 0f, -8f)
                        .Scale(0.18f * detail, 0.1f * detail, 1f)
                        .ExtrudedProfile(0.045f, 0.006f,
                            new(-0.5f, 0.4f), new(0.35f, 0.5f), new(0.5f, -0.36f), new(-0.4f, -0.5f))
                        .ColorRole("jacket.light")
                        .Mirror(ShapeMirrorAxis.X))));
        }

        private static void AddHead(
            ShapeNodeBuilder              hero,
            LowPolyReferenceProfileCage   headCage,
            LowPolyReferenceProfileCage   hairCage,
            float                         headScale,
            LowPolyStylizedHumanHead      headShape,
            LowPolyStylizedHumanFace      faceShape,
            LowPolyStylizedHumanHair      hairShape)
        {
            hero.Group("hero.head.pivot", "Head Pivot", head =>
            {
                head.Position(0f, 2.35f, 0f)
                    .Scale(
                        headScale * HeadAssemblyScale,
                        headScale * HeadAssemblyScale,
                        headScale * HeadAssemblyScale)
                    .Shape("hero.neck", "Short Neck", LowPolyShapeTypes.LatheProfile, neck => neck
                        .Position(0f, 0.06f, 0.02f)
                        .LatheProfile(16, true,
                            new(0.09f, -0.14f), new(0.12f, -0.07f), new(0.13f, 0.12f), new(0.1f, 0.2f))
                        .ProfileSmoothing(1)
                        .ColorRole("skin.shadow"));

                AddReferenceCage(
                    head,
                    "hero.head",
                    "Reference Sculpted Head",
                    headCage,
                    2.35f,
                    headShape.Width,
                    headShape.Height,
                    headShape.Depth,
                    headShape.JawWidth,
                    "skin",
                    3,
                    1);
                AddEars(head, headShape);
                AddFaceDetails(head, faceShape);
                AddReferenceCage(
                    head,
                    "hero.hair",
                    "Reference Unified Hair Volume",
                    hairCage,
                    2.35f,
                    hairShape.Volume,
                    hairShape.Volume,
                    hairShape.Volume,
                    1f,
                    "hair",
                    3,
                    1);
                AddHairDetails(head, hairShape);
                AddLayeredHairSpikes(head, hairShape);
            });
        }

        private static void AddFaceDetails(ShapeNodeBuilder head, LowPolyStylizedHumanFace face)
        {
            float eyeX      = 0.205f * face.EyeSpacing;
            float eyeWidth  = 0.15f * face.EyeScale;
            float eyeHeight = 0.105f * face.EyeScale * face.EyeOpenness;
            head.Shape("hero.eye.left", "Left Stylized Eye", LowPolyShapeTypes.ExtrudedProfile, eye => eye
                    .Position(-eyeX, 0.43f, -0.448f)
                    .Rotation(0f, 0f, -5f)
                    .Scale(eyeWidth, eyeHeight, 1f)
                    .ExtrudedProfile(0.018f, 0.003f,
                        new(-0.5f, 0f), new(-0.28f, 0.42f), new(0.2f, 0.5f),
                        new(0.5f, 0.12f), new(0.24f, -0.48f), new(-0.28f, -0.42f))
                    .ColorRole("eye")
                    .Mirror(ShapeMirrorAxis.X))
                .Shape("hero.eye.glint.left", "Left Eye Glint", LowPolyShapeTypes.Sphere, glint => glint
                    .Position(-eyeX - (eyeWidth * 0.12f), 0.455f, -0.462f)
                    .Scale(0.023f * face.EyeScale, 0.018f * face.EyeScale, 0.009f)
                    .Color(0.72f, 0.82f, 0.94f)
                    .Mirror(ShapeMirrorAxis.X))
                .Shape("hero.mouth", "Subtle Chibi Mouth", LowPolyShapeTypes.ExtrudedProfile, mouth => mouth
                    .Position(0f, 0.205f, -0.472f)
                    .Scale(0.075f * face.MouthWidth, 0.025f, 1f)
                    .ExtrudedProfile(0.012f, 0.002f,
                        new(-0.5f, 0.12f), new(0f, -0.2f), new(0.5f, 0.12f), new(0f, 0.18f))
                    .ColorRole("mouth"));
        }

        private static void AddEars(ShapeNodeBuilder head, LowPolyStylizedHumanHead shape)
        {
            head.Shape("hero.ear.left", "Left Ear", LowPolyShapeTypes.Capsule, ear => ear
                    .Position(-0.435f * shape.Width, 0.5f * shape.Height, 0f)
                    .Scale(0.065f, 0.095f, 0.042f)
                    .ColorRole("skin.shadow")
                    .Mirror(ShapeMirrorAxis.X));
        }

        private static void AddHairDetails(ShapeNodeBuilder head, LowPolyStylizedHumanHair hair)
        {
            float partOffset = (hair.Parting - 0.7f) * 0.34f;
            float fringeDrop = (hair.FringeLength - 0.5f) * 0.2f;
            head.Shape("hero.hair.fringe.primary", "Diagonal Primary Fringe", LowPolyShapeTypes.ExtrudedProfile,
                    fringe => fringe
                        .Position(-0.08f + partOffset, 0.64f - fringeDrop, -0.47f)
                        .Rotation(0f, 0f, 10f)
                        .Scale(0.34f, 0.31f + fringeDrop, 1f)
                        .ExtrudedProfile(0.055f, 0.008f,
                            new(-0.5f, 0.4f), new(-0.22f, 0.5f), new(0.5f, 0.42f),
                            new(0.2f, -0.5f), new(-0.18f, -0.24f))
                        .ProfileSmoothing(1)
                        .ColorRole("hair.shadow"))
                .Shape("hero.hair.fringe.secondary", "Diagonal Secondary Fringe", LowPolyShapeTypes.ExtrudedProfile,
                    fringe => fringe
                        .Position(0.2f + partOffset, 0.66f - (fringeDrop * 0.7f), -0.465f)
                        .Rotation(0f, 0f, -9f)
                        .Scale(0.21f, 0.27f + fringeDrop, 1f)
                        .ExtrudedProfile(0.05f, 0.008f,
                            new(-0.5f, 0.42f), new(-0.12f, 0.5f), new(0.5f, 0.36f),
                            new(0.08f, -0.5f), new(-0.3f, -0.18f))
                        .ProfileSmoothing(1)
                        .ColorRole("hair"))
                .Shape("hero.hair.side.left", "Short Left Temple Lock", LowPolyShapeTypes.ExtrudedProfile, lockShape =>
                    lockShape
                        .Position(-0.43f, 0.48f, -0.3f)
                        .Rotation(0f, -12f, -4f)
                        .Scale(0.11f, 0.2f + (hair.SideburnLength * 0.06f), 1f)
                        .ExtrudedProfile(0.08f, 0.008f,
                            new(-0.5f, 0.5f), new(0.48f, 0.4f), new(0.12f, -0.5f), new(-0.28f, -0.12f))
                        .ColorRole("hair.shadow")
                        .Mirror(ShapeMirrorAxis.X))
                .Shape("hero.hair.highlight", "Hair Highlight Plane", LowPolyShapeTypes.ExtrudedProfile, highlight =>
                    highlight
                        .Position(-0.18f, 0.88f, -0.39f)
                        .Rotation(-8f, 0f, 16f)
                        .Scale(0.18f, 0.28f, 1f)
                        .ExtrudedProfile(0.025f, 0.004f,
                            new(-0.5f, 0.5f), new(0.18f, 0.38f), new(0.5f, -0.5f), new(-0.08f, -0.2f))
                        .ColorRole("hair.light"));
        }

        private static void AddLayeredHairSpikes(ShapeNodeBuilder head, LowPolyStylizedHumanHair hair)
        {
            float length = hair.SpikeLength;
            float back   = hair.BackSpikeVolume;
            AddHairSpike(head, "crown-left", "Crown Left Spike", new(-0.16f, 1.03f, 0.02f),
                new(-8f, -18f, 22f), new(0.18f, 0.31f * length, 0.14f), "hair.light");
            AddHairSpike(head, "crown-center", "Crown Center Spike", new(0.04f, 1.1f, 0.04f),
                new(-12f, 4f, -5f), new(0.17f, 0.36f * length, 0.15f), "hair");
            AddHairSpike(head, "crown-right", "Crown Right Spike", new(0.25f, 1.01f, 0.06f),
                new(-10f, 18f, -25f), new(0.17f, 0.3f * length, 0.14f), "hair.shadow");

            AddHairSpike(head, "side-left-upper", "Left Upper Side Spike", new(-0.47f, 0.84f, 0.05f),
                new(0f, -72f, 68f), new(0.16f, 0.3f * length, 0.14f), "hair");
            AddHairSpike(head, "side-left-lower", "Left Lower Side Spike", new(-0.51f, 0.62f, 0.12f),
                new(5f, -82f, 82f), new(0.15f, 0.27f * length, 0.13f), "hair.shadow");
            AddHairSpike(head, "side-right-upper", "Right Upper Side Spike", new(0.5f, 0.84f, 0.06f),
                new(0f, 72f, -68f), new(0.17f, 0.32f * length, 0.14f), "hair");
            AddHairSpike(head, "side-right-lower", "Right Lower Side Spike", new(0.53f, 0.61f, 0.13f),
                new(5f, 82f, -82f), new(0.15f, 0.28f * length, 0.13f), "hair.shadow");

            AddHairSpike(head, "back-top", "Back Top Spike", new(0.12f, 0.96f, 0.4f * back),
                new(72f, 8f, -8f), new(0.18f, 0.34f * length, 0.16f), "hair");
            AddHairSpike(head, "back-left", "Back Left Spike", new(-0.3f, 0.78f, 0.43f * back),
                new(78f, -34f, 28f), new(0.18f, 0.33f * length, 0.15f), "hair.shadow");
            AddHairSpike(head, "back-right", "Back Right Spike", new(0.34f, 0.76f, 0.44f * back),
                new(78f, 38f, -30f), new(0.19f, 0.36f * length, 0.16f), "hair");
            AddHairSpike(head, "back-left-lower", "Back Left Lower Spike", new(-0.24f, 0.53f, 0.43f * back),
                new(88f, -28f, 42f), new(0.16f, 0.29f * length, 0.14f), "hair.shadow");
            AddHairSpike(head, "back-right-lower", "Back Right Lower Spike", new(0.26f, 0.5f, 0.44f * back),
                new(88f, 30f, -44f), new(0.17f, 0.31f * length, 0.14f), "hair.shadow");
        }

        private static void AddHairSpike(
            ShapeNodeBuilder head,
            string           id,
            string           name,
            ForgeVector3     position,
            ForgeVector3     rotation,
            ForgeVector3     scale,
            string           colorRole)
        {
            head.Shape($"hero.hair.spike.{id}", name, LowPolyShapeTypes.ExtrudedProfile, spike => spike
                .Position(position.X, position.Y, position.Z)
                .Rotation(rotation.X, rotation.Y, rotation.Z)
                .Scale(scale.X, scale.Y, scale.Z)
                .ExtrudedProfile(0.12f, 0.012f,
                    new(-0.5f, 0.42f), new(-0.2f, 0.5f), new(0.5f, -0.5f), new(-0.32f, -0.12f))
                .ProfileSmoothing(1)
                .ColorRole(colorRole));
        }

        private static void AddReferenceCage(
            ShapeNodeBuilder            parent,
            string                      id,
            string                      name,
            LowPolyReferenceProfileCage cage,
            float                       parentGlobalY,
            float                       width,
            float                       height,
            float                       depth,
            float                       lowerWidth,
            string                      colorRole,
            int                         subdivisions,
            int                         smoothing)
        {
            parent.Shape(id, name, LowPolyShapeTypes.ProfileCage, shape =>
            {
                shape.Position(
                        cage.Position.X * ReferenceHeight,
                        (cage.Position.Y * ReferenceHeight) - parentGlobalY,
                        cage.Position.Z * ReferenceHeight)
                    .Scale(
                        cage.Scale.X * ReferenceHeight * width,
                        cage.Scale.Y * ReferenceHeight * height,
                        cage.Scale.Z * ReferenceHeight * depth)
                    .ColorRole(colorRole);
                foreach (ShapeProfileCageSection section in cage.Sections)
                {
                    ForgeVector2[] profile = new ForgeVector2[section.Profile.Count];
                    for (int index = 0; index < profile.Length; index++)
                    {
                        ForgeVector2 point = section.Profile[index];
                        float jawScale     = point.Y < 0f ? lowerWidth : 1f;
                        profile[index]     = new(point.X * jawScale, point.Y);
                    }

                    shape.ProfileCageSection(section.Z, profile);
                }

                shape.CageQuality(subdivisions, smoothing, true);
            });
        }

        private static void AddArm(
            ShapeNodeBuilder hero,
            string           side,
            string           label,
            float            x,
            LowPolyStylizedHumanOutfit outfit)
        {
            string prefix = $"hero.arm.{side}";
            hero.Group($"{prefix}.shoulder.pivot", $"{label} Shoulder Pivot", shoulder => shoulder
                .Position(x, 2.16f, 0f)
                .Shape($"{prefix}.sleeve", $"{label} Layered Short Sleeve", LowPolyShapeTypes.ProfileLoft, sleeve => sleeve
                    .Position(0f, -0.17f, 0f)
                    .Scale(0.24f, 0.32f, 0.27f)
                    .ProfileLoft(
                        RoundedProfile(1f, 1f),
                        Section(-0.5f, 0.78f, 0.82f),
                        Section(0f, 1f, 1f),
                        Section(0.5f, 0.82f, 0.86f))
                    .LoftQuality(2, true)
                    .ProfileSmoothing(1)
                    .ColorRole("jacket"))
                .Group($"{prefix}.elbow.pivot", $"{label} Elbow Pivot", elbow => elbow
                    .Position(0f, -0.42f, 0f)
                    .Shape($"{prefix}.forearm", $"{label} Tapered Forearm", LowPolyShapeTypes.LatheProfile, arm => arm
                        .Position(0f, -0.27f, 0f)
                        .LatheProfile(16, true,
                            new(0.08f, -0.3f), new(0.105f, -0.18f), new(0.118f, 0.16f), new(0.1f, 0.3f))
                        .ProfileSmoothing(1)
                        .ColorRole("skin"))
                    .Shape($"{prefix}.sleeve.band", $"{label} Sleeve Band", LowPolyShapeTypes.LatheProfile, band => band
                        .Position(0f, -0.04f, 0f)
                        .Scale(outfit.DetailScale, outfit.DetailScale, outfit.DetailScale)
                        .LatheProfile(12, true,
                            new(0.12f, -0.035f), new(0.125f, 0f), new(0.12f, 0.035f))
                        .ColorRole("jacket.light"))
                    .Shape($"{prefix}.glove", $"{label} Fingerless Glove", LowPolyShapeTypes.LatheProfile, glove => glove
                        .Position(0f, -0.56f, 0f)
                        .LatheProfile(16, true,
                            new(0.095f, -0.11f), new(0.13f, -0.04f), new(0.132f, 0.13f), new(0.105f, 0.17f))
                        .ProfileSmoothing(1)
                        .ColorRole("glove"))
                    .Shape($"{prefix}.glove.cuff", $"{label} Glove Wrist Cuff", LowPolyShapeTypes.LatheProfile, cuff => cuff
                        .Position(0f, -0.48f, 0f)
                        .LatheProfile(12, true,
                            new(0.115f, -0.045f), new(0.14f * outfit.DetailScale, 0f), new(0.115f, 0.045f))
                        .ColorRole("glove"))
                    .Shape($"{prefix}.hand", $"{label} Relaxed Hand", LowPolyShapeTypes.Capsule, hand => hand
                        .Position(0f, -0.76f, -0.01f)
                        .Scale(0.095f, 0.17f, 0.08f)
                        .ColorRole("skin"))
                    .Shape($"{prefix}.finger.outer", $"{label} Exposed Outer Fingers", LowPolyShapeTypes.Capsule, finger => finger
                        .Position(-0.055f, -0.82f, -0.035f)
                        .Scale(0.026f, 0.085f, 0.025f)
                        .Rotation(8f, 0f, -4f)
                        .ColorRole("skin"))
                    .Shape($"{prefix}.finger.inner", $"{label} Exposed Inner Fingers", LowPolyShapeTypes.Capsule, finger => finger
                        .Position(0.055f, -0.82f, -0.035f)
                        .Scale(0.026f, 0.08f, 0.025f)
                        .Rotation(6f, 0f, 4f)
                        .ColorRole("skin"))));
        }

        private static void AddLeg(
            ShapeNodeBuilder hero,
            string           side,
            string           label,
            float            x,
            float            legLength,
            LowPolyStylizedHumanOutfit outfit)
        {
            string prefix = $"hero.leg.{side}";
            hero.Group($"{prefix}.hip.pivot", $"{label} Hip Pivot", hip => hip
                .Position(x, 1.45f, 0f)
                .Shape($"{prefix}.pants", $"{label} Layered Baggy Shorts", LowPolyShapeTypes.ProfileCage, pants => pants
                    .Position(0f, -0.31f * legLength, 0f)
                    .Scale(0.31f * outfit.ShortsVolume, 0.58f * legLength, 0.38f * outfit.ShortsVolume)
                    .ProfileCage(
                        Cage(-0.5f, BaggyShortProfile(0.82f)),
                        Cage(-0.1f, BaggyShortProfile(1f)),
                        Cage(0.5f, BaggyShortProfile(0.86f)))
                    .CageQuality(2, 1, true)
                    .ColorRole("pants"))
                .Shape($"{prefix}.pocket", $"{label} Cargo Pocket", LowPolyShapeTypes.ExtrudedProfile, pocket => pocket
                    .Position(side == "left" ? -0.13f : 0.13f, -0.29f * legLength, -0.2f)
                    .Scale(0.14f, 0.16f, 1f)
                    .ExtrudedProfile(0.055f, 0.008f,
                        new(-0.5f, 0.5f), new(0.5f, 0.42f), new(0.4f, -0.5f), new(-0.42f, -0.42f))
                    .ColorRole("pants"))
                .Group($"{prefix}.knee.pivot", $"{label} Knee Pivot", knee => knee
                    .Position(0f, -0.6f * legLength, 0f)
                    .Shape($"{prefix}.knee", $"{label} Exposed Knee", LowPolyShapeTypes.Capsule, joint => joint
                        .Position(0f, -0.02f, -0.03f)
                        .Scale(0.1f, 0.09f, 0.115f)
                        .ColorRole("skin.shadow"))
                    .Shape($"{prefix}.boot.shaft", $"{label} Fitted Tall Boot", LowPolyShapeTypes.ProfileCage, boot => boot
                        .Position(0f, -0.34f * legLength, 0.01f)
                        .Scale(0.21f, 0.58f * legLength * outfit.BootHeight, 0.25f)
                        .ProfileCage(
                            Cage(-0.5f, BootShaftProfile(0.82f)),
                            Cage(0f, BootShaftProfile(1f)),
                            Cage(0.5f, BootShaftProfile(0.78f)))
                        .CageQuality(2, 1, true)
                        .ColorRole("boot"))
                    .Shape($"{prefix}.boot", $"{label} Long Toe Boot", LowPolyShapeTypes.ProfileCage, boot => boot
                        .Position(0f, -0.66f * legLength, -0.075f)
                        .Scale(0.22f, 0.18f, 0.4f)
                        .ProfileCage(
                            Cage(-0.5f, BootFootProfile(0.92f)),
                            Cage(0f, BootFootProfile(1f)),
                            Cage(0.5f, BootFootProfile(0.64f)))
                        .CageQuality(1, 1, true)
                        .ColorRole("boot"))
                    .Shape($"{prefix}.boot.laces", $"{label} Boot Laces", LowPolyShapeTypes.ProfileSweep, laces => laces
                        .Position(0f, -0.35f * legLength, -0.15f)
                        .ProfileSweep(SmallDiamondProfile(), new ForgeVector3[]
                        {
                            new(-0.07f, 0.2f, 0f), new(0.07f, 0.14f, 0f), new(-0.07f, 0.08f, 0f),
                            new(0.07f, 0.02f, 0f), new(-0.065f, -0.04f, 0f), new(0.06f, -0.1f, 0f),
                            new(-0.055f, -0.16f, 0f), new(0.05f, -0.22f, 0f)
                        })
                        .SweepQuality(1, true)
                        .ColorRole("jacket.light"))
                    .Shape($"{prefix}.boot.cuff", $"{label} Boot Top Cuff", LowPolyShapeTypes.LatheProfile, cuff => cuff
                        .Position(0f, -0.08f * legLength, 0.01f)
                        .LatheProfile(12, true,
                            new(0.115f, -0.04f), new(0.135f * outfit.DetailScale, 0f), new(0.115f, 0.04f))
                        .ColorRole("boot"))
                    .Shape($"{prefix}.boot.toe-panel", $"{label} Boot Toe Panel", LowPolyShapeTypes.ExtrudedProfile, panel => panel
                        .Position(0f, -0.62f * legLength, -0.27f)
                        .Rotation(78f, 0f, 0f)
                        .Scale(0.14f * outfit.DetailScale, 0.2f, 1f)
                        .ExtrudedProfile(0.035f, 0.006f,
                            new(-0.45f, 0.5f), new(0.45f, 0.5f), new(0.5f, -0.35f), new(0f, -0.5f), new(-0.5f, -0.35f))
                        .ColorRole("jacket.light"))
                    .Shape($"{prefix}.sole", $"{label} Red Boot Sole", LowPolyShapeTypes.ExtrudedProfile, sole => sole
                        .Position(0f, -0.765f * legLength, -0.09f)
                        .Rotation(90f, 0f, 0f)
                        .Scale(0.24f, 0.43f, 1f)
                        .ExtrudedProfile(0.028f, 0.004f,
                            new(-0.42f, 0.5f), new(0.42f, 0.5f), new(0.5f, -0.36f),
                            new(0.26f, -0.5f), new(-0.26f, -0.5f), new(-0.5f, -0.36f))
                        .ColorRole("sole"))));
        }

        private static LowPolyReferenceProfileCage CreateAuthoredHeadCage()
        {
            ForgeVector2[] profile = HeadProfile();
            return new LowPolyReferenceProfileCage(
                new(0f, 2.81f / ReferenceHeight, 0f),
                new(0.86f / ReferenceHeight, 0.9f / ReferenceHeight, 0.78f / ReferenceHeight),
                new ShapeProfileCageSection[]
                {
                    Cage(-0.5f, TransformProfile(profile, 0.78f, 0.9f, -0.035f)),
                    Cage(-0.32f, TransformProfile(profile, 0.94f, 0.98f, -0.015f)),
                    Cage(0f, TransformProfile(profile, 1f, 1f, 0f)),
                    Cage(0.32f, TransformProfile(profile, 0.93f, 0.98f, 0.01f)),
                    Cage(0.5f, TransformProfile(profile, 0.74f, 0.88f, 0.025f))
                });
        }

        private static LowPolyReferenceProfileCage CreateAuthoredHairCage()
        {
            ForgeVector2[] profile = HairCapProfile();
            return new LowPolyReferenceProfileCage(
                new(0f, 2.99f / ReferenceHeight, 0.035f / ReferenceHeight),
                new(1.01f / ReferenceHeight, 0.76f / ReferenceHeight, 0.88f / ReferenceHeight),
                new ShapeProfileCageSection[]
                {
                    Cage(-0.5f, TransformProfile(profile, 0.72f, 0.82f, -0.04f)),
                    Cage(-0.32f, TransformProfile(profile, 0.94f, 0.96f, -0.01f)),
                    Cage(0f, TransformProfile(profile, 1f, 1f, 0f)),
                    Cage(0.32f, TransformProfile(profile, 0.95f, 0.98f, 0.01f)),
                    Cage(0.5f, TransformProfile(profile, 0.76f, 0.86f, 0.025f))
                });
        }

        private static ForgeVector2[] HeadProfile()
        {
            return new ForgeVector2[]
            {
                new(0f, 0.5f), new(0.28f, 0.46f), new(0.45f, 0.31f), new(0.5f, 0.05f),
                new(0.44f, -0.24f), new(0.28f, -0.44f), new(0f, -0.5f), new(-0.28f, -0.44f),
                new(-0.44f, -0.24f), new(-0.5f, 0.05f), new(-0.45f, 0.31f), new(-0.28f, 0.46f)
            };
        }

        private static ForgeVector2[] HairCapProfile()
        {
            return new ForgeVector2[]
            {
                new(-0.08f, 0.5f), new(0.22f, 0.47f), new(0.42f, 0.36f), new(0.5f, 0.16f),
                new(0.47f, -0.12f), new(0.34f, -0.38f), new(0.08f, -0.44f), new(-0.2f, -0.4f),
                new(-0.42f, -0.24f), new(-0.5f, 0.04f), new(-0.44f, 0.29f), new(-0.3f, 0.44f)
            };
        }

        private static ForgeVector2[] TransformProfile(
            ForgeVector2[] source,
            float          scaleX,
            float          scaleY,
            float          offsetY)
        {
            ForgeVector2[] result = new ForgeVector2[source.Length];
            for (int index = 0; index < result.Length; index++)
                result[index] = new(source[index].X * scaleX, (source[index].Y * scaleY) + offsetY);

            return result;
        }

        private static ShapeRigJoint Joint(
            string role,
            string nodeId,
            float  minimumX,
            float  maximumX,
            float  minimumY,
            float  maximumY,
            float  minimumZ,
            float  maximumZ)
        {
            return new ShapeRigJoint(role, nodeId, new ShapeRigRotationConstraint(
                new(minimumX, minimumY, minimumZ),
                new(maximumX, maximumY, maximumZ)));
        }

        private static ShapeProfileCageSection Cage(float z, ForgeVector2[] profile)
        {
            return new(z, profile);
        }

        private static ShapeProfileSection Section(float z, float x, float y)
        {
            return new(z, new(x, y), ForgeVector2.Zero);
        }

        private static ForgeVector2[] RoundedProfile(float width, float height)
        {
            return new ForgeVector2[]
            {
                new(-0.34f * width, 0.5f * height), new(0.34f * width, 0.5f * height),
                new(0.5f * width, 0.32f * height), new(0.5f * width, -0.32f * height),
                new(0.34f * width, -0.5f * height), new(-0.34f * width, -0.5f * height),
                new(-0.5f * width, -0.32f * height), new(-0.5f * width, 0.32f * height)
            };
        }

        private static ForgeVector2[] TorsoProfile(float width)
        {
            return new ForgeVector2[]
            {
                new(-0.36f * width, 0.5f), new(0.36f * width, 0.5f), new(0.5f * width, 0.32f),
                new(0.43f * width, -0.5f), new(-0.43f * width, -0.5f), new(-0.5f * width, 0.32f)
            };
        }

        private static ForgeVector2[] BaggyShortProfile(float width)
        {
            return new ForgeVector2[]
            {
                new(-0.3f * width, 0.5f), new(0.3f * width, 0.5f), new(0.48f * width, 0.26f),
                new(0.5f * width, -0.16f), new(0.28f * width, -0.5f), new(-0.28f * width, -0.5f),
                new(-0.5f * width, -0.16f), new(-0.48f * width, 0.26f)
            };
        }

        private static ForgeVector2[] BootShaftProfile(float width)
        {
            return new ForgeVector2[]
            {
                new(-0.42f * width, 0.5f), new(0.42f * width, 0.5f), new(0.5f * width, 0.35f),
                new(0.36f * width, -0.5f), new(-0.36f * width, -0.5f), new(-0.5f * width, 0.35f)
            };
        }

        private static ForgeVector2[] BootFootProfile(float width)
        {
            return new ForgeVector2[]
            {
                new(-0.4f * width, 0.5f), new(0.4f * width, 0.5f), new(0.5f * width, 0.2f),
                new(0.44f * width, -0.42f), new(0.22f * width, -0.5f), new(-0.22f * width, -0.5f),
                new(-0.44f * width, -0.42f), new(-0.5f * width, 0.2f)
            };
        }

        private static ForgeVector2[] SmallDiamondProfile()
        {
            return new ForgeVector2[]
            {
                new(0f, 0.022f), new(0.022f, 0f), new(0f, -0.022f), new(-0.022f, 0f)
            };
        }
    }
}
