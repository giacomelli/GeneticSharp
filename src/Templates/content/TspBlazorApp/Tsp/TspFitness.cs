using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Randomizations;

public class TspFitness : IFitness
{
    int _areaWidth;
    int _areaHeight;

    public TspFitness(int numberOfCities, int areaWidth, int areaHeight)
    {
        Cities = new List<TspCity>(numberOfCities);

        _areaWidth = areaWidth;
        _areaHeight = areaHeight;

        for (int i = 0; i < numberOfCities; i++)
        {
            Cities.Add(GetRandomCity());
        }
    }

    public IList<TspCity> Cities { get; private set; }
   
    public double Evaluate(IChromosome chromosome)
    {
        var genes = chromosome.GetGenes();
        var distanceSum = 0.0;
        var lastCityIndex = Convert.ToInt32(genes[0].Value, CultureInfo.InvariantCulture);
        var citiesIndexes = new List<int>();
        citiesIndexes.Add(lastCityIndex);

        // Calculates the total route distance.
        foreach (var g in genes)
        {
            var currentCityIndex = Convert.ToInt32(g.Value, CultureInfo.InvariantCulture);
            distanceSum += Cities[currentCityIndex].Distance(Cities[lastCityIndex]);
            lastCityIndex = currentCityIndex;

            citiesIndexes.Add(lastCityIndex);
        }

        distanceSum += Cities[citiesIndexes.Last()].Distance(Cities[citiesIndexes.First()]);

        var fitness = 1.0 - (distanceSum / (Cities.Count * 1000.0));

        ((TspChromosome)chromosome).Distance = distanceSum;

        // Are there repeated cities on the indexes?
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

    private TspCity GetRandomCity()
    {
        return new TspCity(
            RandomizationProvider.Current.GetFloat(0, _areaWidth), 
            RandomizationProvider.Current.GetFloat(0, _areaHeight));
    }
}