using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using HelperSharp;
using System.Linq;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Selections
{
	/// <summary>
	/// In the Roulette wheel selection method [Holland, 1992], the first step is to calculate the cumulative fitness of the 
	/// whole population through the sum of the fitness of all individuals. After that, the probability of selection is 
	/// calculated for each individual.
	/// Then, an array is built containing cumulative probabilities of the individuals. So, n random numbers are generated in the range 0 to fitness sum.
	/// and for each random number an array element which can have higher value is searched for. Therefore, individuals are selected according to their 
	/// probabilities of selection. 
	/// </summary>
	public class RouletteWheelSelection : SelectionBase
	{
		#region Constructors
		public RouletteWheelSelection() : base(2)
		{
		}
		#endregion

		#region ISelection implementation
		protected override IList<IChromosome> PerformSelectChromosomes (int number, Generation generation)
		{
			var chromosomes = generation.Chromosomes;
			var selected = new List<IChromosome> ();
			var sumFitness = chromosomes.Sum(c => c.Fitness.Value);		
            var rouleteWheel = new List<double>();
            var accumulativePercent = 0.0;

            foreach (var c in chromosomes)
            {
                accumulativePercent += c.Fitness.Value / sumFitness;
                rouleteWheel.Add(accumulativePercent);
            }

			for(int i = 0; i < number; i++)
			{
				var pointer = RandomizationProvider.Current.GetDouble ();
				var chromosomeIndex = rouleteWheel.Select ((value, index) => new { Value = value, Index = index }).FirstOrDefault (r => r.Value >= pointer).Index; 
				selected.Add (chromosomes[chromosomeIndex]);
			}

			return selected;
		}
		#endregion
	}
}