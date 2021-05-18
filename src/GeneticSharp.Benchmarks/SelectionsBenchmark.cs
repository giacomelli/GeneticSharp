using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Extensions.Tsp;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class SelectionsBenchmark
    {
        private readonly Generation _generation = new Generation(1, new List<IChromosome>
        {
            new TspChromosome(10) { Fitness = 1 },
            new TspChromosome(10) { Fitness = 2 },
            new TspChromosome(10) { Fitness = 3 },
            new TspChromosome(10) { Fitness = 4 },
            new TspChromosome(10) { Fitness = 5 },
            new TspChromosome(10) { Fitness = 6 },
            new TspChromosome(10) { Fitness = 7 },
            new TspChromosome(10) { Fitness = 8 },
            new TspChromosome(10) { Fitness = 9 },
            new TspChromosome(10) { Fitness = 10 },
        });

        private const int ChromosomesNumber = 4;

        [Benchmark(Baseline = true)]
        public ISelection Elite()
        {
            var target = new EliteSelection();
            target.SelectChromosomes(ChromosomesNumber, _generation);
            return target;
        }

        [Benchmark]
        public ISelection RouletteWheel()
        {
            var target = new RouletteWheelSelection();
            target.SelectChromosomes(ChromosomesNumber, _generation);
            return target;
        }

        [Benchmark]
        public ISelection StochasticUniversalSampling()
        {
            var target = new StochasticUniversalSamplingSelection();
            target.SelectChromosomes(ChromosomesNumber, _generation);
            return target;
        }

        [Benchmark]
        public ISelection Tournament()
        {
            var target = new TournamentSelection();
            target.SelectChromosomes(ChromosomesNumber, _generation);
            return target;
        }
    }
}