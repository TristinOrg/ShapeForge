using System;

namespace ShapeForge
{
    /// <summary>
    /// Reports invalid definitions or missing generators during shape generation.
    /// </summary>
    public sealed class ShapeGenerationException : Exception
    {
        /// <summary>
        /// Initializes a shape generation exception.
        /// </summary>
        public ShapeGenerationException(string message)
            : base(message)
        {
        }
    }
}
