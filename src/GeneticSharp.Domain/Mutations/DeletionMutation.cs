using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Replaces a gene with the supplied replacement gene, which should do nothing. Useful for search criteria allowing for a wider search space.
    /// </summary>
    public class DeletionMutation :MutationBase
    {
        private Gene _replacementGene;

        public DeletionMutation(Gene replacementGene) :base()
        {
            _replacementGene = replacementGene;
        }

        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            ValidateLength(chromosome);

            if (RandomizationProvider.Current.GetDouble() <= probability)
            {
                int deletionIndex = RandomizationProvider.Current.GetInt(0, chromosome.Length-1);

                chromosome.ReplaceGene(deletionIndex, _replacementGene);


            }
        }

        /// <summary>
        /// Validate length of the chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        protected virtual void ValidateLength(IChromosome chromosome)
        {
            if (chromosome.Length < 3)
            {
                throw new MutationException(this, "A chromosome should have, at least, 3 genes. {0} has only {1} gene.".With(chromosome.GetType().Name, chromosome.Length));
            }
        }
    }
}
