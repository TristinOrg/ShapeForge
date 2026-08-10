using System;

namespace ShapeForge
{
    /// <summary>
    /// Localizes one provider-observed mismatch to semantic ShapeForge identities.
    /// </summary>
    [Serializable]
    public sealed class ShapeVisualDiscrepancy
    {
        /// <summary>Gets or sets the stable discrepancy identifier.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets an extensible mismatch category.</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Gets or sets the affected view identifier.</summary>
        public string ViewId { get; set; } = string.Empty;

        /// <summary>Gets or sets the affected ShapeDefinition node ID, when known.</summary>
        public string NodeId { get; set; } = string.Empty;

        /// <summary>Gets or sets the affected Detail Inventory ID, when known.</summary>
        public string DetailId { get; set; } = string.Empty;

        /// <summary>Gets or sets discrepancy impact.</summary>
        public ShapeVisualDiscrepancySeverity Severity { get; set; }

        /// <summary>Gets or sets the observed mismatch description.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Gets or sets a provider-neutral correction hint.</summary>
        public string SuggestedAction { get; set; } = string.Empty;
    }
}
