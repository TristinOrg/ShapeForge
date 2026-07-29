using System;

namespace ShapeForge
{
    /// <summary>
    /// Carries immutable model-level information through a generation pass.
    /// </summary>
    public sealed class ShapeGenerationContext
    {
        /// <summary>
        /// Initializes a generation context for a model definition.
        /// </summary>
        public ShapeGenerationContext(ShapeDefinition definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        /// <summary>
        /// Gets the model being generated.
        /// </summary>
        public ShapeDefinition Definition { get; }
    }
}

