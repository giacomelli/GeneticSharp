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
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class GeneticAlgorithmsBenchmark
    {
        [Benchmark]
        public void Start_Tsp_LinearTaskExecutor()
        {
            StartGA(new LinearTaskExecutor());
        }

        [Benchmark]
        public void Start_Tsp_ParallelTaskExecutor()
        {
            StartGA(new ParallelTaskExecutor());
        }

        [Benchmark]
        public void Start_Tsp_TplTaskExecutor()
        {
            StartGA(new TplTaskExecutor());
        }

        private void StartGA(ITaskExecutor taskExecutor)
        {
            int numberOfCities = 100;
            var selection = new EliteSelection();
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();
            var chromosome = new TspChromosome(numberOfCities);
            var fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);

            var population = new Population(50, 50, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(1000),
                TaskExecutor = taskExecutor
            };

            ga.Start();
        }
    }
}