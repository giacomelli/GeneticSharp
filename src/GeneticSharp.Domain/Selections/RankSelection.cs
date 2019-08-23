using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Selections
{
    /// <summary>
    /// Rank Selection
    /// <remarks>
    /// Is a kind of Fitness Proportionate Selection. 
    /// <see href=" https://www.obitko.com/tutorials/genetic-algorithms/selection.php">Rank Selection</see>
    /// <para>
    /// The Rank selection method, is similar to the <see cref="RouletteWheelSelection"/>. However the size of the wheel and sectors are
    /// calculated differently.
    /// </para>
    /// <para>
    /// In the Rank selection method, the first step is to sort the population by fitness and assign a new fitness value from 1 to n.
    /// The worst chromosome is given a fitness of 1, the best a fitness of n.
    /// </para>
    /// <para>
    /// Then, an array is built containing cumulative probabilities of the individuals. So, n random numbers are generated in the range 0 to fitness sum.
    /// and for each random number an array element which can have higher value is searched for. Therefore, individuals are selected according to their 
    /// probabilities of selection. 
    /// </para>
    /// </remarks>
    /// </summary>
    [DisplayName("Rank")]
    public class RankSelection : SelectionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Selections.RankSelection"/> class.
        /// </summary>
        public RankSelection() : base(2)
        {
        }
        #endregion

        #region ISelection implementation
        /// <summary>
        /// Selects from wheel.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="chromosomes">The chromosomes.</param>
        /// <param name="rankWheel">The rank wheel.</param>
        /// <param name="getPointer">The get pointer.</param>
        /// <returns>The selected chromosomes.</returns>
        protected static IList<IChromosome> SelectFromWheel(int number, IList<IChromosome> chromosomes, IList<double> rankWheel, Func<double> getPointer)
        {
            var selected = new List<IChromosome>();

            for (int i = 0; i < number; i++)
            {
                var pointer = getPointer();

                var chromosome = rankWheel
                                        .Select((value, index) => new { Value = value, Index = index })
                                        .FirstOrDefault(r => r.Value >= pointer);

                if (chromosome != null)
                    selected.Add(chromosomes[chromosome.Index]);
            }

            return selected;
        }

        /// <summary>
        /// Calculates the cumulative percent.
        /// </summary>
        /// <param name="chromosomes">The chromosomes.</param>
        /// <param name="rankWheel">The rank wheel.</param>
        protected static void CalculateCumulativeFitnessRank(IList<IChromosome> chromosomes, IList<double> rankWheel)
        {
            var totalFitness = chromosomes.Count * (chromosomes.Count + 1) / 2;

            var cumulativeRank = 0.0;

            for (int n = chromosomes.Count; n > 0; n--)
            {
                cumulativeRank += (double)n / totalFitness;
                rankWheel.Add(cumulativeRank);
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
            var chromosomes = generation.Chromosomes.OrderByDescending(c => c.Fitness).ToList();
            var rankWheel = new List<double>();
            var rnd = RandomizationProvider.Current;

            CalculateCumulativeFitnessRank(chromosomes, rankWheel);

            return SelectFromWheel(number, chromosomes, rankWheel, () => rnd.GetDouble());
        }
        #endregion
    }
}
