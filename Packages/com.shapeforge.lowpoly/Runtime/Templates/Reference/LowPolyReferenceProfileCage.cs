using System;
using System.Collections.Generic;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Contains a normalized transform and profile-cage sections reconstructed from reference views.
    /// </summary>
    public sealed class LowPolyReferenceProfileCage
    {
        /// <summary>Initializes an immutable reference-derived profile cage.</summary>
        public LowPolyReferenceProfileCage(
            ForgeVector3                           position,
            ForgeVector3                           scale,
            IReadOnlyList<ShapeProfileCageSection> sections)
        {
            Position = position;
            Scale    = scale;
            Sections = sections ?? throw new ArgumentNullException(nameof(sections));
        }

        /// <summary>Gets the part center in normalized reference space.</summary>
        public ForgeVector3 Position { get; }

        /// <summary>Gets the part width, height, and depth in normalized reference space.</summary>
        public ForgeVector3 Scale { get; }

        /// <summary>Gets the ordered normalized profile-cage sections.</summary>
        public IReadOnlyList<ShapeProfileCageSection> Sections { get; }
    }
}
