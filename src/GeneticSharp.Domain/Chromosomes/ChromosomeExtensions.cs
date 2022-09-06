using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp
{
    /// <summary>
    /// Chromosome extensions.
    /// </summary>
    public static class ChromosomeExtensions
    {
        /// <summary>
        /// Checks if any of the chromosomes has repeated gene.
        /// </summary>
        /// <remarks>
        /// This can happen when used with a IMutation's implementation that not keep the chromosome ordered, 
        /// like OnePointCrossover, TwoPointCrossover and UniformCrossover is combined with a ICrossover's implementation
        /// that need ordered chromosomes, like OX1 and PMX.
        /// </remarks>
        /// <returns><c>true</c>, if chromosome has repeated gene, <c>false</c> otherwise.</returns>
        /// <param name="chromosomes">The chromosomes.</param>
        public static bool AnyHasRepeatedGene(this IList<IChromosome> chromosomes)
        {
            for (int i = 0; i < chromosomes.Count; i++)
            {
                var c = chromosomes[i];
                var notRepeatedGenesLength = c.GetGenes().Distinct().Count();

                if (notRepeatedGenesLength < c.Length)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates the chromosomes.
        /// </summary>
        /// <param name="chromosomes">The chromosomes.</param>
        public static void ValidateGenes(this IList<IChromosome> chromosomes)
        {
            if (chromosomes.Any(c => c.GetGenes().Any(g => g.Value == null)))
            {
                throw new InvalidOperationException("The chromosome '{0}' is generating genes with null value.".With(chromosomes.First().GetType().Name));
            }
        }

        /// <summary>
        /// Validates the chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosomes.</param>
        public static void ValidateGenes(this IChromosome chromosome)
        {
            if (chromosome != null && chromosome.GetGenes().Any(g => g.Value == null))
            {
                throw new InvalidOperationException("The chromosome '{0}' is generating genes with null value.".With(chromosome.GetType().Name));
            }
        }
    }
}