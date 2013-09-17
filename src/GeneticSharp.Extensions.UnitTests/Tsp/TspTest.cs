using NUnit.Framework;
using System;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Domain.Selections;
using TestSharp;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain;

namespace GeneticSharp.Extensions.UnitTests.Tsp
{
	[TestFixture()]
	public class TspTest
	{
		[Test()]
		public void Evolve_ManyGenerations_Fast ()
		{
			int numberOfCities = 40;
			var selection = new EliteSelection();
			var crossover = new OrderedCrossover();
			var mutation = new TworsMutation();
			var chromosome = new TspChromosome(numberOfCities);
			var fitness = new TspFitness (numberOfCities, 0, 1000, 0, 1000);

			var population = new Population (40, 40, chromosome);

			var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);

			ga.Start();
			var firstDistance = ((TspChromosome)ga.Population.BestChromosome).Distance;

			ga.Termination = new GenerationNumberTermination (1001);

            TimeAssert.LessThan(3000, () =>
            {
                ga.Start();
            });

			var lastDistance = ((TspChromosome)ga.Population.BestChromosome).Distance;

            Assert.Less(lastDistance, firstDistance);
		}
	}
}

