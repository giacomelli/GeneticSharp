using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Alternating-position (AP).
    /// <remarks>
    /// <para>
    /// The alternating position crossover operator (Larrañaga et al. 1996a) simply creates an offspring by selecting alternately the next 
    /// element of the first parent and the next element of the second parent, omitting the elements already present in the offspring
    /// </para>
    /// <para>
    /// For example, if parent 1 is (1 2 3 4 5 6 7 8) and parent 2 is (3 7 5 1 6 8 2 4) 
    /// the AP operator gives the following offspring: (1 3 2 7 5 4 6 8)
    /// </para>
    /// <para>
    /// Exchanging the parents results in (3 1 7 2 5 4 6 8).
    /// <see href="../docs/Genetic Algorithms for the Travelling Salesman Problem - A Review of Representations and Operators.pdf">Genetic Algorithms for the Travelling Salesman Problem: A Review of Representations and Operators</see>
    /// </para>
    /// </remarks>
    /// </summary>
    [DisplayName("Alternating-position (AP)")]
    public sealed class AlternatingPositionCrossover : CrossoverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.VotingRecombinationCrossover"/> class.
        /// </summary>
        public AlternatingPositionCrossover() : base(2, 2)
        {
            IsOrdered = true;
        }

        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>The offspring (children) of the parents.</returns>
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            if (parents.AnyHasRepeatedGene())
            {
                throw new CrossoverException(this, "The Alternating-position (AP) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }

            var p1 = parents[0];
            var p2 = parents[1];
            var child1 = CreateChild(p1, p2);
            var child2 = CreateChild(p2, p1);

            return new List<IChromosome> { child1, child2 };
        }

        private IChromosome CreateChild(IChromosome firstParent, IChromosome secondParent)
        {
            var child = firstParent.CreateNew();
            var childGenes = new Gene[firstParent.Length];
            var childGenesIndex = 0;

            for (int i = 0; i < firstParent.Length && childGenesIndex < firstParent.Length; i++)
            {
                AddChildGene(childGenes, ref childGenesIndex, firstParent.GetGene(i));

                // The childGenesIndes could be incremented by the previous AddChildGene call
                if (childGenesIndex < secondParent.Length)
                    AddChildGene(childGenes, ref childGenesIndex, secondParent.GetGene(i));
            }

            child.ReplaceGenes(0, childGenes);

            return child;
        }

        private void AddChildGene(Gene[] childGenes, ref int childGenesIndex, Gene parentGene)
        {
            if (!childGenes.Contains(parentGene))
            {
                childGenes[childGenesIndex] = parentGene;
                childGenesIndex++;
            }
        }
    }
}
