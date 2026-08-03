using System;
using System.Collections.Generic;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Transforms the authored Fantasy Hero into a complete Unity Humanoid-compatible hierarchy.
    /// </summary>
    public static class LowPolyHumanoidHeroPreset
    {
        private const string ChestId     = "hero.humanoid.chest";
        private const string NeckId      = "hero.humanoid.neck";
        private const string LeftHandId  = "hero.humanoid.left.hand";
        private const string LeftFootId  = "hero.humanoid.left.foot";
        private const string RightHandId = "hero.humanoid.right.hand";
        private const string RightFootId = "hero.humanoid.right.foot";

        /// <summary>
        /// Gets the shared fantasy-hero style identifier.
        /// </summary>
        public const string StyleId = LowPolyHeroPreset.StyleId;

        /// <summary>
        /// Creates the complete Fantasy Hero appearance in a canonical Humanoid T-Pose hierarchy.
        /// </summary>
        public static ShapeDefinition CreateDefinition()
        {
            ShapeDefinition definition = LowPolyHeroPreset.CreateDefinition();
            ShapeNode       root       = definition.Root;
            ShapeNode       hips       = FindNode(root, "hero.pelvis.pivot");
            ShapeNode       spine      = FindNode(root, "hero.spine.pivot");
            ShapeNode       head       = Detach(root, "hero.head.pivot");
            ShapeNode       leftArm    = Detach(root, "hero.arm.left.shoulder.pivot");
            ShapeNode       rightArm   = Detach(root, "hero.arm.right.shoulder.pivot");
            ShapeNode       leftLeg    = Detach(root, "hero.leg.left.hip.pivot");
            ShapeNode       rightLeg   = Detach(root, "hero.leg.right.hip.pivot");

            ShapeNode chest = Group(ChestId, "Chest", 0f, 0f, 0f);
            ShapeNode neck  = Group(NeckId, "Neck", 0f, 0.74f, 0f);
            spine.Add(chest);
            chest.Add(neck);

            head.Transform.Position = new(0f, 0.14f, 0f);
            neck.Add(head);

            ReparentArm(chest, leftArm, "left", "Left", -90f);
            ReparentArm(chest, rightArm, "right", "Right", 90f);
            ReparentLeg(hips, leftLeg, "left", "Left");
            ReparentLeg(hips, rightLeg, "right", "Right");

            definition.Name = "Low Poly Humanoid Fantasy Hero";
            root.Name       = definition.Name;
            definition.Rig  = CreateRig(root.Id);
            OrientAppearanceForward(root, definition.Rig);
            return definition;
        }

        /// <summary>
        /// Creates the exact palette used by the original Fantasy Hero preset.
        /// </summary>
        public static ShapeStyleDefinition CreateStyle()
        {
            return LowPolyHeroPreset.CreateStyle();
        }

        private static ShapeRigDefinition CreateRig(string rootId)
        {
            return new()
            {
                Type = "humanoid/full",
                Joints = new List<ShapeRigJoint>
                {
                    Joint(ShapeRigRoles.Root, rootId),
                    Joint(ShapeRigRoles.Hips, "hero.pelvis.pivot"),
                    Joint(ShapeRigRoles.Spine, "hero.spine.pivot"),
                    Joint(ShapeRigRoles.Chest, ChestId),
                    Joint(ShapeRigRoles.Neck, NeckId),
                    Joint(ShapeRigRoles.Head, "hero.head.pivot"),
                    Joint(ShapeRigRoles.LeftUpperArm, "hero.arm.left.shoulder.pivot"),
                    Joint(ShapeRigRoles.LeftLowerArm, "hero.arm.left.elbow.pivot"),
                    Joint(ShapeRigRoles.LeftHand, LeftHandId),
                    Joint(ShapeRigRoles.RightUpperArm, "hero.arm.right.shoulder.pivot"),
                    Joint(ShapeRigRoles.RightLowerArm, "hero.arm.right.elbow.pivot"),
                    Joint(ShapeRigRoles.RightHand, RightHandId),
                    Joint(ShapeRigRoles.LeftUpperLeg, "hero.leg.left.hip.pivot"),
                    Joint(ShapeRigRoles.LeftLowerLeg, "hero.leg.left.knee.pivot"),
                    Joint(ShapeRigRoles.LeftFoot, LeftFootId),
                    Joint(ShapeRigRoles.RightUpperLeg, "hero.leg.right.hip.pivot"),
                    Joint(ShapeRigRoles.RightLowerLeg, "hero.leg.right.knee.pivot"),
                    Joint(ShapeRigRoles.RightFoot, RightFootId)
                }
            };
        }

        private static void ReparentArm(
            ShapeNode chest,
            ShapeNode upperArm,
            string    side,
            string    label,
            float     rotationZ)
        {
            upperArm.Transform.Position    = new(upperArm.Transform.Position.X, 0.69f, 0f);
            upperArm.Transform.EulerAngles = new(0f, 0f, rotationZ);
            chest.Add(upperArm);

            ShapeNode lowerArm = FindNode(upperArm, $"hero.arm.{side}.elbow.pivot");
            ShapeNode handMesh = Detach(lowerArm, $"hero.arm.{side}.hand");
            ShapeNode hand     = Group(
                side == "left" ? LeftHandId : RightHandId,
                $"{label} Hand",
                handMesh.Transform.Position.X,
                handMesh.Transform.Position.Y,
                handMesh.Transform.Position.Z);
            handMesh.Transform.Position = ForgeVector3.Zero;
            hand.Add(handMesh);
            lowerArm.Add(hand);
        }

        private static void ReparentLeg(ShapeNode hips, ShapeNode upperLeg, string side, string label)
        {
            upperLeg.Transform.Position = new(upperLeg.Transform.Position.X, 0f, upperLeg.Transform.Position.Z);
            hips.Add(upperLeg);

            ShapeNode lowerLeg = FindNode(upperLeg, $"hero.leg.{side}.knee.pivot");
            ShapeNode boot     = Detach(lowerLeg, $"hero.leg.{side}.boot");
            ShapeNode sole     = Detach(lowerLeg, $"hero.leg.{side}.sole");
            ShapeNode foot     = Group(
                side == "left" ? LeftFootId : RightFootId,
                $"{label} Foot",
                boot.Transform.Position.X,
                boot.Transform.Position.Y,
                boot.Transform.Position.Z);

            ForgeVector3 footPosition = foot.Transform.Position;
            boot.Transform.Position = ForgeVector3.Zero;
            sole.Transform.Position = new(
                sole.Transform.Position.X - footPosition.X,
                sole.Transform.Position.Y - footPosition.Y,
                sole.Transform.Position.Z - footPosition.Z);
            foot.Add(boot).Add(sole);
            lowerLeg.Add(foot);
        }

        private static ShapeNode Group(string id, string name, float x, float y, float z)
        {
            return new(id, name, ShapeTypes.Group)
            {
                Transform = new ShapeTransform { Position = new(x, y, z) }
            };
        }

        private static void OrientAppearanceForward(ShapeNode root, ShapeRigDefinition rig)
        {
            HashSet<string> boneIds = new(StringComparer.Ordinal);
            foreach (ShapeRigJoint joint in rig.Joints)
                boneIds.Add(joint.NodeId);

            foreach (ShapeRigJoint joint in rig.Joints)
            {
                ShapeNode       bone       = FindNode(root, joint.NodeId);
                List<ShapeNode> appearance = new();
                for (int index = bone.Children.Count - 1; index >= 0; index--)
                {
                    ShapeNode child = bone.Children[index];
                    if (boneIds.Contains(child.Id))
                        continue;

                    appearance.Insert(0, child);
                    bone.Children.RemoveAt(index);
                }

                if (appearance.Count == 0)
                    continue;

                ShapeNode visual = Group($"{bone.Id}.humanoid.visual", $"{bone.Name} Visuals", 0f, 0f, 0f);
                visual.Transform.EulerAngles = new(0f, 180f, 0f);
                foreach (ShapeNode child in appearance)
                    visual.Add(child);
                bone.Add(visual);
            }
        }

        private static ShapeNode Detach(ShapeNode parent, string id)
        {
            for (int index = 0; index < parent.Children.Count; index++)
            {
                ShapeNode child = parent.Children[index];
                if (child.Id != id)
                    continue;

                parent.Children.RemoveAt(index);
                return child;
            }

            throw new InvalidOperationException($"Fantasy Hero node '{id}' was not found under '{parent.Id}'.");
        }

        private static ShapeNode FindNode(ShapeNode node, string id)
        {
            if (node.Id == id)
                return node;

            foreach (ShapeNode child in node.Children)
            {
                ShapeNode result = FindNode(child, id);
                if (result != null)
                    return result;
            }

            return null;
        }

        private static ShapeRigJoint Joint(string role, string nodeId)
        {
            return new(role, nodeId);
        }
    }
}
