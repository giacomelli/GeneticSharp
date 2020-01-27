using System;
using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Framework.Threading;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class GeneticAlgorithmsBenchmark
    {
        private const int _minPopulationSize = 50;
        private const int _generations = 1000;
        private const int _numberOfCities = 100;

        [Benchmark]
        public GeneticAlgorithm LinearTaskExecutor()
        {
            var ga = CreateGA(); 
            ga.TaskExecutor = new LinearTaskExecutor();
            ga.Start();

            return ga;
        }

        [Benchmark]
        public GeneticAlgorithm ParallelTaskExecutor()
        {
            var ga = CreateGA();
            ga.TaskExecutor = new ParallelTaskExecutor();
            ga.Start();

            return ga;
        }

        [Benchmark]
        public GeneticAlgorithm TplTaskExecutor()
        {
            var ga = CreateGA(c => new TplPopulation(_minPopulationSize, _minPopulationSize, c));
            ga.OperatorsStrategy = new TplOperatorsStrategy();
            ga.TaskExecutor = new TplTaskExecutor();
            ga.Start();

            return ga;
        }

        private GeneticAlgorithm CreateGA(Func<TspChromosome, Population> createPopulation = null)
        {
            var selection = new EliteSelection();
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();
            var chromosome = new TspChromosome(_numberOfCities);
            var fitness = new TspFitness(_numberOfCities, 0, 1000, 0, 1000);

            var population = createPopulation == null 
            ? new Population(_minPopulationSize, _minPopulationSize, chromosome)
            : createPopulation(chromosome);

            population.GenerationStrategy = new PerformanceGenerationStrategy();

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(_generations)
            };

            return ga;
        }
    }
}