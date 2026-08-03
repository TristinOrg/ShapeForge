using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Defines the canonical semantic roles required by a biped humanoid rig.
    /// </summary>
    public static class ShapeHumanoidRig
    {
        private static readonly string[] requiredRoles =
        {
            ShapeRigRoles.Hips,
            ShapeRigRoles.Spine,
            ShapeRigRoles.Chest,
            ShapeRigRoles.Neck,
            ShapeRigRoles.Head,
            ShapeRigRoles.LeftUpperArm,
            ShapeRigRoles.LeftLowerArm,
            ShapeRigRoles.LeftHand,
            ShapeRigRoles.RightUpperArm,
            ShapeRigRoles.RightLowerArm,
            ShapeRigRoles.RightHand,
            ShapeRigRoles.LeftUpperLeg,
            ShapeRigRoles.LeftLowerLeg,
            ShapeRigRoles.LeftFoot,
            ShapeRigRoles.RightUpperLeg,
            ShapeRigRoles.RightLowerLeg,
            ShapeRigRoles.RightFoot
        };

        /// <summary>
        /// Gets the mandatory semantic roles for a Unity-compatible biped humanoid skeleton.
        /// </summary>
        public static IReadOnlyList<string> RequiredRoles => requiredRoles;

        /// <summary>
        /// Throws when a semantic rig does not provide every mandatory humanoid role.
        /// </summary>
        public static void ValidateRequiredRoles(ShapeRigDefinition rig)
        {
            if (rig == null)
                throw new ArgumentNullException(nameof(rig));

            ShapeRigIndex index = new(rig);
            foreach (string role in requiredRoles)
            {
                if (!index.TryGetNodeId(role, out _))
                    throw new ShapeValidationException($"Humanoid rig requires semantic role '{role}'.");
            }
        }
    }
}
