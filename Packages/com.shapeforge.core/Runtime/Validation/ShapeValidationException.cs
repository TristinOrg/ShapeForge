using System;

namespace ShapeForge
{
    /// <summary>
    /// Reports a ShapeForge definition that violates the current specification.
    /// </summary>
    public sealed class ShapeValidationException : Exception
    {
        /// <summary>
        /// Initializes a shape validation exception.
        /// </summary>
        public ShapeValidationException(string message)
            : base(message)
        {
        }
    }
}
