using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using HelperSharp;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Base class for IMutation's implementation.
    /// </summary>
    public abstract class MutationBase : IMutation
    {
        #region Properties
        /// <summary>
        /// Gets if the operator is ordered (if can keep the chromosome order).
        /// </summary>
        public bool IsOrdered { get; protected set; }
        #endregion

        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        public void Mutate(IChromosome chromosome, float probability)
        {
            ExceptionHelper.ThrowIfNull("chromosome", chromosome);

            PerformMutate(chromosome, probability);
        }

        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        protected abstract void PerformMutate(IChromosome chromosome, float probability);
        #endregion
    }
}
