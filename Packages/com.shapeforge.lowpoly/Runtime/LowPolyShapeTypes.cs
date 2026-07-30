namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Defines shape identifiers implemented by the official Low Poly package.
    /// </summary>
    public static class LowPolyShapeTypes
    {
        /// <summary>
        /// Identifies a unit Low Poly cube scaled by its shape transform.
        /// </summary>
        public const string Cube = "lowpoly/cube";

        /// <summary>
        /// Identifies a unit Low Poly sphere scaled by its shape transform.
        /// </summary>
        public const string Sphere = "lowpoly/sphere";

        /// <summary>
        /// Identifies a unit Low Poly cylinder scaled by its shape transform.
        /// </summary>
        public const string Cylinder = "lowpoly/cylinder";

        /// <summary>
        /// Identifies a unit Low Poly capsule scaled by its shape transform.
        /// </summary>
        public const string Capsule = "lowpoly/capsule";

        /// <summary>
        /// Identifies a unit triangular wedge suitable for sloped silhouettes.
        /// </summary>
        public const string Wedge = "lowpoly/wedge";

        /// <summary>
        /// Identifies a centered tapered box controlled by top and bottom dimensions.
        /// </summary>
        public const string Frustum = "lowpoly/frustum";
    }

    /// <summary>
    /// Defines numeric parameter names supported by official Low Poly procedural shapes.
    /// </summary>
    public static class LowPolyShapeParameters
    {
        /// <summary>Controls a frustum's top width relative to its transform scale.</summary>
        public const string TopWidth = "topWidth";

        /// <summary>Controls a frustum's top depth relative to its transform scale.</summary>
        public const string TopDepth = "topDepth";

        /// <summary>Controls a frustum's bottom width relative to its transform scale.</summary>
        public const string BottomWidth = "bottomWidth";

        /// <summary>Controls a frustum's bottom depth relative to its transform scale.</summary>
        public const string BottomDepth = "bottomDepth";
    }
}
