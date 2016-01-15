using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.AutoConfig;
using GeneticSharp.Extensions.Checkers;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Threading;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.AutoConfig
{
    [TestFixture]
    [Category("Extensions")]
    class AutoConfigTest
    {
        [SetUp]
        public void InitializeTest()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void Evolve_ManyGenerations_Fast()
        {
            var selection = new EliteSelection();
            var crossover = new UniformCrossover();
            var mutation = new UniformMutation(true);
            var chromosome = new AutoConfigChromosome();
            var targetChromosome = new TspChromosome(10);
            var targetFitness = new TspFitness(10, 0, 100, 0, 100);            
            var fitness = new AutoConfigFitness(targetFitness, targetChromosome);
            fitness.PopulationMinSize = 20;
            fitness.PopulationMaxSize = 20;
            fitness.Termination = new TimeEvolvingTermination(TimeSpan.FromSeconds(5));
            
            var population = new Population(10, 10, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            
            ga.TaskExecutor = new SmartThreadPoolTaskExecutor()
            {
                MinThreads = 10,
                MaxThreads = 20
            };        

            ga.Termination = new GenerationNumberTermination(2);
            ga.Start();

            Assert.NotNull(ga.BestChromosome);            
        }

    }
}
