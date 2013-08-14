using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// The Uniform Crossover uses a fixed mixing ratio between two parents. 
    /// Unlike one- and two-point crossover, the Uniform Crossover enables the parent chromosomes to contribute the gene level rather than the segment level.
    /// <remarks>
    /// If the mixing ratio is 0.5, the offspring has approximately half of the genes from first parent and the other half from second parent, although cross over points can be randomly chosen.
    /// </remarks>
    /// http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#Uniform_Crossover_and_Half_Uniform_Crossover
    /// </summary>
    public class UniformCrossover : CrossoverBase
    {
        #region Constructors
        public UniformCrossover(float mixProbability)
            : base(2, 2)
        {
            MixProbability = mixProbability;
        }
        #endregion

        #region Properties
        public float MixProbability { get; private set; }
        #endregion

        #region Methods
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)        
        {
            var firstParent = parents[0];
            var secondParent = parents[1];
            var firstChild = firstParent.CreateNew();
            var secondChild = secondParent.CreateNew();

            for (int i = 0; i < firstParent.Length; i++)
            {
                if (RandomizationProvider.Current.GetDouble() < MixProbability)
                {
                    firstChild.ReplaceGene(i, firstParent.GetGene(i));
                    secondChild.ReplaceGene(i, secondParent.GetGene(i));
                }
                else
                {
					firstChild.ReplaceGene(i, secondParent.GetGene(i));
					secondChild.ReplaceGene(i, firstParent.GetGene(i));
                }
            }

            return new List<IChromosome> { firstChild, secondChild };
        }
        #endregion
    }
}
