using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Populations
{
    /// <summary>
    /// Represents a population of candidate solutions (chromosomes) using TPL to create them.
    /// </summary>
    public class TplPopulation : Population
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Populations.TplPopulation"/> class.
        /// </summary>
        /// <param name="minSize">The minimum size (chromosomes).</param>
        /// <param name="maxSize">The maximum size (chromosomes).</param>
        /// <param name="adamChromosome">The original chromosome of all population ;).</param>
        public TplPopulation(int minSize, int maxSize, IChromosome adamChromosome) : base(minSize, maxSize, adamChromosome)
        {
        }
        #endregion

        #region Public methods
      

        protected override IList<IChromosome> CreateNewChromosomes(int nbChromosomes)
        {
            var chromosomes = new ConcurrentBag<IChromosome>();
            Parallel.For(0, nbChromosomes, i =>
            {
                var c = CreateNewChromosome();

                chromosomes.Add(c);
            });


          
            return chromosomes.ToList();
        }


        #endregion
    }
}