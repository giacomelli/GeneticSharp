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

namespace GeneticSharp.Extensions.UnitTests
{
	[TestFixture()]
	public class TspTest
	{
		[Test()]
		public void RunGenerations_ManyGenerations_Fast ()
		{
			int numberOfCities = 40;
			var selection = new EliteSelection();
			var crossover = new OrderedCrossover();
			var mutation = new TworsMutation();
			var chromosome = new TspChromosome(numberOfCities);
			var fitness = new TspFitness (numberOfCities, 0, 1000, 0, 1000);

			var population = new Population(
				40,
				40,
				chromosome,
				fitness,
				selection, crossover, mutation);

            population.RunGeneration();
            var firstDistance = ((TspChromosome)population.BestChromosome).Distance;

			population.Termination = new GenerationNumberTermination (1001);

            TimeAssert.LessThan(3000, () =>
            {
                population.RunGenerations();
            });

            var lastDistance = ((TspChromosome)population.BestChromosome).Distance;

            Assert.Less(lastDistance, firstDistance);
		}
	}
}

