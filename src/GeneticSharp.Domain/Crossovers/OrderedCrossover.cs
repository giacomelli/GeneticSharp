using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelperSharp;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.ComponentModel;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Ordered Crossover (OX1).
	/// <remarks>
	/// A portion of one parent is mapped to a portion of the other parent. 
    /// From the replaced portion on, the rest is filled up by the remaining genes, where already present genes are omitted and the order is preserved.
	/// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#Crossover_for_Ordered_Chromosomes">Crossover for Ordered Chromosomes</see>
    /// 
    /// The Ordered Crossover method is presented by Goldberg[8], is used when the problem is of order based, 
    /// for example in Ushaped assembly line balancing etc. Given two parent 
    /// chromosomes, two random crossover points are selected 
    /// partitioning them into a left, middle and right portion. The 
    /// ordered two-point crossover behaves in the following way: 
    /// child1 inherits its left and right section from parent1, and its middle section is determined.
	/// <see href="http://arxiv.org/ftp/arxiv/papers/1203/1203.3097.pdf>A Comparative Study of Adaptive Crossover Operators for Genetic Algorithms to Resolve the Traveling Salesman Problem</see>
    /// 
    /// Order 1 Crossover is a fairly simple permutation crossover. 
    /// Basically, a swath of consecutive alleles from parent 1 drops down, 
    /// and remaining values are placed in the child in the order which they appear in parent 2.
	/// <see href="http://www.rubicite.com/Tutorials/GeneticAlgorithms/CrossoverOperators/Order1CrossoverOperator.aspx">Order 1 Crossover</see>
	/// </remarks>
    /// </summary>
	[DisplayName("Ordered (OX1)")]
	public class OrderedCrossover : CrossoverBase
    {
        #region Constructors
        public OrderedCrossover()
			: base(2, 2)
        {
        }
        #endregion

        #region Methods
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
			var firstParent = parents [0];
			var secondParent = parents [1];

			if (firstParent.Length < 2) {
				throw new CrossoverException(this, "A chromosome should have, at least, 2 genes. {0} has only {1} gene.".With(firstParent.GetType().Name, firstParent.Length));
			}

			var middleSectionIndexes = RandomizationProvider.Current.GetInts (2, 0, firstParent.Length);
			var middleSectionBeginIndex = middleSectionIndexes [0];
			var middleSectionEndIndex = middleSectionIndexes [1];
			var firstChild = CreateChild (firstParent, secondParent, middleSectionBeginIndex, middleSectionEndIndex);
			var secondChild = CreateChild (secondParent, firstParent, middleSectionBeginIndex, middleSectionEndIndex);

			return new List<IChromosome> () { firstChild, secondChild };
        }

		private IChromosome CreateChild(IChromosome firstParent, IChromosome secondParent, int middleSectionBeginIndex, int middleSectionEndIndex)
		{
			var middleSectionGenes = firstParent.GetGenes ().Skip (middleSectionBeginIndex).Take ((middleSectionEndIndex - middleSectionBeginIndex) + 1);
			var secondParentRemainingGenes = secondParent.GetGenes ().Except (middleSectionGenes).GetEnumerator ();		
			var child = firstParent.CreateNew ();

			for(int i = 0; i < firstParent.Length; i++)
			{
				var firstParentGene = firstParent.GetGene(i);
			
				if (i >= middleSectionBeginIndex && i <= middleSectionEndIndex) {
					child.ReplaceGene (i, firstParentGene);
				} else {
					secondParentRemainingGenes.MoveNext ();
					child.ReplaceGene (i, secondParentRemainingGenes.Current);
				}
			}

			return child;
		}
        #endregion
    }
}
