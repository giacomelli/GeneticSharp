using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using HelperSharp;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Reverse Sequence Mutation (RSM).
    /// <remarks>
    /// In the reverse sequence mutation operator, we take a sequence S limited by two 
    /// positions i and j randomly chosen, such that i&lt;j. The gene order in this sequence 
    /// will be reversed by the same way as what has been covered in the previous operation.
    /// <see href="http://arxiv.org/ftp/arxiv/papers/1203/1203.3099.pdf">Analyzing the Performance of Mutation Operators to Solve the Travelling Salesman Problem</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Reverse Sequence (RSM)")]
    public class ReverseSequenceMutation : MutationBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseSequenceMutation"/> class.
        /// </summary>
        public ReverseSequenceMutation()
        {
            IsOrdered = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            if (chromosome.Length < 3)
            {
                throw new MutationException(this, "A chromosome should have, at least, 3 genes. {0} has only {1} gene.".With(chromosome.GetType().Name, chromosome.Length));
            }

            if (RandomizationProvider.Current.GetDouble() <= probability)
            {
                var indexes = RandomizationProvider.Current.GetUniqueInts(2, 0, chromosome.Length).OrderBy(i => i).ToArray();
                var firstIndex = indexes[0];
                var secondIndex = indexes[1];

                var revertedSequence = chromosome.GetGenes().Skip(firstIndex).Take((secondIndex - firstIndex) + 1).Reverse().ToArray();

                chromosome.ReplaceGenes(firstIndex, revertedSequence);
            }
        }
        #endregion
    }
}
