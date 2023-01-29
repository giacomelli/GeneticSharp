using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp
{
    /// <summary>
    /// A base class for selection.
    /// </summary>
    public abstract class SelectionBase : ISelection
    {
        readonly int m_minNumberChromosomes;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.SelectionBase"/> class.
        /// </summary>
        /// <param name="minNumberChromosomes">Minimum number chromosomes support to be selected.</param>
        protected SelectionBase(int minNumberChromosomes)
        {
            m_minNumberChromosomes = minNumberChromosomes;
        }
        
        /// <summary>
        /// Selects the number of chromosomes from the generation specified.
        /// </summary>
        /// <returns>The selected chromosomes.</returns>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        public IList<IChromosome> SelectChromosomes(int number, Generation generation)
        {
            if (number < m_minNumberChromosomes)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "The number of selected chromosomes should be at least {0}.".With(m_minNumberChromosomes));
            }

            ExceptionHelper.ThrowIfNull("generation", generation);

            if (generation.Chromosomes.Any(c => !c.Fitness.HasValue))
            {
                throw new SelectionException(
                       this,
                       "There are chromosomes with null fitness.");
            }

            return PerformSelectChromosomes(number, generation);
        }

        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <returns>The selected chromosomes.</returns>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        protected abstract IList<IChromosome> PerformSelectChromosomes(int number, Generation generation);        
    }
}