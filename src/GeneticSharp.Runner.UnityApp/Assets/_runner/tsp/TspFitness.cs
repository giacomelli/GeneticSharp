using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Randomizations;
using UnityEngine;

namespace GeneticSharp.Extensions.Tsp
{
    public class TspFitness : IFitness
    {
        public TspFitness(int numberOfCities, float minX, float maxX, float minY, float maxY)
        {
            Cities = new List<TspCity>(numberOfCities);
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;

            if (maxX >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("maxX");
            }

            if (maxY >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("maxY");
            }

            for (int i = 0; i < numberOfCities; i++)
            {
                var p = GetCityRandomPosition();
                var city = new TspCity(p.x, p.y);
                Cities.Add(city);
            }
        }

        public IList<TspCity> Cities { get; private set; }
        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float MinY { get; private set; }
        public float MaxY { get; private set; }

        public double Evaluate(IChromosome chromosome)
        {
            var genes = chromosome.GetGenes();
            var distanceSum = 0.0;
            var lastCityIndex = Convert.ToInt32(genes[0].Value, CultureInfo.InvariantCulture);
            var citiesIndexes = new List<int>();
            citiesIndexes.Add(lastCityIndex);

            foreach (var g in genes)
            {
                var currentCityIndex = Convert.ToInt32(g.Value, CultureInfo.InvariantCulture);
                distanceSum += CalcDistanceTwoCities(Cities[currentCityIndex], Cities[lastCityIndex]);
                lastCityIndex = currentCityIndex;

                citiesIndexes.Add(lastCityIndex);
            }

            distanceSum += CalcDistanceTwoCities(Cities[citiesIndexes.Last()], Cities[citiesIndexes.First()]);

            var fitness = 1.0 - (distanceSum / (Cities.Count * 1000.0));

            ((TspChromosome)chromosome).Distance = distanceSum;

            // There is repeated cities on the indexes?
            var diff = Cities.Count - citiesIndexes.Distinct().Count();

            if (diff > 0)
            {
                fitness /= diff;
            }

            if (fitness < 0)
            {
                fitness = 0;
            }

            return fitness;
        }

        private Vector2 GetCityRandomPosition()
        {
            return new Vector2(RandomizationProvider.Current.GetFloat(MinX, MaxX + 1), RandomizationProvider.Current.GetFloat(MinY, MaxY + 1));
        }

        private static double CalcDistanceTwoCities(TspCity one, TspCity two)
        {
            return Math.Sqrt(Math.Pow(two.X - one.X, 2) + Math.Pow(two.Y - one.Y, 2));
        }
    }
}