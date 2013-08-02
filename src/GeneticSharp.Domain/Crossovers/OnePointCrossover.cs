using System;
using System.Linq;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using HelperSharp;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#One-point_crossover
    /// </summary>
    public sealed class OnePointCrossover : CrossoverBase
    {
        #region Fields
        private int m_swapPointGeneIndex;
        #endregion

        #region Constructors
        public OnePointCrossover(int swapPointGeneIndex) : base(2, 2)
        {
            m_swapPointGeneIndex = swapPointGeneIndex;
        }
        #endregion

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var firstParent = parents[0];
            var secondParent = parents[1];

            var genesLength = firstParent.Length;

            if (genesLength <= m_swapPointGeneIndex)
            {
                throw new ArgumentOutOfRangeException(
                    "parents", "The swap point gene index is {0}, but there is only {1} genes. The swap should result at least one gene to each side."
                    .With(m_swapPointGeneIndex, genesLength));
            }
            
            var firstChild = CreateChild(firstParent, secondParent);
            var secondChild = CreateChild(secondParent, firstParent);
            
            return new List<IChromosome>() { firstChild, secondChild };
        }

        private IChromosome CreateChild(IChromosome leftParent, IChromosome rightParent)
        {
            var child = leftParent.CreateNew();
            child.ClearGenes();
			child.AddGenes (leftParent.GetGenes().Take(m_swapPointGeneIndex));
			child.AddGenes (rightParent.GetGenes().Skip(m_swapPointGeneIndex));
            
            return child;
        }
    }
}
