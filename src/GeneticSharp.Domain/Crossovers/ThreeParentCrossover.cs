using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Three Parent Crossover.
    /// <remarks>
    /// In this technique, the child is derived from three parents. 
    /// They are randomly chosen. Each bit of first parent is checked with bit of second parent whether they are same. 
    /// If same then the bit is taken for the offspring otherwise the bit from the third parent is taken for the offspring.
    /// 
    /// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#Three_parent_crossover">Wikipedia</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Three Parent")]
    public class ThreeParentCrossover : CrossoverBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreeParentCrossover"/> class.
        /// </summary>
        public ThreeParentCrossover()
            : base(3, 1)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">Parents.</param>
        /// <returns>
        /// The offspring (children) of the parents.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var p1 = parents[0];
            var p1Genes = p1.GetGenes();
            var p2Genes = parents[1].GetGenes();
            var p3Genes = parents[2].GetGenes();
            var offspring = p1.CreateNew();
            Gene p1Gene;

            for (int i = 0; i < p1.Length; i++)
            {
                p1Gene = p1Genes[i];

                if (p1Gene == p2Genes[i])
                {
                    offspring.ReplaceGene(i, p1Gene);
                }
                else
                {
                    offspring.ReplaceGene(i, p3Genes[i]);
                }
            }

            return new List<IChromosome>() { offspring };
        }
        #endregion
    }
}
