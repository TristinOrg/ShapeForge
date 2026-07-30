using System;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Adds concise Low Poly procedural-shape configuration to the engine-neutral builder.
    /// </summary>
    public static class LowPolyShapeBuilderExtensions
    {
        /// <summary>Configures the top and bottom dimensions of a frustum node.</summary>
        public static ShapeNodeBuilder Frustum(
            this ShapeNodeBuilder builder,
            float                 topWidth,
            float                 topDepth,
            float                 bottomWidth,
            float                 bottomDepth)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder
                .Parameter(LowPolyShapeParameters.TopWidth, topWidth)
                .Parameter(LowPolyShapeParameters.TopDepth, topDepth)
                .Parameter(LowPolyShapeParameters.BottomWidth, bottomWidth)
                .Parameter(LowPolyShapeParameters.BottomDepth, bottomDepth);
        }

        /// <summary>Configures a normalized outline and depth for an extruded-profile node.</summary>
        public static ShapeNodeBuilder ExtrudedProfile(
            this ShapeNodeBuilder builder,
            float                 depth,
            params ForgeVector2[] points)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder
                .Profile(points)
                .Parameter(LowPolyShapeParameters.ProfileDepth, depth);
        }

        /// <summary>Configures a normalized outline, depth, and single-segment edge bevel.</summary>
        public static ShapeNodeBuilder ExtrudedProfile(
            this ShapeNodeBuilder builder,
            float                 depth,
            float                 bevel,
            params ForgeVector2[] points)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder
                .Profile(points)
                .Parameter(LowPolyShapeParameters.ProfileDepth, depth)
                .Parameter(LowPolyShapeParameters.ProfileBevel, bevel);
        }
    }
}
