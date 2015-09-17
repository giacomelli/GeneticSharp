using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Reinsertions
{
    /// <summary>
    /// Pure Reinsertion.
    /// <remarks>
    /// When there are same number of offspring than parents, select the offspring to be reinserted, the parents are discarded. 
    /// <see href="http://usb-bg.org/Bg/Annual_Informatics/2011/SUB-Informatics-2011-4-29-35.pdf">Generalized Nets Model of offspring Reinsertion in Genetic Algorithm</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Pure")]
    public class PureReinsertion : ReinsertionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.PureReinsertion"/> class.
        /// </summary>
        public PureReinsertion() : base(false, false)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Selects the chromosomes which will be reinserted.
        /// </summary>
        /// <returns>The chromosomes to be reinserted in next generation..</returns>
        /// <param name="population">The population.</param>
        /// <param name="offspring">The offspring.</param>
        /// <param name="parents">The parents.</param>
        protected override IList<IChromosome> PerformSelectChromosomes(Population population, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return offspring;
        }
        #endregion
    }
}