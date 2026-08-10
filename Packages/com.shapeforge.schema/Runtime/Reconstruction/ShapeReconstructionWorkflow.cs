using System;

namespace ShapeForge
{
    /// <summary>
    /// Stores all explicit artifacts required to resume provider-neutral reconstruction.
    /// </summary>
    [Serializable]
    public sealed class ShapeReconstructionWorkflow
    {
        /// <summary>Identifies the current workflow schema.</summary>
        public const string CurrentSchema = "shapeforge.reconstruction/1.0";

        /// <summary>Gets or sets the schema identifier.</summary>
        public string Schema { get; set; } = CurrentSchema;
        /// <summary>Gets or sets the stable workflow identifier.</summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>Gets or sets the current deterministic stage.</summary>
        public ShapeReconstructionState State { get; set; }
        /// <summary>Gets or sets the zero-based correction iteration.</summary>
        public int Iteration { get; set; }
        /// <summary>Gets or sets the bounded correction limit.</summary>
        public int MaximumIterations { get; set; } = 8;
        /// <summary>Gets or sets the current editable candidate.</summary>
        public ShapeDefinition Definition { get; set; }
        /// <summary>Gets or sets the explicit reference assessment.</summary>
        public ShapeReferenceAssessment Assessment { get; set; }
        /// <summary>Gets or sets the semantic detail inventory.</summary>
        public ShapeDetailInventory Inventory { get; set; }
        /// <summary>Gets or sets the resumable construction plan.</summary>
        public ShapeConstructionPlan Construction { get; set; }
        /// <summary>Gets or sets the latest provider-neutral render comparison.</summary>
        public ShapeRenderComparison Comparison { get; set; }
        /// <summary>Gets or sets the reviewed correction patch.</summary>
        public ShapePatchDocument PendingPatch { get; set; }
        /// <summary>Gets or sets the declared final quality policy.</summary>
        public ShapeQualityPolicy QualityPolicy { get; set; }
    }
}
