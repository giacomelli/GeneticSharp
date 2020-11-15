using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Reinsertions
{
    /// <summary>
    /// Uniform Reinsertion.
    /// <remarks>
    /// When there are less offspring than parents, select the offspring uniformly at random to be reinserted, the parents are discarded. 
    /// <see href="http://old.usb-bg.org/Bg/Annual_Informatics/2011/SUB-Informatics-2011-4-29-35.pdf">Generalized Nets Model of offspring Reinsertion in Genetic Algorithm</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Uniform")]
    public class UniformReinsertion : ReinsertionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.UniformReinsertion"/> class.
        /// </summary>
        public UniformReinsertion() : base(true, true)
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
            if (offspring.Count == 0)
            {
                throw new ReinsertionException(this, "The minimum size of the offspring is 1.");
            }

            var rnd = RandomizationProvider.Current;
            // See the aforementioned publication: produce  less  offspring  than  parents,  and  replace  parents  uniformly at random

            var parentsToSelect =  population.MinSize - offspring.Count;
            if (parentsToSelect > 0)
            {
                var parentIds = rnd.GetInts(parentsToSelect, 0, parents.Count);
                foreach (var parentId in parentIds)
                {
                    offspring.Add(parents[parentId]);
                }

                return offspring;
            }
            else
            {
                //In order to support population collapse, and to keep the original intent, that is keep some parents together with offsping, we'll replace parents uniformly at random by random offspring
                var parentIds = rnd.GetInts(parents.Count, 0, parents.Count);
                var childrenIds = rnd.GetInts(parents.Count, 0, offspring.Count);
                for (int i = 0; i < parents.Count; i++)
                {
                    parents[parentIds[i]] = offspring[childrenIds[i]];
                }

                return parents;

            }

        }
        #endregion
    }
}
