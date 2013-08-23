using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Checkers;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Extensions.UnitTests.Checkers
{
    [TestFixture]
    public class CheckersTest
    {
        [Test()]
        public void Evolve_ManyGenerations_Fast()
        {
            int movesAhead = 10;
            int boardSize = 10;
            var selection = new EliteSelection();
            var crossover = new OrderedCrossover();
            var mutation = new TworsMutation();
            var chromosome = new CheckersChromosome(movesAhead, boardSize);
            var fitness = new CheckersFitness(boardSize);

            var population = new Population(40, 40, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.GenerationRan += delegate
            {
                if (ga.Population.GenerationsNumber % 100 == 0)
                {
                    fitness.Update(ga.Population.BestChromosome as CheckersChromosome);
                }
            };

            ga.Start();
            var firstFitness = ((CheckersChromosome)ga.Population.BestChromosome).Fitness;

            ga.Termination = new GenerationNumberTermination(1001);

            TimeAssert.LessThan(3000, () =>
            {
                ga.Start();
            });

            var lastFitness = ((CheckersChromosome)ga.Population.BestChromosome).Fitness;

            Assert.Less(firstFitness, lastFitness);
        }
    }
}