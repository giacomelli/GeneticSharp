using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Texts;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Two-Point Crossover (C2)
    /// <remarks>
    /// Two-point crossover calls for two points to be selected on the parents. 
    /// Everything between the two points is swapped between the parents, rendering two children.
    /// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#Two-point_crossover">Wikipedia</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Two-Point")]
    public class TwoPointCrossover : OnePointCrossover
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.TwoPointCrossover"/> class.
        /// </summary>
        /// <param name="swapPointOneGeneIndex">Swap point one gene index.</param>
        /// <param name="swapPointTwoGeneIndex">Swap point two gene index.</param>
        public TwoPointCrossover(int swapPointOneGeneIndex, int swapPointTwoGeneIndex)
        {
            if (swapPointOneGeneIndex >= swapPointTwoGeneIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(swapPointTwoGeneIndex), "The swap point two gene index should be greater than swap point one index.");
            }

            SwapPointOneGeneIndex = swapPointOneGeneIndex;
            SwapPointTwoGeneIndex = swapPointTwoGeneIndex;
            MinChromosomeLength = 3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.TwoPointCrossover"/> class.
        /// </summary>
        public TwoPointCrossover() : this(0, 1)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the index of the swap point one gene.
        /// </summary>
        /// <value>The index of the swap point one gene.</value>
        public int SwapPointOneGeneIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the swap point two gene.
        /// </summary>
        /// <value>The index of the swap point two gene.</value>
        public int SwapPointTwoGeneIndex { get; set; }
        #endregion

        #region Methods       
        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>
        /// The offspring (children) of the parents.
        /// </returns>
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var firstParent = parents[0];
            var secondParent = parents[1];
            var parentLength = firstParent.Length;
            var swapPointsLength = parentLength - 1;

            if (SwapPointTwoGeneIndex >= swapPointsLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(parents),
                    "The swap point two index is {0}, but there is only {1} genes. The swap should result at least one gene to each sides.".With(SwapPointTwoGeneIndex, parentLength));
            }

            return CreateChildren(firstParent, secondParent);
        }

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="leftParent">Left parent.</param>
        /// <param name="rightParent">Right parent.</param>
        protected override IChromosome CreateChild(IChromosome leftParent, IChromosome rightParent)
        {
            var firstCutGenesCount = SwapPointOneGeneIndex + 1;
            var secondCutGenesCount = SwapPointTwoGeneIndex + 1;
            var child = leftParent.CreateNew();
            child.ReplaceGenes(0, leftParent.GetGenes().Take(firstCutGenesCount).ToArray());
            child.ReplaceGenes(firstCutGenesCount, rightParent.GetGenes().Skip(firstCutGenesCount).Take(secondCutGenesCount - firstCutGenesCount).ToArray());
            child.ReplaceGenes(secondCutGenesCount, leftParent.GetGenes().Skip(secondCutGenesCount).ToArray());

            return child;
        }
        #endregion
    }
}
