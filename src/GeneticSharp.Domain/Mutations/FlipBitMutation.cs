using System;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Takes the chosen genome and inverts the bits (i.e. if the genome bit is 1, it is changed to 0 and vice versa).
    /// </summary>
    /// <remarks>
    /// When using this mutation the genetic algorithm should use IBinaryChromosome.
    /// </remarks>
    [DisplayName("Flip Bit")]
    public class FlipBitMutation : MutationBase
    {
        #region Fields
        private readonly IRandomization m_rnd;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Mutations.FlipBitMutation"/> class.
        /// </summary>
        public FlipBitMutation ()
        {
            m_rnd = RandomizationProvider.Current;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        protected override void PerformMutate (IChromosome chromosome, float probability)
        {
            var binaryChromosome = chromosome as IBinaryChromosome;

            if (binaryChromosome == null) 
            {
                throw new MutationException (this, "Needs a binary chromosome that implements IBinaryChromosome.");    
            }

            if (m_rnd.GetDouble() <= probability)
            {
                var index = m_rnd.GetInt(0, chromosome.Length);
                binaryChromosome.FlipGene (index);
            }
        }
        #endregion
    }
}

