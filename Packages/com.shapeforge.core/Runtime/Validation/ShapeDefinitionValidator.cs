using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Validates engine-agnostic ShapeForge definitions before generation.
    /// </summary>
    public sealed class ShapeDefinitionValidator
    {
        /// <summary>
        /// Validates schema compatibility, node identity, and required node data.
        /// </summary>
        public void Validate(ShapeDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (!string.Equals(definition.Schema, ShapeDefinition.CurrentSchema, StringComparison.Ordinal))
                throw new ShapeValidationException($"Unsupported shape schema '{definition.Schema}'.");

            if (definition.Root != null && definition.Root.MirrorAxis != ShapeMirrorAxis.None)
                throw new ShapeValidationException("The root node cannot create a mirrored sibling.");

            HashSet<string> nodeIds = new HashSet<string>(StringComparer.Ordinal);
            ValidateNode(definition.Root, nodeIds);
        }

        private static void ValidateNode(ShapeNode node, HashSet<string> nodeIds)
        {
            if (node == null)
                throw new ShapeValidationException("Shape definitions cannot contain null nodes.");

            if (string.IsNullOrWhiteSpace(node.Id))
                throw new ShapeValidationException("Every shape node requires a stable ID.");

            if (!nodeIds.Add(node.Id))
                throw new ShapeValidationException($"Duplicate shape node ID '{node.Id}'.");

            if (string.IsNullOrWhiteSpace(node.Type))
                throw new ShapeValidationException($"Shape node '{node.Id}' requires a type.");

            if (node.Transform == null)
                throw new ShapeValidationException($"Shape node '{node.Id}' requires a transform.");

            if (node.Appearance == null)
                throw new ShapeValidationException($"Shape node '{node.Id}' requires appearance data.");

            if (!Enum.IsDefined(typeof(ShapeMirrorAxis), node.MirrorAxis))
                throw new ShapeValidationException($"Shape node '{node.Id}' has an unsupported mirror axis.");

            if (node.Appearance.HasColorOverride)
                ForgeColorValidator.Validate(node.Appearance.Color, $"Shape node '{node.Id}'");

            if (node.Parameters == null)
                throw new ShapeValidationException($"Shape node '{node.Id}' requires a parameter collection.");

            foreach (KeyValuePair<string, float> parameter in node.Parameters)
            {
                if (string.IsNullOrWhiteSpace(parameter.Key))
                    throw new ShapeValidationException($"Shape node '{node.Id}' has an empty parameter name.");

                if (float.IsNaN(parameter.Value) || float.IsInfinity(parameter.Value))
                    throw new ShapeValidationException(
                        $"Shape node '{node.Id}' parameter '{parameter.Key}' must be finite.");
            }

            if (node.Profile == null)
                throw new ShapeValidationException($"Shape node '{node.Id}' requires a profile collection.");

            foreach (ForgeVector2 point in node.Profile)
            {
                if (float.IsNaN(point.X) || float.IsInfinity(point.X) ||
                    float.IsNaN(point.Y) || float.IsInfinity(point.Y))
                    throw new ShapeValidationException($"Shape node '{node.Id}' profile points must be finite.");
            }

            if (node.ProfileSections == null)
                throw new ShapeValidationException($"Shape node '{node.Id}' requires a profile section collection.");

            foreach (ShapeProfileSection section in node.ProfileSections)
            {
                if (section == null)
                    throw new ShapeValidationException($"Shape node '{node.Id}' cannot contain null profile sections.");

                if (float.IsNaN(section.Z) || float.IsInfinity(section.Z) ||
                    float.IsNaN(section.Scale.X) || float.IsInfinity(section.Scale.X) ||
                    float.IsNaN(section.Scale.Y) || float.IsInfinity(section.Scale.Y) ||
                    float.IsNaN(section.Offset.X) || float.IsInfinity(section.Offset.X) ||
                    float.IsNaN(section.Offset.Y) || float.IsInfinity(section.Offset.Y))
                    throw new ShapeValidationException($"Shape node '{node.Id}' profile sections must be finite.");
            }

            foreach (ShapeNode child in node.Children)
                ValidateNode(child, nodeIds);
        }
    }
}
