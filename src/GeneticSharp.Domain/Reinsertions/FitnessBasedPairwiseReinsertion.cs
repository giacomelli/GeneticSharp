using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Reinsertions
{
    /// <summary>
    /// Fitness Based Pairwise reinsertion. For each index of parents/offsprings populations of same length, compares fitness of the parent and offspring individuals for that index, and keeps the one with better fitness.
    /// </summary>
    [DisplayName("Fitness Based Pairwise")]
    public class FitnessBasedPairwiseReinsertion : ReinsertionBase
    {
        public FitnessBasedPairwiseReinsertion():base(true, true)
        {
            
        }

        protected override IList<IChromosome> PerformSelectChromosomes(IPopulation population, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            if (parents.Count!=offspring.Count)
            {
                throw new NotImplementedException("FitnessBasedPairwiseReinsertion requires the offspring number to equals parents number");
            }

            for (int i = 0; i < parents.Count; i++)
            {
                if (parents[i].Fitness<offspring[i].Fitness)
                {
                    parents[i] = offspring[i];
                }
            }

            return parents;
        }
    }
}