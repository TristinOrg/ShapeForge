using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeForge.Unity
{
    /// <summary>
    /// Persists and restores all renderer appearance bindings for one generated model.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnityShapeAppearanceManifest : MonoBehaviour
    {
        private static readonly int ColorProperty     = Shader.PropertyToID("_Color");
        private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");

        private static MaterialPropertyBlock sharedProperties;

        [SerializeField] private List<UnityShapeAppearanceBinding> bindings =
            new List<UnityShapeAppearanceBinding>();

        /// <summary>
        /// Gets the number of persisted renderer bindings.
        /// </summary>
        public int BindingCount => bindings.Count;

        /// <summary>
        /// Replaces the persisted renderer bindings and applies them immediately.
        /// </summary>
        public void Configure(IEnumerable<UnityShapeAppearanceBinding> newBindings)
        {
            if (newBindings == null)
                throw new ArgumentNullException(nameof(newBindings));

            bindings.Clear();
            bindings.AddRange(newBindings);
            Apply();
        }

        /// <summary>
        /// Restores all shared-material and property-block appearance state.
        /// </summary>
        public void Apply()
        {
            foreach (UnityShapeAppearanceBinding binding in bindings)
                Apply(binding);
        }

        internal static void Apply(UnityShapeAppearanceBinding binding)
        {
            Renderer renderer = binding.Renderer;
            if (renderer == null)
                return;

            renderer.sharedMaterial = binding.BaseMaterial;

            if (binding.Mode == UnityShapeAppearanceMode.SharedMaterial)
            {
                renderer.SetPropertyBlock(null);
                renderer.sharedMaterial = UnityPaletteMaterialCache.Get(binding.BaseMaterial, binding.Color);
                return;
            }

            if (sharedProperties == null)
                sharedProperties = new MaterialPropertyBlock();

            sharedProperties.Clear();
            renderer.GetPropertyBlock(sharedProperties);
            sharedProperties.SetColor(ColorProperty, binding.Color);
            sharedProperties.SetColor(BaseColorProperty, binding.Color);
            renderer.SetPropertyBlock(sharedProperties);
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

    /// <summary>
    /// Selects the Unity rendering path for one resolved color.
    /// </summary>
    public enum UnityShapeAppearanceMode
    {
        SharedMaterial = 0,
        PropertyBlock  = 1
    }

    /// <summary>
    /// Stores one renderer's resolved appearance without owning transient render resources.
    /// </summary>
    [Serializable]
    public sealed class UnityShapeAppearanceBinding
    {
        [SerializeField] private Renderer                 renderer;
        [SerializeField] private Material                 baseMaterial;
        [SerializeField] private Color                    color;
        [SerializeField] private UnityShapeAppearanceMode mode;

        /// <summary>
        /// Initializes a serialized appearance binding.
        /// </summary>
        public UnityShapeAppearanceBinding(
            Renderer                 renderer,
            Material                 baseMaterial,
            Color                    color,
            UnityShapeAppearanceMode mode)
        {
            this.renderer     = renderer;
            this.baseMaterial = baseMaterial;
            this.color        = color;
            this.mode         = mode;
        }

        /// <summary>
        /// Gets the target renderer.
        /// </summary>
        public Renderer Renderer => renderer;

        /// <summary>
        /// Gets the unmodified source material.
        /// </summary>
        public Material BaseMaterial => baseMaterial;

        /// <summary>
        /// Gets the resolved Unity color.
        /// </summary>
        public Color Color => color;

        /// <summary>
        /// Gets the selected rendering path.
        /// </summary>
        public UnityShapeAppearanceMode Mode => mode;
    }
}
