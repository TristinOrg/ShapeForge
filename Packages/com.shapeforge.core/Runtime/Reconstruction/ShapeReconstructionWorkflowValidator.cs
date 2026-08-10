using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Validates persistent reconstruction identity, bounds, and current candidate state.
    /// </summary>
    public sealed class ShapeReconstructionWorkflowValidator
    {
        /// <summary>Returns bounded workflow diagnostics without advancing state.</summary>
        public ShapeDiagnosticReport Analyze(ShapeReconstructionWorkflow workflow)
        {
            List<ShapeDiagnostic> diagnostics = new();
            if (workflow == null)
            {
                diagnostics.Add(Error("shape.reconstruction.null", "Reconstruction workflow cannot be null.", "/"));
                return new(diagnostics);
            }
            if (workflow.Schema != ShapeReconstructionWorkflow.CurrentSchema)
                diagnostics.Add(Error("shape.reconstruction.schema", $"Unsupported reconstruction schema '{workflow.Schema}'.", "/schema"));
            if (string.IsNullOrWhiteSpace(workflow.Id))
                diagnostics.Add(Error("shape.reconstruction.id", "Reconstruction workflow requires a stable ID.", "/id"));
            if (workflow.MaximumIterations < 1 || workflow.MaximumIterations > 100)
                diagnostics.Add(Error("shape.reconstruction.iterations.limit", "Maximum iterations must be between 1 and 100.", "/maximumIterations"));
            if (workflow.Iteration < 0 || workflow.Iteration > workflow.MaximumIterations)
                diagnostics.Add(Error("shape.reconstruction.iteration", "Iteration must remain inside the declared bound.", "/iteration"));
            if (workflow.Definition == null)
                diagnostics.Add(Error("shape.reconstruction.definition", "Reconstruction requires a current definition.", "/definition"));
            else
            {
                foreach (ShapeDiagnostic diagnostic in new ShapeDefinitionValidator().Analyze(workflow.Definition).Diagnostics)
                    diagnostics.Add(diagnostic);
            }
            return new(diagnostics);
        }

        private static ShapeDiagnostic Error(string code, string message, string path) =>
            new(code, ShapeDiagnosticSeverity.Error, message, path: path);
    }
}
