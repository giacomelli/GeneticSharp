using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Infrastructure.Framework.Texts;

namespace GeneticSharp.Domain.Chromosomes
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

        /// <summary>
        /// Initializes and returns a chromosome
        /// </summary>
        /// <typeparam name="TChromosome">the chromosome type</typeparam>
        /// <param name="chromosome">the chromosome being initialized</param>
        /// <returns></returns>
        public static TChromosome Initialized<TChromosome>(this TChromosome chromosome)
            where TChromosome : IChromosome
        {
            chromosome.InitializeGenes();
            return chromosome;
        }

        public static TChromosome FlipGene<TChromosome>(this TChromosome chromosome,int firstIndex, int secondIndex)
            where TChromosome : IChromosome
        {
            var firstGene = chromosome.GetGene(firstIndex);
            var secondGene = chromosome.GetGene(secondIndex);

            chromosome.ReplaceGene(firstIndex, secondGene);
            chromosome.ReplaceGene(secondIndex, firstGene);
            return chromosome;
        }


        public static EvolutionResult GetResult(this GeneticAlgorithm ga)
        {
            return new EvolutionResult() { Population = ga.Population, TimeEvolving = ga.TimeEvolving };
        }

    }
}