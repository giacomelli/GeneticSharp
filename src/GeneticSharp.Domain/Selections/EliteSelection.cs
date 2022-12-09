using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GeneticSharp
{
    /// <summary>
    /// Selects the chromosomes with the best fitness and a small portion of the best individuals
    /// from the last generation is carried over (without any changes) to the next one.
    /// </summary>
    /// <remarks>
    /// <see href="https://en.wikipedia.org/wiki/Selection_(genetic_algorithm)">Wikipedia</see>
    /// </remarks>
    [DisplayName("Elite")]
    public sealed class EliteSelection : SelectionBase
    {
        readonly int _previousGenerationChromosomesNumber;
        List<IChromosome> _previousGenerationChromosomes;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.EliteSelection"/> class.
        /// </summary>
        public EliteSelection() 
            : this(1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.EliteSelection"/> class.
        /// </summary>
        /// <param name="previousGenerationChromosomesNumber">The number of best chromosomes of the previous generation to carried over to the next one.</param>
        public EliteSelection(int previousGenerationChromosomesNumber) 
            : base(2)
        {
            _previousGenerationChromosomesNumber = previousGenerationChromosomesNumber;
        }

        #region ISelection implementation
        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>The select chromosomes.</returns>
        protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
        {
            if(generation.Number == 1)
                _previousGenerationChromosomes = new List<IChromosome>();

            _previousGenerationChromosomes.AddRange(generation.Chromosomes);

            var ordered = _previousGenerationChromosomes.OrderByDescending(c => c.Fitness);
            var result = ordered.Take(number).ToList();

            _previousGenerationChromosomes = result.Take(_previousGenerationChromosomesNumber).ToList();            

            return result;
        }

        #endregion
    }
}