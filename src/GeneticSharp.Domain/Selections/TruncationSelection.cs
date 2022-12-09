using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GeneticSharp
{
    /// <summary>
    /// Selects the chromosomes with the best fitness.
    /// </summary>
    /// <remarks>
    /// <see href="https://en.wikipedia.org/wiki/Truncation_selection"/>
    /// </remarks>
    [DisplayName("Truncation")]
    public sealed class TruncationSelection : SelectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.TruncationSelection"/> class.
        /// </summary>
        public TruncationSelection() : base(2)
        {
        }        

        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>The select chromosomes.</returns>
        protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
        {
            var ordered = generation.Chromosomes.OrderByDescending(c => c.Fitness);
            return ordered.Take(number).ToList();
        }        
    }
}