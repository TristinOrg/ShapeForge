namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Provides measured multi-view silhouettes for the pocket-fantasy hero preset.
    /// </summary>
    public static class LowPolyFantasyHeroReference
    {
        /// <summary>Identifies the measured head part.</summary>
        public const string HeadPartId = "character/head";

        /// <summary>Identifies the measured hair part.</summary>
        public const string HairPartId = "character/hair";

        /// <summary>Creates a fresh, mutable copy of the built-in aligned reference observations.</summary>
        public static ShapeReferenceDefinition Create()
        {
            ShapeReferenceDefinition reference = new()
            {
                Name = "Pocket Fantasy Hero Reference"
            };
            reference.Parts.Add(Part(
                HeadPartId,
                View(0.375f, 0.69f, 0.625f, 0.95f,
                    new(0.5f, 1f), new(0.75f, 0.93f), new(0.93f, 0.7f), new(1f, 0.42f),
                    new(0.88f, 0.15f), new(0.68f, 0f), new(0.32f, 0f), new(0.12f, 0.15f),
                    new(0f, 0.42f), new(0.07f, 0.7f), new(0.25f, 0.93f)),
                View(0.38f, 0.69f, 0.62f, 0.95f,
                    new(0.52f, 1f), new(0.82f, 0.88f), new(1f, 0.62f), new(0.94f, 0.36f),
                    new(0.73f, 0.08f), new(0.5f, 0f), new(0.24f, 0.1f), new(0f, 0.38f),
                    new(0.04f, 0.68f), new(0.22f, 0.9f)),
                View(0.375f, 0.69f, 0.625f, 0.95f,
                    new(0.5f, 1f), new(0.78f, 0.91f), new(0.97f, 0.65f), new(0.95f, 0.3f),
                    new(0.72f, 0.04f), new(0.5f, 0f), new(0.28f, 0.04f), new(0.05f, 0.3f),
                    new(0.03f, 0.65f), new(0.22f, 0.91f))));
            reference.Parts.Add(Part(
                HairPartId,
                View(0.345f, 0.655f, 0.655f, 1f,
                    new(0.5f, 1f), new(0.6f, 0.91f), new(0.72f, 0.98f), new(0.7f, 0.86f),
                    new(0.87f, 0.91f), new(0.82f, 0.76f), new(1f, 0.68f), new(0.86f, 0.58f),
                    new(0.94f, 0.43f), new(0.77f, 0.36f), new(0.78f, 0.22f), new(0.64f, 0.3f),
                    new(0.58f, 0.06f), new(0.49f, 0.25f), new(0.36f, 0f), new(0.33f, 0.3f),
                    new(0.16f, 0.14f), new(0.19f, 0.4f), new(0f, 0.32f), new(0.12f, 0.56f),
                    new(0.02f, 0.66f), new(0.2f, 0.73f), new(0.17f, 0.87f), new(0.38f, 0.84f)),
                View(0.34f, 0.65f, 0.66f, 1f,
                    new(0.52f, 1f), new(0.68f, 0.88f), new(0.81f, 0.95f), new(0.8f, 0.82f),
                    new(0.96f, 0.86f), new(0.89f, 0.7f), new(1f, 0.59f), new(0.87f, 0.52f),
                    new(0.94f, 0.38f), new(0.78f, 0.37f), new(0.82f, 0.2f), new(0.63f, 0.28f),
                    new(0.5f, 0f), new(0.37f, 0.25f), new(0.16f, 0.18f), new(0.2f, 0.38f),
                    new(0f, 0.46f), new(0.12f, 0.62f), new(0.08f, 0.78f), new(0.3f, 0.9f)),
                View(0.345f, 0.655f, 0.655f, 1f,
                    new(0.5f, 1f), new(0.64f, 0.9f), new(0.78f, 0.98f), new(0.76f, 0.84f),
                    new(0.94f, 0.88f), new(0.86f, 0.72f), new(1f, 0.62f), new(0.86f, 0.54f),
                    new(0.96f, 0.39f), new(0.77f, 0.38f), new(0.81f, 0.2f), new(0.62f, 0.27f),
                    new(0.5f, 0f), new(0.38f, 0.27f), new(0.19f, 0.2f), new(0.23f, 0.38f),
                    new(0.04f, 0.39f), new(0.14f, 0.54f), new(0f, 0.62f), new(0.14f, 0.72f),
                    new(0.06f, 0.88f), new(0.24f, 0.84f), new(0.22f, 0.98f), new(0.36f, 0.9f))));
            return reference;
        }

        private static ShapeReferencePart Part(
            string                        id,
            ShapeReferenceViewObservation front,
            ShapeReferenceViewObservation side,
            ShapeReferenceViewObservation back)
        {
            return new ShapeReferencePart
            {
                Id    = id,
                Front = front,
                Side  = side,
                Back  = back
            };
        }

        private static ShapeReferenceViewObservation View(
            float          minimumX,
            float          minimumY,
            float          maximumX,
            float          maximumY,
            params ForgeVector2[] normalizedSilhouette)
        {
            ShapeReferenceViewObservation view = new()
            {
                Minimum = new(minimumX, minimumY),
                Maximum = new(maximumX, maximumY)
            };
            float width  = maximumX - minimumX;
            float height = maximumY - minimumY;
            foreach (ForgeVector2 point in normalizedSilhouette)
                view.Silhouette.Add(new(minimumX + (point.X * width), minimumY + (point.Y * height)));

            return view;
        }
    }
}
