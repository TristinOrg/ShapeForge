namespace ShapeForge
{
    /// <summary>
    /// Identifies the impact of an observed visual discrepancy.
    /// </summary>
    public enum ShapeVisualDiscrepancySeverity
    {
        /// <summary>Records a minor visual refinement.</summary>
        Information,

        /// <summary>Records a visible issue that should be reviewed.</summary>
        Warning,

        /// <summary>Records a blocking mismatch.</summary>
        Error
    }
}
