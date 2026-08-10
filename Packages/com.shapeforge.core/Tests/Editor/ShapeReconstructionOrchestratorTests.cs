using NUnit.Framework;

namespace ShapeForge.Tests
{
    /// <summary>
    /// Verifies deterministic, bounded, and transactional reconstruction transitions.
    /// </summary>
    public sealed class ShapeReconstructionOrchestratorTests
    {
        [Test]
        public void CorrectionAppliesPatchAndAdvancesIteration()
        {
            ShapeReconstructionWorkflow workflow = Create(ShapeReconstructionState.Correcting);
            workflow.PendingPatch = new();

            ShapeReconstructionStepResult result = new ShapeReconstructionOrchestrator().Advance(workflow);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.State, Is.EqualTo(ShapeReconstructionState.Constructing));
            Assert.That(result.Iteration, Is.EqualTo(1));
            Assert.That(result.Definition, Is.Not.SameAs(workflow.Definition));
        }

        [Test]
        public void CorrectionStopsAtDeclaredIterationBound()
        {
            ShapeReconstructionWorkflow workflow = Create(ShapeReconstructionState.Correcting);
            workflow.Iteration         = 1;
            workflow.MaximumIterations = 1;
            workflow.PendingPatch      = new();

            ShapeReconstructionStepResult result = new ShapeReconstructionOrchestrator().Advance(workflow);

            Assert.That(result.State, Is.EqualTo(ShapeReconstructionState.Failed));
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public void QualityStageCompletesValidCandidate()
        {
            ShapeReconstructionWorkflow workflow = Create(ShapeReconstructionState.QualityChecking);
            workflow.QualityPolicy = new() { Id = "example/final" };
            workflow.QualityPolicy.RequiredNodeIds.Add("model");

            ShapeReconstructionStepResult result = new ShapeReconstructionOrchestrator().Advance(workflow);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.State, Is.EqualTo(ShapeReconstructionState.Completed));
        }

        private static ShapeReconstructionWorkflow Create(ShapeReconstructionState state) => new()
        {
            Id                = "example/reconstruction",
            State             = state,
            Definition        = new("Example", new("model", "Model", ShapeTypes.Group)),
            MaximumIterations = 1
        };
    }
}
