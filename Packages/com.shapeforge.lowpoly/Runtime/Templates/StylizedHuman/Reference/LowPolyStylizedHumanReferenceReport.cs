using System;
using System.Collections.Generic;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Summarizes reference coverage and deviations before semantic compilation.
    /// </summary>
    public sealed class LowPolyStylizedHumanReferenceReport
    {
        /// <summary>Creates an immutable reference diagnostic report.</summary>
        public LowPolyStylizedHumanReferenceReport(
            IReadOnlyList<LowPolyStylizedHumanReferenceDiagnostic> diagnostics,
            bool                                                    hasSideView)
        {
            Diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
            HasSideView = hasSideView;
        }

        /// <summary>Gets all measured values in deterministic JSON-path order.</summary>
        public IReadOnlyList<LowPolyStylizedHumanReferenceDiagnostic> Diagnostics { get; }

        /// <summary>Gets whether head and hair depth are constrained by a side view.</summary>
        public bool HasSideView { get; }

        /// <summary>Gets whether every currently supported geometric dimension is constrained.</summary>
        public bool HasCompleteGeometryCoverage => HasSideView;
    }
}
