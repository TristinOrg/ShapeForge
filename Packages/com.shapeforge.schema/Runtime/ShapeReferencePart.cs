using System;

namespace ShapeForge
{
    /// <summary>
    /// Groups aligned front, side, and back observations for one semantic model part.
    /// </summary>
    [Serializable]
    public sealed class ShapeReferencePart
    {
        /// <summary>Gets or sets the stable semantic part ID.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional front-view observation.</summary>
        public ShapeReferenceViewObservation Front { get; set; }

        /// <summary>Gets or sets the optional side-view observation.</summary>
        public ShapeReferenceViewObservation Side { get; set; }

        /// <summary>Gets or sets the optional back-view observation.</summary>
        public ShapeReferenceViewObservation Back { get; set; }
    }
}
