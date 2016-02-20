using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Order-based crossover (OX2).
    /// <remarks>
    /// OX2 was suggested in connection with schedule problems, is a modification of the OX1 operator. 
    /// The OX2 operator selects at random several positions in a parent string, and the order of the elements in the 
    /// selected positions of this parent is imposed on the other parent. For example, consider the parents 
    /// (1 2 3 4 5 6 7 8) and (2 4 6 8 7 5 3 1), and suppose that in the second parent in the second, third 
    /// and sixth positions are selected. The elements in these positions are 4, 6 and 5 respectively. 
    /// In the first parent, these elements are present at the fourth, fifth and sixth positions. 
    /// Now the offspring are equal to parent 1 except in the fourth, fifth and sixth positions: (1 2 3 * * * 7 8). 
    /// We add the missing elements to the offspring in the same order in which they appear in the second parent. 
    /// This results in (1 2 3 4 6 5 7 8). Exchanging the role of the first parent and the second parent gives, 
    /// using the same selected positions, (2 4 3 8 7 5 6 1).
    /// </remarks>
    /// </summary>
    [DisplayName("Order-based (OX2)")]
    public class OrderBasedCrossover : CrossoverBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.OrderBasedCrossover"/> class.
        /// </summary>
        public OrderBasedCrossover()
            : base(2, 2)
        {
            IsOrdered = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>The offspring (children) of the parents.</returns>
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            ValidateParents(parents);

            var firstParent = parents[0];
            var secondParent = parents[1];

            var rnd = RandomizationProvider.Current;
            var swapIndexesLength = rnd.GetInt(1, firstParent.Length - 1);
            var swapIndexes = rnd.GetUniqueInts(swapIndexesLength, 0, firstParent.Length);            
            var firstChild = CreateChild(firstParent, secondParent, swapIndexes);
            var secondChild = CreateChild(secondParent, firstParent, swapIndexes);

            return new List<IChromosome>() { firstChild, secondChild };
        }

        /// <summary>
        /// Validates the parents.
        /// </summary>
        /// <param name="parents">The parents.</param>
        protected virtual void ValidateParents(IList<IChromosome> parents)
        {
            if (parents.AnyHasRepeatedGene())
            {
                throw new CrossoverException(this, "The Order-based Crossover (OX2) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }
        }

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <param name="firstParent">First parent.</param>
        /// <param name="secondParent">Second parent.</param>
        /// <param name="swapIndexes">The swap indexes.</param>
        /// <returns>
        /// The child.
        /// </returns>
        protected virtual IChromosome CreateChild(IChromosome firstParent, IChromosome secondParent, int[] swapIndexes)
        {
            // ...suppose that in the second parent in the second, third 
            // and sixth positions are selected. The elements in these positions are 4, 6 and 5 respectively...
            var secondParentSwapGenes = secondParent.GetGenes()
                .Select((g, i) => new { Gene = g, Index = i })
                .Where((g) => swapIndexes.Contains(g.Index))
                .ToArray();

            var firstParentGenes = firstParent.GetGenes();

            // ...in the first parent, these elements are present at the fourth, fifth and sixth positions...
            var firstParentSwapGenes = firstParentGenes
                .Select((g, i) => new { Gene = g, Index = i })
                .Where((g) => secondParentSwapGenes.Any(s => s.Gene == g.Gene))
                .ToArray();

            var child = firstParent.CreateNew();
            var secondParentSwapGensIndex = 0;

            for (int i = 0; i < firstParent.Length; i++)
            {
                // Now the offspring are equal to parent 1 except in the fourth, fifth and sixth positions.
                // We add the missing elements to the offspring in the same order in which they appear in the second parent.                
                if (firstParentSwapGenes.Any(f => f.Index == i))
                {
                    child.ReplaceGene(i, secondParentSwapGenes[secondParentSwapGensIndex++].Gene);
                }
                else
                {
                    child.ReplaceGene(i, firstParentGenes[i]);                    
                }
            }

            return child;
        }

        #endregion
    }
}
