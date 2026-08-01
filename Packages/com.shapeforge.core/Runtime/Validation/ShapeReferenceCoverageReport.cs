using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Summarizes multi-view coverage and cross-view height consistency for reference parts.
    /// </summary>
    public sealed class ShapeReferenceCoverageReport
    {
        /// <summary>Initializes an immutable reference coverage report.</summary>
        public ShapeReferenceCoverageReport(
            int                       partCount,
            int                       completePartCount,
            IReadOnlyList<string>     inconsistentPartIds)
        {
            PartCount           = partCount;
            CompletePartCount   = completePartCount;
            InconsistentPartIds = inconsistentPartIds ?? throw new ArgumentNullException(nameof(inconsistentPartIds));
        }

        /// <summary>Gets the number of semantic parts.</summary>
        public int PartCount { get; }

        /// <summary>Gets the number of parts observed from front, side, and back.</summary>
        public int CompletePartCount { get; }

        /// <summary>Gets part IDs whose normalized heights disagree across available views.</summary>
        public IReadOnlyList<string> InconsistentPartIds { get; }

        /// <summary>Gets whether every part has front, side, and back observations.</summary>
        public bool HasCompleteCoverage => PartCount > 0 && CompletePartCount == PartCount;

        /// <summary>Gets whether all available views agree within the requested tolerance.</summary>
        public bool IsConsistent => InconsistentPartIds.Count == 0;
    }
}
