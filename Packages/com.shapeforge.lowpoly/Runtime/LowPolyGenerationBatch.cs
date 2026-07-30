using System;
using UnityEngine;

namespace ShapeForge.LowPoly
{
    /// <summary>
    /// Spreads repeated model generation across caller-controlled steps without scheduler allocations.
    /// </summary>
    public sealed class LowPolyGenerationBatch
    {
        private readonly LowPolyModelGenerator generator;
        private readonly ShapeDefinition       definition;
        private readonly Transform             parent;
        private readonly Action<GameObject>    onGenerated;

        internal LowPolyGenerationBatch(
            LowPolyModelGenerator generator,
            ShapeDefinition       definition,
            int                   totalCount,
            Transform             parent,
            Action<GameObject>    onGenerated)
        {
            this.generator   = generator;
            this.definition  = definition;
            this.parent      = parent;
            this.onGenerated = onGenerated;
            TotalCount       = totalCount;
        }

        /// <summary>
        /// Gets the requested number of model instances.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Gets the number of model instances generated so far.
        /// </summary>
        public int GeneratedCount { get; private set; }

        /// <summary>
        /// Gets whether every requested model has been generated.
        /// </summary>
        public bool IsCompleted => GeneratedCount >= TotalCount;

        /// <summary>
        /// Generates up to the supplied model budget and returns the generated count for this step.
        /// </summary>
        public int GenerateNext(int modelBudget)
        {
            if (modelBudget <= 0)
                throw new ArgumentOutOfRangeException(nameof(modelBudget));

            int stepCount = Math.Min(modelBudget, TotalCount - GeneratedCount);
            for (int index = 0; index < stepCount; index++)
            {
                GameObject generated = generator.Generate(definition, parent);
                GeneratedCount++;
                onGenerated?.Invoke(generated);
            }

            return stepCount;
        }
    }
}
