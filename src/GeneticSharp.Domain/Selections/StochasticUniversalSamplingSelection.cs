using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Selections
{
    /// <summary>
    /// Stochastic Universal Sampling.
    /// <remarks>
    /// Also know as: Roulette wheel selection.
    /// Is a kind of Fitness Proportionate Selection. 
    /// <see href=" http://watchmaker.uncommons.org/manual/ch03s02.html">Fitness-Proportionate Selection</see>
    /// <para>
    /// Stochastic Universal Sampling is an elaborately-named variation of roulette wheel selection. 
    /// Stochastic Universal Sampling ensures that the observed selection frequencies of each individual 
    /// are in line with the expected frequencies. So if we have an individual that occupies 4.5% of the 
    /// wheel and we select 100 individuals, we would expect on average for that individual to be selected 
    /// between four and five times. Stochastic Universal Sampling guarantees this. The individual will be 
    /// selected either four times or five times, not three times, not zero times and not 100 times. 
    /// Standard roulette wheel selection does not make this guarantee.
    /// </para>
    /// <see href="http://en.wikipedia.org/wiki/Stochastic_universal_sampling">Wikipedia</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Stochastic Universal Sampling")]
    public class StochasticUniversalSamplingSelection : SelectionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="GeneticSharp.Domain.Selections.StochasticUniversalSamplingSelection"/> class.
        /// </summary>
        public StochasticUniversalSamplingSelection()
            : base(2)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>
        /// The selected chromosomes.
        /// </returns>
        protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
        {
            var chromosomes = generation.Chromosomes;
            var selected = new List<IChromosome>();
            var sumFitness = chromosomes.Sum(c => c.Fitness.Value);
            var rouleteWheel = new List<double>();
            var accumulativePercent = 0.0;
            double stepSize = 1.0 / number;

            foreach (var c in chromosomes)
            {
                accumulativePercent += c.Fitness.Value / sumFitness;
                rouleteWheel.Add(accumulativePercent);
            }

            var pointer = RandomizationProvider.Current.GetDouble();

            for (int i = 0; i < number; i++)
            {
                if (pointer > 1.0)
                {
                    pointer -= 1.0;
                }

                var chromosomeIndex = rouleteWheel.Select((value, index) => new { Value = value, Index = index }).FirstOrDefault(r => r.Value >= pointer).Index;
                selected.Add(chromosomes[chromosomeIndex]);

                pointer += stepSize;
            }

            return selected;
        }
        #endregion
    }
}
