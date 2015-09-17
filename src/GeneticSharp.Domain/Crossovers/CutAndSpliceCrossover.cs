using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Cut and Splice crossover.
    /// <remarks>
    /// Results in a change in length of the children strings. The reason for this difference is that each parent string has a separate choice of crossover point.
    /// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#.22Cut_and_splice.22">Wikipedia</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Cut and Splice")]
    public class CutAndSpliceCrossover : CrossoverBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CutAndSpliceCrossover"/> class.
        /// </summary>
        public CutAndSpliceCrossover()
            : base(2, 2)
        {
            IsOrdered = false;
        }
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
            var parent1 = parents[0];
            var parent2 = parents[1];

            // The minium swap point is 1 to safe generate a gene with at least two genes.
            var parent1Point = RandomizationProvider.Current.GetInt(1, parent1.Length) + 1;
            var parent2Point = RandomizationProvider.Current.GetInt(1, parent2.Length) + 1;

            var offspring1 = CreateOffspring(parent1, parent2, parent1Point, parent2Point);
            var offspring2 = CreateOffspring(parent2, parent1, parent2Point, parent1Point);

            return new List<IChromosome>() { offspring1, offspring2 };
        }

        private static IChromosome CreateOffspring(IChromosome leftParent, IChromosome rightParent, int leftParentPoint, int rightParentPoint)
        {
            var offspring = leftParent.CreateNew();

            offspring.Resize(leftParentPoint + (rightParent.Length - rightParentPoint));
            offspring.ReplaceGenes(0, leftParent.GetGenes().Take(leftParentPoint).ToArray());
            offspring.ReplaceGenes(leftParentPoint, rightParent.GetGenes().Skip(rightParentPoint).ToArray());

            return offspring;
        }
        #endregion
    }
}
