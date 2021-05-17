using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Selections
{
    /// <summary>
    /// Selects the chromosomes with the best fitness.
    /// </summary>
    /// <remarks>
    /// Also know as: Truncation Selection.
    /// </remarks>    
    [DisplayName("Elite")]
    public sealed class EliteSelection : SelectionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Selections.EliteSelection"/> class.
        /// </summary>
        public EliteSelection() : base(2)
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Sorts the selected parents by fitness value descending
        /// </summary>
        public bool SortSelected { get; set; }

        #endregion

        #region ISelection implementation
        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>The select chromosomes.</returns>
        protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
        {
            var ordered = generation.Chromosomes.OrderByDescending(c => c.Fitness);
            if (SortSelected)
            {
                return ordered.Take(number).ToList();
            }

            if (generation.Chromosomes.Count > number)
            {
                var cutoff = ordered.Skip(number - 1).First().Fitness.Value;
                return generation.Chromosomes.Where(c => c.Fitness >= cutoff).Take(number).ToList();
            }

            return generation.Chromosomes;

        }

        #endregion
    }
}