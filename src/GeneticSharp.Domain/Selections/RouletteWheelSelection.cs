using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Selections
{
    /// <summary>
    /// Roulette Wheel Selection
    /// <remarks>
    /// Is a kind of Fitness Proportionate Selection. 
    /// <see href=" http://watchmaker.uncommons.org/manual/ch03s02.html">Fitness-Proportionate Selection</see>
    /// <para>
    /// In the Roulette wheel selection method [Holland, 1992], the first step is to calculate the cumulative fitness of the 
    /// whole population through the sum of the fitness of all individuals. After that, the probability of selection is 
    /// calculated for each individual.
    /// </para>
    /// <para>
    /// Then, an array is built containing cumulative probabilities of the individuals. So, n random numbers are generated in the range 0 to fitness sum.
    /// and for each random number an array element which can have higher value is searched for. Therefore, individuals are selected according to their 
    /// probabilities of selection. 
    /// </para>
    /// <see href="http://en.wikipedia.org/wiki/Fitness_proportionate_selection">Wikipedia</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Roulette Wheel")]
    public class RouletteWheelSelection : SelectionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Selections.RouletteWheelSelection"/> class.
        /// </summary>
        public RouletteWheelSelection() : base(2)
        {
        }
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
            var chromosomes = generation.Chromosomes;
            var selected = new List<IChromosome>();
            var sumFitness = chromosomes.Sum(c => c.Fitness.Value);
            var rouleteWheel = new List<double>();
            var accumulativePercent = 0.0;

            foreach (var c in chromosomes)
            {
                accumulativePercent += c.Fitness.Value / sumFitness;
                rouleteWheel.Add(accumulativePercent);
            }

            for (int i = 0; i < number; i++)
            {
                var pointer = RandomizationProvider.Current.GetDouble();
                var chromosomeIndex = rouleteWheel.Select((value, index) => new { Value = value, Index = index }).FirstOrDefault(r => r.Value >= pointer).Index;
                selected.Add(chromosomes[chromosomeIndex]);
            }

            return selected;
        }
        #endregion
    }
}