using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using GeneticSharp.Infrastructure.Framework.Texts;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Base class for Mutations on a Sub-Sequence.
    /// </summary>
    public abstract class SequenceMutationBase : MutationBase
    {
        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            ValidateLength(chromosome);

            if (RandomizationProvider.Current.GetDouble() <= probability)
            {
                var indexes = RandomizationProvider.Current.GetUniqueInts(2, 0, chromosome.Length).OrderBy(i => i).ToArray();
                var firstIndex = indexes[0];
                var secondIndex = indexes[1];
                var sequenceLength = (secondIndex - firstIndex) + 1;

                var mutatedSequence = MutateOnSequence(chromosome.GetGenes().Skip(firstIndex).Take(sequenceLength)).ToArray();
                
                chromosome.ReplaceGenes(firstIndex, mutatedSequence);
            }
        }

        /// <summary>
        /// Validate length of the chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        protected virtual void ValidateLength(IChromosome chromosome)
        {
            if (chromosome.Length < 3)
            {
                throw new MutationException(this, "A chromosome should have, at least, 3 genes. {0} has only {1} gene.".With(chromosome.GetType().Name, chromosome.Length));
            }
        }

        /// <summary>
        /// Mutate selected sequence.
        /// </summary>
        /// <returns>The resulted sequence after mutation operation.</returns>
        /// <param name="sequence">The sequence to be mutated.</param>
        protected abstract IEnumerable<T> MutateOnSequence<T>(IEnumerable<T> sequence);
        #endregion
    }
}
