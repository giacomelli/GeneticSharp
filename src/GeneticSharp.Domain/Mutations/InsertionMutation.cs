using System.ComponentModel;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Insertion Mutation.
    /// <remarks>
    /// In the insertion mutation operator, a gene is randomly selected from chromosome, is removed, then replaced at a randomly selected position. 
    /// On implementation, we take a sequence S limited by two positions i and j randomly chosen, The selected gene in this sequence 
    /// will be left shifted or right shifted randomly to give the effect of removing the gene and inserting it on another position.
    /// <see href="https://web.cs.elte.hu/blobs/diplomamunkak/bsc_alkmat/2017/keresztury_bence.pdf">Genetic algorithms and the Traveling Salesman Problem</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Insertion")]
    public class InsertionMutation : DisplacementMutation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertionMutation"/> class.
        /// </summary>
        public InsertionMutation()
        {
            IsOrdered = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines genes to shift.
        /// <returns>Count of genes to be shifted</returns>
        /// </summary>
        /// <param name="maxCount">max possible count of genes to shift.</param>
        protected override int DetermineGeneToShift(int maxCount)
        {
            return 1;
        }
        #endregion
    }
}
