using System;
using System.Linq;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using HelperSharp;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Two-point crossover calls for two points to be selected on the parents. 
    /// Everything between the two points is swapped between the parents, rendering two children.
    /// http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#Two-point_crossover
    /// </summary>
    public class TwoPointCrossover : CrossoverBase
    {
        #region Fields
        private int m_swapPointOneGeneIndex;
        private int m_swapPointTwoGeneIndex;
        #endregion

        #region Constructors
        public TwoPointCrossover(int swapPointOneGeneIndex, int swapPointTwoGeneIndex)
            : base(2, 2)
        {
            if (swapPointOneGeneIndex >= swapPointTwoGeneIndex)
            {
                throw new ArgumentOutOfRangeException("swapPointTwoGeneIndex", "The the swap point two index should be greater than swap point one index.");
            }

            m_swapPointOneGeneIndex = swapPointOneGeneIndex;
            m_swapPointTwoGeneIndex = swapPointTwoGeneIndex;
        }
        #endregion

        #region Methods
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {            
            var firstParent = parents[0];
            var secondParent = parents[1];
            var parentLength = firstParent.Length;

            if (parentLength < 3)
            {
                throw new ArgumentOutOfRangeException("parents", "A Two Point Crossover needs chromosomes with, at least, 3 genes.");
            }

            var swapPointsLength = parentLength - 1;


            if (m_swapPointTwoGeneIndex >= swapPointsLength)
            {
                throw new ArgumentOutOfRangeException(

                    "parents", "The swap point two index is {0}, but there is only {1} genes. The swap should result at least one gene to each sides."
                    .With(m_swapPointTwoGeneIndex, parentLength));
            }

            var firstChild = CreateChild(firstParent, secondParent);
            var secondChild = CreateChild(secondParent, firstParent);

            return new List<IChromosome>() { firstChild, secondChild };
        }

        private IChromosome CreateChild(IChromosome leftParent, IChromosome rightParent)
        {
            var firstCutGenesCount = m_swapPointOneGeneIndex + 1;
            var secondCutGenesCount = m_swapPointTwoGeneIndex + 1;
            var child = leftParent.CreateNew();
            child.ClearGenes();
            child.AddGenes(leftParent.GetGenes().Take(firstCutGenesCount));
            child.AddGenes(rightParent.GetGenes().Skip(firstCutGenesCount).Take(secondCutGenesCount - firstCutGenesCount));
            child.AddGenes(leftParent.GetGenes().Skip(secondCutGenesCount));

            return child;
        }
        #endregion
    }
}
