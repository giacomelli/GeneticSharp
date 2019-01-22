using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Extensions.Tsp;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class ReinsertionsBenchmark
    {
        private static readonly int _numberOfCities = 10;
        private readonly IList<IChromosome> _parents = new IChromosome[]
        {
            new TspChromosome(_numberOfCities),
            new TspChromosome(_numberOfCities),
            new TspChromosome(_numberOfCities),
            new TspChromosome(_numberOfCities),
            new TspChromosome(_numberOfCities)
        };

        [Benchmark]
        public IReinsertion Elitist()
        {
            var target = new ElitistReinsertion();
            target.SelectChromosomes(new Population(10, 10, new TspChromosome(_numberOfCities)), new List<IChromosome>(), _parents);
            return target;
        }

        [Benchmark]
        public IReinsertion FitnessBased()
        {
            var target = new FitnessBasedReinsertion();
            target.SelectChromosomes(
            new Population(5, 5, new TspChromosome(_numberOfCities)),
               new List<IChromosome>
               {
                    new TspChromosome(_numberOfCities),
                    new TspChromosome(_numberOfCities),
                    new TspChromosome(_numberOfCities),
                    new TspChromosome(_numberOfCities),
                    new TspChromosome(_numberOfCities),
                    new TspChromosome(_numberOfCities)
               },
            _parents);
            return target;
        }

        [Benchmark]
        public IReinsertion Pure()
        {
            var target = new PureReinsertion();
            target.SelectChromosomes(
               new Population(5, 5, new TspChromosome(_numberOfCities)),
               new List<IChromosome>
               {
                    new TspChromosome(_numberOfCities),
                    new TspChromosome(_numberOfCities),
                    new TspChromosome(_numberOfCities),
                    new TspChromosome(_numberOfCities),
                    new TspChromosome(_numberOfCities)  
               },
               _parents);
            return target;
        }

        [Benchmark(Baseline = true)]
        public IReinsertion Uniform()
        {
            var target = new UniformReinsertion();
            target.SelectChromosomes(
              new Population(5, 5, new TspChromosome(_numberOfCities)),
              new List<IChromosome>
              {
                    new TspChromosome(_numberOfCities)
              },
              _parents);
            return target;
        }
    }
}