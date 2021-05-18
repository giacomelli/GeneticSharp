using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            var maxCrossIndex = Math.Min(parents.Count, offspring.Count);
            var toReturn = new List<IChromosome>(parents);
            for (int i = 0; i < maxCrossIndex; i++)
            {
                if (toReturn[i].Fitness<offspring[i].Fitness)
                {
                    toReturn[i] = offspring[i];
                }
            }

            var missingNb = population.MinSize - toReturn.Count;
            if (missingNb>0)
            {
                if (offspring.Count>maxCrossIndex)
                {
                    var leftOver = offspring.Skip(maxCrossIndex).ToArray();
                    if (leftOver.Length>=missingNb)
                    {
                        foreach (var chromosome in leftOver.Take(missingNb))
                        {
                            toReturn.Add(chromosome);
                        }
                    }
                }
            }

            return toReturn;
        }
    }
}