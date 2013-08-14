using System;
using System.Linq;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using HelperSharp;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// A single crossover point on both parents is selected. 
    /// All data beyond that point in either is swapped between the two parents.     
    /// http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#One-point_crossover
    /// <example>
    /// Parents: 
    /// |0|0|0| x |1|1|1|
    /// Have two swap points indexes: 0 and 1.
    /// 
    /// 1) 
    /// new OnePointCrossover(0);
    /// Children result:
    /// |0|1|1| and |1|0|0|
    /// 
    /// 2) 
    /// new OnePointCrossover(1);
    /// Children result:
    /// |0|0|1| and |1|1|0|
    /// </example>
    /// </summary>
    public sealed class OnePointCrossover : CrossoverBase
    {
        #region Fields
        private int m_swapPointIndex;
        #endregion

        #region Constructors
        public OnePointCrossover(int swapPointIndex) : base(2, 2)
        {
            m_swapPointIndex = swapPointIndex;
        }
        #endregion

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var firstParent = parents[0];
            var secondParent = parents[1];

            var swapPointsLength = firstParent.Length - 1;

            if (m_swapPointIndex >= swapPointsLength)
            {
                throw new ArgumentOutOfRangeException(
                    "parents", "The swap point index is {0}, but there is only {1} genes. The swap should result at least one gene to each side."
                    .With(m_swapPointIndex, firstParent.Length));
            }
            
            var firstChild = CreateChild(firstParent, secondParent);
            var secondChild = CreateChild(secondParent, firstParent);
            
            return new List<IChromosome>() { firstChild, secondChild };
        }

        private IChromosome CreateChild(IChromosome leftParent, IChromosome rightParent)
        {
            var cutGenesCount = m_swapPointIndex + 1;
            var child = leftParent.CreateNew();
            child.ReplaceGenes(0, leftParent.GetGenes().Take(cutGenesCount).ToArray());
			child.ReplaceGenes(cutGenesCount, rightParent.GetGenes().Skip(cutGenesCount).ToArray());
            
            return child;
        }
    }
}
