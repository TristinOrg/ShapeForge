namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Identifies the reference view that supplied a measurement.
    /// </summary>
    public enum LowPolyStylizedHumanReferenceView
    {
        Front,
        Side
    }

    /// <summary>
    /// Classifies how far an observation is from the template baseline.
    /// </summary>
    public enum LowPolyStylizedHumanReferenceDeviation
    {
        NearBaseline,
        Moderate,
        Strong
    }

    /// <summary>
    /// Describes one normalized observation and its effect relative to the template baseline.
    /// </summary>
    public sealed class LowPolyStylizedHumanReferenceDiagnostic
    {
        /// <summary>Creates an immutable measurement diagnostic.</summary>
        public LowPolyStylizedHumanReferenceDiagnostic(
            string                                      path,
            LowPolyStylizedHumanReferenceView           view,
            float                                       observedValue,
            float                                       baselineValue,
            LowPolyStylizedHumanReferenceDeviation      deviation)
        {
            Path          = path;
            View          = view;
            ObservedValue = observedValue;
            BaselineValue = baselineValue;
            Multiplier    = observedValue / baselineValue;
            Deviation     = deviation;
        }

        /// <summary>Gets the JSON path of the observation.</summary>
        public string Path { get; }

        /// <summary>Gets the view that supplied the observation.</summary>
        public LowPolyStylizedHumanReferenceView View { get; }

        /// <summary>Gets the normalized observed value.</summary>
        public float ObservedValue { get; }

        /// <summary>Gets the default template observation.</summary>
        public float BaselineValue { get; }

        /// <summary>Gets the observed-to-baseline ratio.</summary>
        public float Multiplier { get; }

        /// <summary>Gets the magnitude classification relative to the baseline.</summary>
        public LowPolyStylizedHumanReferenceDeviation Deviation { get; }
    }
}
