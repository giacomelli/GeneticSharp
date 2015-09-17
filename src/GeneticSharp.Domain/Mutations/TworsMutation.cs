using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Twors mutation allows the exchange of position of two genes randomly chosen.
    /// <remarks>
    /// <see href="http://arxiv.org/ftp/arxiv/papers/1203/1203.3099.pdf">Analyzing the Performance of Mutation Operators to Solve the Travelling Salesman Problem</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Twors")]
    public class TworsMutation : MutationBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TworsMutation"/> class.
        /// </summary>
        public TworsMutation()
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
            if (RandomizationProvider.Current.GetDouble() <= probability)
            {
                var indexes = RandomizationProvider.Current.GetUniqueInts(2, 0, chromosome.Length);
                var firstIndex = indexes[0];
                var secondIndex = indexes[1];
                var firstGene = chromosome.GetGene(firstIndex);
                var secondGene = chromosome.GetGene(secondIndex);

                chromosome.ReplaceGene(firstIndex, secondGene);
                chromosome.ReplaceGene(secondIndex, firstGene);
            }
        }
        #endregion
    }
}
