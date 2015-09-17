using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Reinsertions
{
    /// <summary>
    /// Elitist reinsertion.
    /// <remarks>
    /// When there are less offspring than parents, select the best parents to be reinserted together with the offspring. 
    /// <see href="http://usb-bg.org/Bg/Annual_Informatics/2011/SUB-Informatics-2011-4-29-35.pdf">Generalized Nets Model of offspring Reinsertion in Genetic Algorithm</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Elitist")]
    public class ElitistReinsertion : ReinsertionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.ElitistReinsertion"/> class.
        /// </summary>
        public ElitistReinsertion() : base(false, true)
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
            var diff = population.MinSize - offspring.Count;

            if (diff > 0)
            {
                var bestParents = parents.OrderByDescending(p => p.Fitness).Take(diff);

                foreach (var p in bestParents)
                {
                    offspring.Add(p);
                }
            }

            return offspring;
        }
        #endregion
    }
}