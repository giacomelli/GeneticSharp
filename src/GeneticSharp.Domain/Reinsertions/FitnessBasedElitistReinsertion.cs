using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Reinsertions
{
    /// <summary>
    /// Fitness Based Elitist reinsertion.
    /// <remarks>
    /// Select the best parents to be reinserted together with the Best offspring. 
    /// <see href="http://old.usb-bg.org/Bg/Annual_Informatics/2011/SUB-Informatics-2011-4-29-35.pdf">Generalized Nets Model of offspring Reinsertion in Genetic Algorithm</see>
    /// <see href="http://www.geatbx.de/docu/algindex-05.html">Evolutionary Algorithms: Principles, Methods and Algorithms</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Fitness Based Elitist")]
    public class FitnessBasedElitistReinsertion : ReinsertionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.FitnessBasedElitistReinsertion"/> class.
        /// </summary>
        public FitnessBasedElitistReinsertion() : base(true, true)
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
        protected override IList<IChromosome> PerformSelectChromosomes(IPopulation population, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            // As per the aforementioned publication, The elitist combined with fitness-based reinsertion prevents this losing of information and is the recommended method. At each generation, a given number of the least fit parents is replaced by the same number of the most fit offspring (Fig. 1, [5]).

            var concat = parents.Concat(offspring);
            return concat.OrderByDescending(p => p.Fitness).Take(population.MinSize).ToList();

        }
        #endregion
    }
}