using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Randomizations;
using System.Linq;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Partial Shuffle Mutation (PSM).
    /// <remarks>
    /// In the partial shuffle mutation operator, we take a sequence S limited by two 
    /// positions i and j randomly chosen, such that i&lt;j. The gene order in this sequence 
    /// will be shuffled. Sequence will be shuffled until it becomes different than the starting order
    /// <see href="http://arxiv.org/ftp/arxiv/papers/1203/1203.3099.pdf">Analyzing the Performance of Mutation Operators to Solve the Travelling Salesman Problem</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Partial Shuffle (PSM)")]
    public class PartialShuffleMutation : SequenceMutationBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialShuffleMutation"/> class.
        /// </summary>
        public PartialShuffleMutation()
        {
            IsOrdered = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Mutate selected sequence.
        /// </summary>
        /// <returns>The resulted sequence after mutation operation.</returns>
        /// <param name="sequence">The sequence to be mutated.</param>
        protected override IEnumerable<T> MutateOnSequence<T>(IEnumerable<T> sequence)
        {
            // If there is at least two differente genes on source sequence,
            // Then is possible shuffle their in sequence.
            if (sequence.Distinct().Count() > 1)
            {
                var result = sequence.Shuffle(RandomizationProvider.Current);
              
                while (sequence.SequenceEqual(result))
                {
                    result = sequence.Shuffle(RandomizationProvider.Current);
                }

                return result; 
            }

            // All genes on sequence are equal, then sequence cannot be shuffled.
            return sequence;
        }
        #endregion
    }
}
