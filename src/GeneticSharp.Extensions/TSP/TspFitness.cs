using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Tsp
{
	/// <summary>
	/// Travelling Salesman Problem fitness function.
	/// <remarks>
	/// The travelling salesman problem (TSP) or travelling salesperson problem asks the following question: 
	/// Given a list of cities and the distances between each pair of cities, what is the shortest possible 
	/// route that visits each city exactly once and returns to the origin city?
	/// <see href="http://en.wikipedia.org/wiki/Travelling_salesman_problem">Wikipedia</see> 
	/// </remarks>
	/// </summary>
	public class TspFitness : IFitness
	{    
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Tsp.TspFitness"/> class.
		/// </summary>
		/// <param name="numberOfCities">The number of cities.</param>
		/// <param name="minX">The minimum city x coordinate.</param>
		/// <param name="maxX">The maximum city x coordinate.</param>
		/// <param name="minY">The minimum city y coordinate.</param>
		/// <param name="maxY">The maximum city y coordinate..</param>
		public TspFitness (int numberOfCities, int minX, int maxX, int minY, int maxY)
		{
			Cities = new List<TspCity> (numberOfCities);
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;            
            
			for (int i = 0; i < numberOfCities; i++) {
                var city = new TspCity(RandomizationProvider.Current.GetInt(MinX, maxX + 1), RandomizationProvider.Current.GetInt(MinY, maxY + 1));
				Cities.Add (city);
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the cities.
		/// </summary>
		/// <value>The cities.</value>
		public List<TspCity> Cities { get; private set; }

		/// <summary>
		/// Gets the minimum x.
		/// </summary>
		/// <value>The minimum x.</value>
        public int MinX { get; private set; }

		/// <summary>
		/// Gets the max x.
		/// </summary>
		/// <value>The max x.</value>
        public int MaxX { get; private set; }

		/// <summary>
		/// Gets the minimum y.
		/// </summary>
		/// <value>The minimum y.</value>
        public int MinY { get; private set; }

		/// <summary>
		/// Gets the max y.
		/// </summary>
		/// <value>The max y.</value>
        public int MaxY { get; private set; }
		#endregion

		#region IFitness implementation
		/// <summary>
		/// Performs the evaluation against the specified chromosome.
		/// </summary>
		/// <param name="chromosome">The chromosome to be evaluated.</param>
		/// <returns>The fitness of the chromosome.</returns>
		public double Evaluate (IChromosome chromosome)
		{
			var genes = chromosome.GetGenes ();
			var distanceSum = 0.0;
			var lastCityIndex = Convert.ToInt32 (genes [0].Value);
            var citiesIndexes = new List<int>();
            citiesIndexes.Add(lastCityIndex);

			foreach (var g in genes) {
                var currentCityIndex = Convert.ToInt32 (g.Value);
				distanceSum += CalcDistanceTwoCities(Cities[currentCityIndex], Cities[lastCityIndex]);
                lastCityIndex = currentCityIndex;

                citiesIndexes.Add(lastCityIndex);
			}

            distanceSum += CalcDistanceTwoCities(Cities[citiesIndexes.Last()], Cities[citiesIndexes.First()]);


            var fitness = 1.0 - (distanceSum / (Cities.Count * 1000.0));
		
			((TspChromosome)chromosome).Distance = distanceSum;

			// There is repeated cities on the indexes?
			var diff =  Cities.Count - citiesIndexes.Distinct ().Count ();

			if (diff > 0) {
				fitness /= diff;
			}

            if (fitness < 0)
            {
                fitness = 0;
            }

            return fitness;
		}

		/// <summary>
		/// Calculates the distance between two cities.
		/// </summary>
		/// <returns>The distance two cities.</returns>
		/// <param name="one">One.</param>
		/// <param name="two">Two.</param>
		private double CalcDistanceTwoCities(TspCity one, TspCity two)
		{
            return Math.Sqrt(Math.Pow(two.X - one.X, 2) + Math.Pow(two.Y - one.Y, 2));
		}
		#endregion
	}
}