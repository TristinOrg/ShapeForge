using System;
using UnityEngine;

namespace ShapeForge.Unity
{
    /// <summary>
    /// Persists a resolved ShapeForge color and reapplies it to a Unity Renderer lifecycle.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnityShapeColor : MonoBehaviour
    {
        private static readonly int ColorProperty     = Shader.PropertyToID("_Color");
        private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");

        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Color    color = Color.white;

        private MaterialPropertyBlock properties;

        /// <summary>
        /// Gets the serialized resolved color.
        /// </summary>
        public Color Color => color;

        /// <summary>
        /// Configures the target Renderer and serialized engine-agnostic color.
        /// </summary>
        public void Configure(Renderer renderer, ForgeColor color)
        {
            targetRenderer = renderer != null
                ? renderer
                : throw new ArgumentNullException(nameof(renderer));
            this.color = color.ToUnity();
            Apply();
        }

        /// <summary>
        /// Reapplies the serialized color without creating a material instance.
        /// </summary>
        public void Apply()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<Renderer>();

            if (targetRenderer == null)
                return;

            if (properties == null)
                properties = new MaterialPropertyBlock();

            targetRenderer.GetPropertyBlock(properties);
            properties.SetColor(ColorProperty, color);
            properties.SetColor(BaseColorProperty, color);
            targetRenderer.SetPropertyBlock(properties);
        }

        private void OnEnable()
        {
            Apply();
        }

        private void OnValidate()
        {
            Apply();
        }
    }
}
