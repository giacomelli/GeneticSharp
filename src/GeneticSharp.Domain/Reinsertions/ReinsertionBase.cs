using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using HelperSharp;

namespace GeneticSharp.Domain.Reinsertions
{
    /// <summary>
    /// Base class for IReinsertion's implementations.
    /// </summary>
    public abstract class ReinsertionBase : IReinsertion
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.ReinsertionBase"/> class.
        /// </summary>
        /// <param name="canCollapse">If set to <c>true</c> can collapse the number of selected chromosomes for reinsertion.</param>
        /// <param name="canExpand">If set to <c>true</c> can expand the number of selected chromosomes for reinsertion.</param>
        protected ReinsertionBase(bool canCollapse, bool canExpand)
        {
            CanCollapse = canCollapse;
            CanExpand = canExpand;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether can collapse the number of selected chromosomes for reinsertion.
        /// </summary>
        public bool CanCollapse { get; private set; }

        /// <summary>
        /// Gets a value indicating whether can expand the number of selected chromosomes for reinsertion.
        /// </summary>
        public bool CanExpand { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Selects the chromosomes which will be reinserted.
        /// </summary>
        /// <returns>The chromosomes to be reinserted in next generation..</returns>
        /// <param name="population">The population.</param>
        /// <param name="offspring">The offspring.</param>
        /// <param name="parents">The parents.</param>
        public IList<IChromosome> SelectChromosomes(Population population, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            ExceptionHelper.ThrowIfNull("population", population);
            ExceptionHelper.ThrowIfNull("offspring", offspring);
            ExceptionHelper.ThrowIfNull("parents", parents);

            if (!CanExpand && offspring.Count < population.MinSize)
            {
                throw new ReinsertionException(this, "Cannot expand the number of chromosome in population. Try another reinsertion!");
            }

            if (!CanCollapse && offspring.Count > population.MaxSize)
            {
                throw new ReinsertionException(this, "Cannot collapse the number of chromosome in population. Try another reinsertion!");
            }

            return PerformSelectChromosomes(population, offspring, parents);
        }

        /// <summary>
        /// Selects the chromosomes which will be reinserted.
        /// </summary>
        /// <returns>The chromosomes to be reinserted in next generation..</returns>
        /// <param name="population">The population.</param>
        /// <param name="offspring">The offspring.</param>
        /// <param name="parents">The parents.</param>
        protected abstract IList<IChromosome> PerformSelectChromosomes(Population population, IList<IChromosome> offspring, IList<IChromosome> parents);
        #endregion
    }
}
