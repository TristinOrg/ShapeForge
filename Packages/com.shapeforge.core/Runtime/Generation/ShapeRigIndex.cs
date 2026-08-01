using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Provides allocation-free semantic role lookups after one-time construction.
    /// </summary>
    public sealed class ShapeRigIndex
    {
        private readonly Dictionary<string, string> nodeIdsByRole;

        /// <summary>
        /// Builds a lookup index from a validated semantic rig definition.
        /// </summary>
        public ShapeRigIndex(ShapeRigDefinition rig)
        {
            if (rig == null)
                throw new ArgumentNullException(nameof(rig));

            nodeIdsByRole = new Dictionary<string, string>(rig.Joints.Count, StringComparer.Ordinal);
            foreach (ShapeRigJoint joint in rig.Joints)
                nodeIdsByRole.Add(joint.Role, joint.NodeId);
        }

        /// <summary>
        /// Tries to resolve a semantic role to its stable shape node ID.
        /// </summary>
        public bool TryGetNodeId(string role, out string nodeId)
        {
            return nodeIdsByRole.TryGetValue(role, out nodeId);
        }
    }
}
