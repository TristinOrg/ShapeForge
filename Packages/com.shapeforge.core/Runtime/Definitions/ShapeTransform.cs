using System;
using UnityEngine;

namespace ShapeForge
{
    /// <summary>
    /// Describes the local transform of a shape node.
    /// </summary>
    [Serializable]
    public sealed class ShapeTransform
    {
        [SerializeField] private Vector3 position    = Vector3.zero;
        [SerializeField] private Vector3 eulerAngles = Vector3.zero;
        [SerializeField] private Vector3 scale       = Vector3.one;

        /// <summary>
        /// Gets or sets the local position.
        /// </summary>
        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        /// <summary>
        /// Gets or sets the local Euler rotation in degrees.
        /// </summary>
        public Vector3 EulerAngles
        {
            get => eulerAngles;
            set => eulerAngles = value;
        }

        /// <summary>
        /// Gets or sets the local scale.
        /// </summary>
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        /// <summary>
        /// Applies this definition to a Unity transform.
        /// </summary>
        public void ApplyTo(Transform target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.localPosition    = position;
            target.localEulerAngles = eulerAngles;
            target.localScale       = scale;
        }
    }
}

