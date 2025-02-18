using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GeneticSharp
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
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.RouletteWheelSelection"/> class.
        /// </summary>
        public RouletteWheelSelection() : base(2)
        {
        }

        /// <summary>
        /// Selects from wheel.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="chromosomes">The chromosomes.</param>
        /// <param name="rouletteWheel">The roulette wheel.</param>
        /// <param name="getPointer">The get pointer.</param>
        /// <returns>The selected chromosomes.</returns>
        protected static IList<IChromosome> SelectFromWheel(int number, IList<IChromosome> chromosomes, IList<double> rouletteWheel, Func<double> getPointer)
        {
            var selected = new List<IChromosome>();

            for (int i = 0; i < number; i++)
            {
                var pointer = getPointer();

                var chromosome = rouletteWheel
                                        .Select((value, index) => new { Value = value, Index = index })
                                        .FirstOrDefault(r => r.Value >= pointer);

                if (chromosome != null)
                    selected.Add(chromosomes[chromosome.Index].Clone());
            }

            return selected;
        }

        /// <summary>
        /// Calculates the cumulative percent.
        /// </summary>
        /// <param name="chromosomes">The chromosomes.</param>
        /// <param name="rouletteWheel">The roulette wheel.</param>
        protected static void CalculateCumulativePercentFitness(IList<IChromosome> chromosomes, IList<double> rouletteWheel)
        {
            var sumFitness = chromosomes.Sum(c => c.Fitness!.Value);

            var cumulativePercent = 0.0;

            for (int i = 0; i < chromosomes.Count; i++)
            {
                cumulativePercent += chromosomes[i].Fitness!.Value / sumFitness;
                rouletteWheel.Add(cumulativePercent);
            }
        }

        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>The select chromosomes.</returns>
        protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
        {
            var chromosomes = generation.Chromosomes;
            var rouletteWheel = new List<double>();
            var rnd = RandomizationProvider.Current;

            CalculateCumulativePercentFitness(chromosomes, rouletteWheel);

            return SelectFromWheel(number, chromosomes, rouletteWheel, () => rnd.GetDouble());
        }        
    }
}