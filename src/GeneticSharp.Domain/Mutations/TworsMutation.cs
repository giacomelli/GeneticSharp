using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using HelperSharp;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Twors mutation allows the exchange of position of two genes randomly chosen.
    /// <remarks>
    /// <see href="http://arxiv.org/ftp/arxiv/papers/1203/1203.3099.pdf">Analyzing the Performance of Mutation Operators to Solve the Travelling Salesman Problem</see>
    /// </remarks>
    /// </summary>
    public class TworsMutation : IMutation
    {
        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        public void Mutate(IChromosome chromosome, float probability)
        {
            if (chromosome.Length < 2)
            {
                throw new MutationException(this, "A chromosome should have, at least, 2 genes. {0} has only {1} gene.".With(chromosome.GetType().Name, chromosome.Length));
            }

            if (RandomizationProvider.Current.GetDouble() <= probability)
            {
				var indexes = RandomizationProvider.Current.GetUniqueInts(2, 0, chromosome.Length);
                var firstIndex = indexes[0];
                var secondIndex = indexes[1];
                var firstGene = chromosome.GetGene(firstIndex);
                var secondGene = chromosome.GetGene(secondIndex);

                chromosome.ReplaceGene(firstIndex, secondGene);
                chromosome.ReplaceGene(secondIndex, firstGene);
            }
        }
        #endregion
    }
}
