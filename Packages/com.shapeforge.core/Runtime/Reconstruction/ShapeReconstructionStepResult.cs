using System;

namespace ShapeForge
{
    /// <summary>
    /// Reports one deterministic reconstruction transition and its optional patched candidate.
    /// </summary>
    public sealed class ShapeReconstructionStepResult
    {
        /// <summary>Initializes an immutable transition result.</summary>
        public ShapeReconstructionStepResult(
            ShapeReconstructionState state,
            int                      iteration,
            ShapeDefinition          definition,
            ShapeDiagnosticReport    diagnostics)
        {
            State       = state;
            Iteration   = iteration;
            Definition  = definition;
            Diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
        }

        /// <summary>Gets the next persisted state.</summary>
        public ShapeReconstructionState State { get; }
        /// <summary>Gets the next correction iteration.</summary>
        public int Iteration { get; }
        /// <summary>Gets the current or transactionally patched candidate.</summary>
        public ShapeDefinition Definition { get; }
        /// <summary>Gets transition diagnostics.</summary>
        public ShapeDiagnosticReport Diagnostics { get; }
        /// <summary>Gets whether the transition is safe to persist.</summary>
        public bool Succeeded => Diagnostics.IsValid;
    }
}
