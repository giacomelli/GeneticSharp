using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using System.Collections.Generic;

namespace GeneticSharp.Domain.Selections {
    /// <summary>
    /// Selects random chromosomes.
    /// </summary>
    public class RandomSelection : SelectionBase {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSelection"/> class.
        /// </summary>
        public RandomSelection() : base(1) { }

        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>The select chromosomes.</returns>
        protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation) {
            var selected = new List<IChromosome>(number);
            var chromosomes = generation.Chromosomes;
            int[] selectedIndices = RandomizationProvider.Current.GetUniqueInts(number, 0, chromosomes.Count);
            foreach (int index in selectedIndices) {
                selected.Add(chromosomes[index]);
            }
            return selected;
        }
    }
}
