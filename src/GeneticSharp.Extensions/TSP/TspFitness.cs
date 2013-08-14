using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Tsp
{
	public class TspFitness : IFitness
	{    
		#region Constructors
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
		public List<TspCity> Cities { get; private set; }
        public int MinX { get; private set; }
        public int MaxX { get; private set; }
        public int MinY { get; private set; }
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
				distanceSum += CalcDistanceTwoCities(Cities[currentCityIndex], Cities[lastCityIndex]);
                lastCityIndex = currentCityIndex;

                citiesIndexes.Add(lastCityIndex);
			}

            distanceSum += CalcDistanceTwoCities(Cities[citiesIndexes.Last()], Cities[citiesIndexes.First()]);


            var fitness = 1.0 - (distanceSum / (Cities.Count * 1000.0));
		    var numberOfCitiesNotIndex = Cities.Count - citiesIndexes.Distinct().Count();

			((TspChromosome)chromosome).Distance = distanceSum;

            if (numberOfCitiesNotIndex > 0)
            {
				throw new InvalidOperationException ("Less cities");
            }

            if (fitness < 0)
            {
                fitness = 0;
            }

            return fitness;
		}

		public double CalcDistanceTwoCities(TspCity one, TspCity two)
		{
            return Math.Sqrt(Math.Pow(two.X - one.X, 2) + Math.Pow(two.Y - one.Y, 2));
		}

		public bool SupportsParallel {
			get {
				return false;
			}
		}

		#endregion
	}
}

