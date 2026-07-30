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
    }
}
