using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Extensions.Tsp;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class CrossoversBenchmark
    {
        private const int _numberOfCities = 100;

        [Benchmark]
        public IList<IChromosome> AlternatingPosition()
        {
            var target = new AlternatingPositionCrossover();
            return target.Cross(CreateTwoParents());
        }

        [Benchmark]
        public IList<IChromosome> CutAndSpliceCrossover()
        {
            var target = new CutAndSpliceCrossover();
            return target.Cross(CreateTwoParents());
        }

        [Benchmark]
        public IList<IChromosome> CycleCrossover()
        {
            var target = new CycleCrossover();
            return target.Cross(CreateTwoParents());
        }

        [Benchmark]
        public IList<IChromosome> OnePointCrossover()
        {
            var target = new OnePointCrossover();
            return target.Cross(CreateTwoParents());
        }

        [Benchmark]
        public IList<IChromosome> OrderBasedCrossover()
        {
            var target = new OrderBasedCrossover();
            return target.Cross(CreateTwoParents());
        }

        [Benchmark]
        public IList<IChromosome> OrderedCrossover()
        {
            var target = new OrderedCrossover();
            return target.Cross(CreateTwoParents());
        }

        [Benchmark]
        public IList<IChromosome> PartiallyMappedCrossover()
        {
            var target = new PartiallyMappedCrossover();
            return target.Cross(CreateTwoParents());
        }

        [Benchmark]
        public IList<IChromosome> PositionBasedCrossover()
        {
            var target = new PositionBasedCrossover();
            return target.Cross(CreateTwoParents());
        }

        [Benchmark]
        public IList<IChromosome> ThreeParentCrossover()
        {
            var target = new ThreeParentCrossover();
            return target.Cross(CreateThreeParents());
        }

        [Benchmark]
        public IList<IChromosome> TwoPointCrossover()
        {
            var target = new TwoPointCrossover();
            return target.Cross(CreateTwoParents());
        }

        [Benchmark]
        public IList<IChromosome> UniformCrossover()
        {
            var target = new UniformCrossover();
            return target.Cross(CreateTwoParents());
        }

        [Benchmark]
        public IList<IChromosome> VotingRecombinationCrossover()
        {
            var target = new VotingRecombinationCrossover();
            return target.Cross(CreateThreeParents());
        }

        private IList<IChromosome> CreateTwoParents()
        {
            return new TspChromosome[]
            {
                new TspChromosome(_numberOfCities),
                new TspChromosome(_numberOfCities)
            };
        }

        private IList<IChromosome> CreateThreeParents()
        {
            return new TspChromosome[]
            {
                new TspChromosome(_numberOfCities),
                new TspChromosome(_numberOfCities),
                new TspChromosome(_numberOfCities)
            };
        }
    }
}