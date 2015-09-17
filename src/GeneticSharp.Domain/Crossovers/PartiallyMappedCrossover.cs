using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Partially mapped crossover (PMX).
    /// <remarks>
    /// The partially-mapped crossover operator was suggested by Gold- berg and Lingle (1985). 
    /// It passes on ordering and value information from the parent tours to the offspring tours. 
    /// A portion of one parent’s string is mapped onto a portion of the other parent’s string and the remaining information is exchanged.
    /// <see href="http://lev4projdissertation.googlecode.com/svn-history/r100/trunk/reading/read/aiRev99.pdf">Genetic Algorithms for the Travelling Salesman Problem - A Review of Representations and Operators</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Partially Mapped (PMX)")]
    public class PartiallyMappedCrossover : CrossoverBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.PartiallyMappedCrossover"/> class.
        /// </summary>
        public PartiallyMappedCrossover() : base(2, 2, 3)
        {
            IsOrdered = true;
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
            if (parents.AnyHasRepeatedGene())
            {
                throw new CrossoverException(this, "The Partially Mapped Crossover (PMX) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }

            var parent1 = parents[0];
            var parent2 = parents[1];

            var cutPointsIndexes = RandomizationProvider.Current.GetUniqueInts(2, 0, parent1.Length);
            var firstCutPointIndex = cutPointsIndexes[0];
            var secondCutPointIdnex = cutPointsIndexes[1];

            var parent1Genes = parent1.GetGenes();
            var parent1MappingSection = parent1Genes.Skip(firstCutPointIndex).Take((secondCutPointIdnex - firstCutPointIndex) + 1).ToArray();

            var parent2Genes = parent2.GetGenes();
            var parent2MappingSection = parent2Genes.Skip(firstCutPointIndex).Take((secondCutPointIdnex - firstCutPointIndex) + 1).ToArray();

            var offspring1 = parent1.CreateNew();
            var offspring2 = parent2.CreateNew();

            offspring2.ReplaceGenes(firstCutPointIndex, parent1MappingSection);
            offspring1.ReplaceGenes(firstCutPointIndex, parent2MappingSection);

            var length = parent1.Length;

            for (int i = 0; i < length; i++)
            {
                if (i >= firstCutPointIndex && i <= secondCutPointIdnex)
                {
                    continue;
                }

                var geneForoffspring1 = GetGeneNotInMappingSection(parent1Genes[i], parent2MappingSection, parent1MappingSection);
                offspring1.ReplaceGene(i, geneForoffspring1);

                var geneForoffspring2 = GetGeneNotInMappingSection(parent2Genes[i], parent1MappingSection, parent2MappingSection);
                offspring2.ReplaceGene(i, geneForoffspring2);
            }

            return new List<IChromosome>() { offspring1, offspring2 };
        }

        private Gene GetGeneNotInMappingSection(Gene candidateGene, Gene[] mappingSection, Gene[] otherParentMappingSection)
        {
            var indexOnMappingSection = mappingSection
                .Select((item, index) => new { Gene = item, Index = index })
                .FirstOrDefault(g => g.Gene.Equals(candidateGene));

            if (indexOnMappingSection != null)
            {
                return GetGeneNotInMappingSection(otherParentMappingSection[indexOnMappingSection.Index], mappingSection, otherParentMappingSection);
            }

            return candidateGene;
        }
        #endregion
    }
}
