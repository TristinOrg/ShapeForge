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
            ValidateRig(definition.Rig, nodeIds);
        }

        private static void ValidateRig(ShapeRigDefinition rig, HashSet<string> nodeIds)
        {
            if (rig == null)
                return;

            if (string.IsNullOrWhiteSpace(rig.Type))
                throw new ShapeValidationException("A semantic rig requires a type.");

            if (rig.Joints == null)
                throw new ShapeValidationException("A semantic rig requires a joint collection.");

            HashSet<string> roles = new(StringComparer.Ordinal);
            foreach (ShapeRigJoint joint in rig.Joints)
            {
                if (joint == null || string.IsNullOrWhiteSpace(joint.Role))
                    throw new ShapeValidationException("Every semantic rig joint requires a role.");

                if (!roles.Add(joint.Role))
                    throw new ShapeValidationException($"Duplicate semantic rig role '{joint.Role}'.");

                if (string.IsNullOrWhiteSpace(joint.NodeId) || !nodeIds.Contains(joint.NodeId))
                    throw new ShapeValidationException(
                        $"Semantic rig role '{joint.Role}' targets unknown node '{joint.NodeId}'.");

                ValidateRotationConstraint(joint);
            }
        }

        private static void ValidateRotationConstraint(ShapeRigJoint joint)
        {
            ShapeRigRotationConstraint constraint = joint.RotationConstraint;
            if (constraint == null)
                return;

            ValidateFinite(constraint.Minimum, joint.Role);
            ValidateFinite(constraint.Maximum, joint.Role);
            if (constraint.Minimum.X > constraint.Maximum.X ||
                constraint.Minimum.Y > constraint.Maximum.Y ||
                constraint.Minimum.Z > constraint.Maximum.Z)
                throw new ShapeValidationException(
                    $"Semantic rig role '{joint.Role}' rotation minimum cannot exceed its maximum.");
        }

        private static void ValidateFinite(ForgeVector3 value, string role)
        {
            if (float.IsNaN(value.X) || float.IsInfinity(value.X) ||
                float.IsNaN(value.Y) || float.IsInfinity(value.Y) ||
                float.IsNaN(value.Z) || float.IsInfinity(value.Z))
                throw new ShapeValidationException(
                    $"Semantic rig role '{role}' rotation limits must be finite.");
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

            if (node.Path == null)
                throw new ShapeValidationException($"Shape node '{node.Id}' requires a path collection.");

            foreach (ForgeVector3 point in node.Path)
            {
                if (float.IsNaN(point.X) || float.IsInfinity(point.X) ||
                    float.IsNaN(point.Y) || float.IsInfinity(point.Y) ||
                    float.IsNaN(point.Z) || float.IsInfinity(point.Z))
                    throw new ShapeValidationException($"Shape node '{node.Id}' path points must be finite.");
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

            if (node.ProfileCageSections == null)
                throw new ShapeValidationException($"Shape node '{node.Id}' requires a profile cage section collection.");

            foreach (ShapeProfileCageSection section in node.ProfileCageSections)
            {
                if (section == null)
                    throw new ShapeValidationException(
                        $"Shape node '{node.Id}' cannot contain null profile cage sections.");

                if (float.IsNaN(section.Z) || float.IsInfinity(section.Z) || section.Profile == null)
                    throw new ShapeValidationException(
                        $"Shape node '{node.Id}' profile cage sections must be finite and contain profiles.");

                foreach (ForgeVector2 point in section.Profile)
                {
                    if (float.IsNaN(point.X) || float.IsInfinity(point.X) ||
                        float.IsNaN(point.Y) || float.IsInfinity(point.Y))
                        throw new ShapeValidationException(
                            $"Shape node '{node.Id}' profile cage points must be finite.");
                }
            }

            foreach (ShapeNode child in node.Children)
                ValidateNode(child, nodeIds);
        }
    }
}
