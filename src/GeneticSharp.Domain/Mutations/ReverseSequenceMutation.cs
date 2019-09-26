using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
    public class ReverseSequenceMutation : SequenceMutationBase
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
        /// Mutate selected sequence.
        /// </summary>
        /// <returns>The resulted sequence after mutation operation.</returns>
        /// <param name="sequence">The sequence to be mutated.</param>
        protected override IEnumerable<T> MutateOnSequence<T>(IEnumerable<T> sequence)
        {
            return sequence.Reverse();
        }
        #endregion
    }
}
