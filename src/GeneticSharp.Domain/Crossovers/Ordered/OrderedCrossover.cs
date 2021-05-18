using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Ordered Crossover (OX1).
    /// <remarks>
    /// Also know as: Order Crossover.
    /// <para>
    /// A portion of one parent is mapped to a portion of the other parent. 
    /// From the replaced portion on, the rest is filled up by the remaining genes, where already present genes are omitted and the order is preserved.
    /// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#Crossover_for_Ordered_Chromosomes">Crossover for Ordered Chromosomes</see>
    /// </para>
    /// <para>
    /// The Ordered Crossover method is presented by Goldberg, is used when the problem is of order based, 
    /// for example in Ushaped assembly line balancing etc. Given two parent 
    /// chromosomes, two random crossover points are selected 
    /// partitioning them into a left, middle and right portion. The 
    /// ordered two-point crossover behaves in the following way: 
    /// child1 inherits its left and right section from parent1, and its middle section is determined.
    /// <see href="http://arxiv.org/ftp/arxiv/papers/1203/1203.3097.pdf">A Comparative Study of Adaptive Crossover Operators for Genetic Algorithms to Resolve the Traveling Salesman Problem</see>
    /// </para>
    /// <para>
    /// The order crossover operator (Figure 4) was proposed by Davis (1985). 
    /// The OX1 exploits a property of the path representation, that the order of cities (not their positions) are important. 
    /// <see href="http://lev4projdissertation.googlecode.com/svn-history/r100/trunk/reading/read/aiRev99.pdf">Genetic Algorithms for the Travelling Salesman Problem - A Review of Representations and Operators</see>
    /// </para>
    /// <para>
    /// Order 1 Crossover is a fairly simple permutation crossover. 
    /// Basically, a swath of consecutive alleles from parent 1 drops down, 
    /// and remaining values are placed in the child in the order which they appear in parent 2.
    /// <see href="http://www.rubicite.com/Tutorials/GeneticAlgorithms/CrossoverOperators/Order1CrossoverOperator.aspx">Order 1 Crossover</see>
    /// </para>
    /// </remarks>
    /// </summary>
    [DisplayName("Ordered (OX1)")]
    public sealed class OrderedCrossover : CrossoverBase, IWeightedCrossover
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.OrderedCrossover"/> class.
        /// </summary>
        public OrderedCrossover()
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
            var middleSectionIndexes = RandomizationProvider.Current.GetUniqueInts(2, 0, parents[0].Length);
            Array.Sort(middleSectionIndexes);
            return this.PerformSectionCross(parents, true, (0, middleSectionIndexes[0], middleSectionIndexes[1]));
        }


        /// <summary>
        /// The geometric version of the ordered crossover sets the length of the parent section length according to the weights for both children, without including the symmetrical child.
        /// </summary>
        /// <param name="parents">the parent chromosomes to cross</param>
        /// <param name="weights">the weight for each parent chromosome</param>
        /// <returns>the children of the crossover applied to the parents, given the parent weights</returns>
        public IList<IChromosome> PerformWeightedCross(IList<IChromosome> parents, IList<double> weights)
        {
            
            if (parents.Count!=2)
            {
                throw new InvalidOperationException(
                    "Geometric ordered crossover with more than 2 parents not yet supported");
            }

            var parentOne = parents[0];
            var parentTwo = parents[1];

            var firstParentLength = (int) (parents[0].Length * weights[0] / ((weights[0] + weights[1])));

            var firstChildFirstIndex = RandomizationProvider.Current.GetInt(0, parentOne.Length - firstParentLength);
            var secondChildFirstIndex = RandomizationProvider.Current.GetInt(0, firstParentLength);
            var firstChildMiddleSectionIndices = (0, firstChildFirstIndex, firstChildFirstIndex + firstParentLength);
            var secondChildMiddleSectionIndices = (1, secondChildFirstIndex, secondChildFirstIndex + parentOne.Length - firstParentLength);

            return this.PerformSectionCross(parents, false, firstChildMiddleSectionIndices,
                secondChildMiddleSectionIndices);
        }




        /// <summary>
        /// Performs orderd crossover given the indexes of the middle section
        /// </summary>
        /// <param name="parents">the parents to cross</param>
        /// <param name="middleSectionIndexes">an int array with begin and end indices for the middle section</param>
        /// <returns></returns>
        private IList<IChromosome> PerformSectionCross(IList<IChromosome> parents, bool generateSecondChild, params (int sectionParentIndex, int beginIndex, int endIndex) [] childMiddleSectionIndexes)
        {
            var parentOne = parents[0];
            var parentTwo = parents[1];

            if (parents.AnyHasRepeatedGene())
            {
                throw new CrossoverException(this, "The Ordered Crossover (OX1) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }

            var nbChildren = generateSecondChild ? 2 * childMiddleSectionIndexes.Length : childMiddleSectionIndexes.Length;
            var toReturn = new List<IChromosome>(nbChildren);
            foreach (var middleSectionIndexes in childMiddleSectionIndexes)
            {
                var middleSectionBeginIndex = middleSectionIndexes.beginIndex;
                var middleSectionEndIndex = middleSectionIndexes.endIndex;
                var firstChild = CreateChild(parentOne, parentTwo, middleSectionBeginIndex, middleSectionEndIndex);
                toReturn.Add(firstChild);
                if (generateSecondChild)
                {
                    var secondChild = CreateChild(parentTwo, parentOne, middleSectionBeginIndex, middleSectionEndIndex);
                    toReturn.Add(secondChild);
                }
                
            }

            return toReturn;
        }

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="firstParent">First parent.</param>
        /// <param name="secondParent">Second parent.</param>
        /// <param name="middleSectionBeginIndex">Middle section begin index.</param>
        /// <param name="middleSectionEndIndex">Middle section end index.</param>
        private static IChromosome CreateChild(IChromosome firstParent, IChromosome secondParent, int middleSectionBeginIndex, int middleSectionEndIndex)
        {
            var middleSectionGenes = firstParent.GetGenes().Skip(middleSectionBeginIndex).Take(middleSectionEndIndex - middleSectionBeginIndex + 1);

            using (var secondParentRemainingGenes = secondParent.GetGenes().Except(middleSectionGenes).GetEnumerator())
            {
                var child = firstParent.CreateNew();

                for (int i = 0; i < firstParent.Length; i++)
                {
                    var firstParentGene = firstParent.GetGene(i);

                    if (i >= middleSectionBeginIndex && i <= middleSectionEndIndex)
                    {
                        child.ReplaceGene(i, firstParentGene);
                    }
                    else
                    {
                        secondParentRemainingGenes.MoveNext();
                        child.ReplaceGene(i, secondParentRemainingGenes.Current);
                    }
                }

                return child;
            }
        }
        #endregion

       
    }
}
