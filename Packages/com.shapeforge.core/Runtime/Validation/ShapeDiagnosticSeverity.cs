namespace ShapeForge
{
    /// <summary>
    /// Identifies the impact of a ShapeForge diagnostic.
    /// </summary>
    public enum ShapeDiagnosticSeverity
    {
        /// <summary>Provides non-actionable context.</summary>
        Information,

        /// <summary>Identifies a usable definition with a potential problem.</summary>
        Warning,

        /// <summary>Identifies a definition that cannot be compiled safely.</summary>
        Error
    }
}
