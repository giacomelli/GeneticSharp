using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
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
    [MemoryDiagnoser]
    [RPlotExporter]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class GeneticAlgorithmsBenchmark
    {
        [Params(10, 100)]
        public int NumberOfCities { get; set; }

        [Params(50)]
        public int MinPopulationSize { get; set; }

        [Params(1000)]
        public int Generations { get; set; }

        [Benchmark]
        public GeneticAlgorithm LinearTaskExecutor()
        {
            var ga = CreateGA(); 
            ga.TaskExecutor = new LinearTaskExecutor();

            return ga;
        }

        [Benchmark]
        public GeneticAlgorithm ParallelTaskExecutor()
        {
            var ga = CreateGA();
            ga.TaskExecutor = new ParallelTaskExecutor();

            return ga;
        }

        [Benchmark]
        public GeneticAlgorithm TplTaskExecutor()
        {
            var ga = CreateGA(c =>  new TplPopulation(MinPopulationSize, MinPopulationSize, c));
            ga.OperatorsStrategy = new TplOperatorsStrategy();
            ga.TaskExecutor = new TplTaskExecutor();

            return ga;
        }

        private GeneticAlgorithm CreateGA(Func<TspChromosome, Population> createPopulation = null)
        {
            var selection = new EliteSelection();
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();
            var chromosome = new TspChromosome(NumberOfCities);
            var fitness = new TspFitness(NumberOfCities, 0, 1000, 0, 1000);

            var population = createPopulation == null ? new Population(MinPopulationSize, MinPopulationSize, chromosome) : createPopulation(chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(Generations)
            };

            return ga;
        }
    }
}