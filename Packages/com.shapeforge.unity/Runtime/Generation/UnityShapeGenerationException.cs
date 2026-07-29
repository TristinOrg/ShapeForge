using System;

namespace ShapeForge.Unity
{
    /// <summary>
    /// Reports failures while adapting a valid ShapeForge definition to Unity.
    /// </summary>
    public sealed class UnityShapeGenerationException : Exception
    {
        /// <summary>
        /// Initializes a Unity shape generation exception.
        /// </summary>
        public UnityShapeGenerationException(string message)
            : base(message)
        {
        }
    }
}
