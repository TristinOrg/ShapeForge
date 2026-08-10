using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Advances explicit reconstruction artifacts without owning vision providers or credentials.
    /// </summary>
    public sealed class ShapeReconstructionOrchestrator
    {
        /// <summary>Evaluates and advances one resumable workflow stage.</summary>
        public ShapeReconstructionStepResult Advance(ShapeReconstructionWorkflow workflow)
        {
            ShapeDiagnosticReport baseline = new ShapeReconstructionWorkflowValidator().Analyze(workflow);
            if (!baseline.IsValid)
                return Result(workflow, workflow?.State ?? ShapeReconstructionState.Failed, baseline);

            switch (workflow.State)
            {
                case ShapeReconstructionState.Draft:
                    return ValidateAndMove(workflow, workflow.Assessment == null
                        ? Missing("assessment", "/assessment")
                        : new ShapeReferenceAssessmentValidator().Analyze(workflow.Assessment),
                        ShapeReconstructionState.ReferenceAssessed);
                case ShapeReconstructionState.ReferenceAssessed:
                    return ValidateAndMove(workflow, workflow.Inventory == null
                        ? Missing("detail inventory", "/inventory")
                        : new ShapeDetailCoverageAnalyzer().Analyze(workflow.Definition, workflow.Inventory).Diagnostics,
                        ShapeReconstructionState.InventoryReady);
                case ShapeReconstructionState.InventoryReady:
                    return ValidateAndMove(workflow, workflow.Construction == null
                        ? Missing("construction plan", "/construction")
                        : new ShapeConstructionPlanValidator().Analyze(workflow.Construction),
                        ShapeReconstructionState.Constructing);
                case ShapeReconstructionState.Constructing:
                    return AdvanceConstruction(workflow);
                case ShapeReconstructionState.Comparing:
                    return AdvanceComparison(workflow);
                case ShapeReconstructionState.Correcting:
                    return ApplyCorrection(workflow);
                case ShapeReconstructionState.QualityChecking:
                    return AdvanceQuality(workflow);
                case ShapeReconstructionState.Completed:
                case ShapeReconstructionState.Failed:
                    return Result(workflow, workflow.State, ShapeDiagnosticReport.Success);
                default:
                    return Result(workflow, ShapeReconstructionState.Failed, Missing("known workflow state", "/state"));
            }
        }

        private static ShapeReconstructionStepResult AdvanceConstruction(ShapeReconstructionWorkflow workflow)
        {
            if (workflow.Construction == null)
                return Result(workflow, workflow.State, Missing("construction plan", "/construction"));
            ShapeConstructionPlanReport report = new ShapeConstructionPlanEvaluator().Evaluate(workflow.Construction);
            ShapeReconstructionState state = report.Diagnostics.IsValid && report.CompletedCount == report.PassCount
                ? ShapeReconstructionState.Comparing
                : workflow.State;
            return Result(workflow, state, report.Diagnostics);
        }

        private static ShapeReconstructionStepResult AdvanceComparison(ShapeReconstructionWorkflow workflow)
        {
            if (workflow.Comparison == null)
                return Result(workflow, workflow.State, Missing("render comparison", "/comparison"));
            ShapeRenderCompareReport report = new ShapeRenderCompareAggregator().Aggregate(workflow.Comparison);
            ShapeReconstructionState state = workflow.PendingPatch?.Operations.Count > 0
                ? ShapeReconstructionState.Correcting
                : ShapeReconstructionState.QualityChecking;
            return Result(workflow, state, report.Diagnostics);
        }

        private static ShapeReconstructionStepResult ApplyCorrection(ShapeReconstructionWorkflow workflow)
        {
            if (workflow.PendingPatch == null)
                return Result(workflow, workflow.State, Missing("reviewed correction patch", "/pendingPatch"));
            if (workflow.Iteration >= workflow.MaximumIterations)
                return Result(workflow, ShapeReconstructionState.Failed,
                    Missing("correction inside the declared iteration bound", "/maximumIterations"));
            ShapePatchResult patch = new ShapePatchApplier().TryApply(workflow.Definition, workflow.PendingPatch);
            return new(
                patch.Succeeded ? ShapeReconstructionState.Constructing : workflow.State,
                patch.Succeeded ? workflow.Iteration + 1 : workflow.Iteration,
                patch.Succeeded ? patch.Definition : workflow.Definition,
                patch.Diagnostics);
        }

        private static ShapeReconstructionStepResult AdvanceQuality(ShapeReconstructionWorkflow workflow)
        {
            if (workflow.QualityPolicy == null)
                return Result(workflow, workflow.State, Missing("quality policy", "/qualityPolicy"));
            ShapeQualityReport report = new ShapeQualityGate().Evaluate(workflow.Definition, workflow.QualityPolicy);
            return Result(workflow,
                report.Passed ? ShapeReconstructionState.Completed : ShapeReconstructionState.Failed,
                report.Diagnostics);
        }

        private static ShapeReconstructionStepResult ValidateAndMove(
            ShapeReconstructionWorkflow workflow,
            ShapeDiagnosticReport diagnostics,
            ShapeReconstructionState next) =>
            Result(workflow, diagnostics.IsValid ? next : workflow.State, diagnostics);

        private static ShapeReconstructionStepResult Result(
            ShapeReconstructionWorkflow workflow,
            ShapeReconstructionState state,
            ShapeDiagnosticReport diagnostics) =>
            new(state, workflow?.Iteration ?? 0, workflow?.Definition, diagnostics);

        private static ShapeDiagnosticReport Missing(string artifact, string path) => new(new List<ShapeDiagnostic>
        {
            new("shape.reconstruction.artifact.missing", ShapeDiagnosticSeverity.Error,
                $"Reconstruction requires {artifact} at this stage.", path: path)
        });
    }
}
