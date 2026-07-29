using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeForge.Unity
{
    /// <summary>
    /// Uses shared materials for palette colors and property blocks for direct overrides.
    /// </summary>
    public sealed class HybridUnityAppearanceBackend : IUnityAppearanceBackend
    {
        /// <inheritdoc />
        public IUnityAppearanceSession Begin(ShapeGenerationContext context)
        {
            return new Session(context);
        }

        private sealed class Session : IUnityAppearanceSession
        {
            private readonly List<UnityShapeAppearanceBinding> bindings =
                new List<UnityShapeAppearanceBinding>();
            private readonly ShapeGenerationContext context;

            public Session(ShapeGenerationContext context)
            {
                this.context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public void Apply(Renderer renderer, ShapeNode node)
            {
                if (renderer == null)
                    throw new ArgumentNullException(nameof(renderer));

                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                if (!context.TryResolveColor(node, out ForgeColor color))
                    return;

                UnityShapeAppearanceBinding binding = new UnityShapeAppearanceBinding(
                    renderer,
                    renderer.sharedMaterial,
                    color.ToUnity(),
                    node.Appearance.HasColorOverride
                        ? UnityShapeAppearanceMode.PropertyBlock
                        : UnityShapeAppearanceMode.SharedMaterial);

                bindings.Add(binding);
                UnityShapeAppearanceManifest.Apply(binding);
            }

            public void Complete(GameObject root)
            {
                if (root == null)
                    throw new ArgumentNullException(nameof(root));

                if (bindings.Count == 0)
                    return;

                UnityShapeAppearanceManifest manifest = root.AddComponent<UnityShapeAppearanceManifest>();
                manifest.Configure(bindings);
            }
        }
    }
}
