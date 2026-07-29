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

            foreach (ShapeNode child in node.Children)
                ValidateNode(child, nodeIds);
        }
    }
}
