using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Mathematic;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Mathematic
{
    [TestFixture()]
    [Category("Extensions")]
    public class FunctionBuilderTest
    {
        [Test()]
        public void Evolve_ManyGenerations_Fast()
        {
            var selection = new EliteSelection();
            var crossover = new ThreeParentCrossover();
            var mutation = new UniformMutation(true);

            var fitness = new FunctionBuilderFitness(
                new FunctionBuilderInput(
                    new double[] { 1, 2 },
                    3)
                ,
                new FunctionBuilderInput(
                    new double[] { 2, 3 },
                    5)
            );
            var chromosome = new FunctionBuilderChromosome(fitness.AvailableOperations, 3);

            var population = new Population(100, 200, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = new OrTermination(new FitnessThresholdTermination(0), new TimeEvolvingTermination(TimeSpan.FromSeconds(15)));
            ga.Start();
            var bestChromosome = ga.BestChromosome as FunctionBuilderChromosome;
            Assert.AreEqual(0.0, bestChromosome.Fitness.Value);
            var actual = fitness.GetFunctionResult(
                             bestChromosome.BuildFunction(),
                             new FunctionBuilderInput(new double[] { 3, 4 }, 7)
                );

            Assert.AreEqual(7, actual);
        }
    }
}