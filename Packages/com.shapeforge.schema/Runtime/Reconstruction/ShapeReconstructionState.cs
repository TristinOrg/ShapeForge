namespace ShapeForge
{
    /// <summary>
    /// Identifies deterministic stages in a provider-neutral reconstruction workflow.
    /// </summary>
    public enum ShapeReconstructionState
    {
        /// <summary>Awaiting a validated reference assessment.</summary>
        Draft,
        /// <summary>Reference observations are ready for detail inventory.</summary>
        ReferenceAssessed,
        /// <summary>Detail requirements are bound to the current definition.</summary>
        InventoryReady,
        /// <summary>Construction passes are being executed.</summary>
        Constructing,
        /// <summary>A completed candidate is awaiting render comparison.</summary>
        Comparing,
        /// <summary>A reviewed patch is awaiting transactional application.</summary>
        Correcting,
        /// <summary>The candidate is awaiting its final quality gate.</summary>
        QualityChecking,
        /// <summary>The reconstruction passed its declared quality policy.</summary>
        Completed,
        /// <summary>The bounded workflow cannot continue without revised input.</summary>
        Failed
    }
}
