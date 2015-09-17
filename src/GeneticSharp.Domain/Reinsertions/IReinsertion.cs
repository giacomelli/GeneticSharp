using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Reinsertions
{
    /// <summary>
    /// Defines an interface for reinsertions.
    /// <remarks>
    /// If less offspring are produced than the min size of the original population then to 
    /// maintain the size of the population, the offspring have to be reinserted 
    /// into the old population. Similarly, if not all offspring are to be used at each 
    /// generation or if more offspring are generated than the max size of the  
    /// population then a reinsertion scheme must be used to determine which individuals are to exist in the new population
    /// <see href="http://usb-bg.org/Bg/Annual_Informatics/2011/SUB-Informatics-2011-4-29-35.pdf">Generalized Nets Model of offspring Reinsertion in Genetic Algorithm</see>
    /// </remarks>
    /// </summary>
    public interface IReinsertion
    {
        #region Properties
        /// <summary>
        /// Gets a value indicating whether can collapse the number of selected chromosomes for reinsertion.
        /// </summary>
        bool CanCollapse { get; }

        /// <summary>
        /// Gets a value indicating whether can expand the number of selected chromosomes for reinsertion.
        /// </summary>
        bool CanExpand { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Selects the chromosomes which will be reinserted.
        /// </summary>
        /// <returns>The chromosomes to be reinserted in next generation..</returns>
        /// <param name="population">The population.</param>
        /// <param name="offspring">The offspring.</param>
        /// <param name="parents">The parents.</param>
        IList<IChromosome> SelectChromosomes(Population population, IList<IChromosome> offspring, IList<IChromosome> parents);
        #endregion
    }
}
