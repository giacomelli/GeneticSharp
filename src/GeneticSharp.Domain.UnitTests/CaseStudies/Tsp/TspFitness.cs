using System;
using System.Linq;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;
using GeneticSharp.Domain.Randomizations;
using System.Drawing;

namespace GeneticSharp.Domain.UnitTests
{
	public class TspFitness : IFitness
	{
        public TspFitness()
        {
            Cities = new List<TspCity>(6)
            {
                new TspCity(100, 100),
                new TspCity(500, 100),
                new TspCity(1000, 100),
                new TspCity(1000, 500),
                new TspCity(100, 1000),
                new TspCity(1000, 1000)
            };

            MaxX = 1000;
            MaxY = 1000;            
        }

		public TspFitness (int numberOfCities, int maxX, int maxY)
		{
			Cities = new List<TspCity> (numberOfCities);
            MaxX = maxX;
            MaxY = maxY;

			for (int i = 0; i < numberOfCities; i++) {
                var city = new TspCity(RandomizationProvider.Current.GetInt(0, maxX + 1), RandomizationProvider.Current.GetInt(0, maxY + 1));
				Cities.Add (city);

				Console.Write ("({0}, {1}), ", city.Location.X, city.Location.Y);
			}
		}

		#region Properties
		public List<TspCity> Cities { get; private set; }
        public int MaxX { get; private set; }
        public int MaxY { get; private set; }
		#endregion

		#region IFitness implementation

		public double Evaluate (IChromosome chromosome)
		{
			var genes = chromosome.GetGenes ();
			var distanceSum = 0.0;
			var lastCityIndex = Convert.ToInt32 (genes [0].Value);
            var citiesIndexes = new List<int>();
            citiesIndexes.Add(lastCityIndex);

			foreach (var g in genes) {
                var currentCityIndex = Convert.ToInt32 (g.Value);
				distanceSum += CalcDistanceTwoCities(Cities[lastCityIndex], Cities[currentCityIndex]);
                lastCityIndex = currentCityIndex;

                citiesIndexes.Add(lastCityIndex);
			}

            distanceSum += CalcDistanceTwoCities(Cities[citiesIndexes.Last()], Cities[citiesIndexes.First()]);
			
            var fitness = 1.0 - (distanceSum / (Cities.Count * 1000.0));
            
            var numberOfCitiesNotIndex = Cities.Count - citiesIndexes.Distinct().Count();

            if (numberOfCitiesNotIndex > 0)
            {
                fitness /= (numberOfCitiesNotIndex * 10);
            }

            if (fitness < 0)
            {
                fitness = 0;
            }

            return fitness;
		}

		public double CalcDistanceTwoCities(TspCity one, TspCity two)
		{
            return Math.Sqrt(Math.Pow(two.Location.X - one.Location.X, 2) + Math.Pow(two.Location.Y - one.Location.Y, 2));
		}

		public bool SupportsParallel {
			get {
				return false;
			}
		}

		#endregion
	}
}

