using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Extensions.Tsp;

namespace GeneticSharp.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class CrossoversBenchmark
    {
        private readonly IList<IChromosome> _twoParents;
        private readonly IList<IChromosome> _threeParents;

        public CrossoversBenchmark()
        {
            _twoParents = new TspChromosome[]
            {
                new TspChromosome(100),
                new TspChromosome(100)
            };

            _threeParents = new TspChromosome[]
            {
                new TspChromosome(100),
                new TspChromosome(100),
                new TspChromosome(100),
            };
        }

        [Benchmark]
        public IList<IChromosome> AlternatingPosition()
        {
            var target = new AlternatingPositionCrossover();
            return target.Cross(_twoParents);
        }

        [Benchmark]
        public IList<IChromosome> CutAndSpliceCrossover()
        {
            var target = new CutAndSpliceCrossover();
            return target.Cross(_twoParents);
        }

        [Benchmark]
        public IList<IChromosome> CycleCrossover()
        {
            var target = new CycleCrossover();
            return target.Cross(_twoParents);
        }

        [Benchmark]
        public IList<IChromosome> OnePointCrossover()
        {
            var target = new OnePointCrossover();
            return target.Cross(_twoParents);
        }

        [Benchmark]
        public IList<IChromosome> OrderBasedCrossover()
        {
            var target = new OrderBasedCrossover();
            return target.Cross(_twoParents);
        }

        [Benchmark]
        public IList<IChromosome> OrderedCrossover()
        {
            var target = new OrderedCrossover();
            return target.Cross(_twoParents);
        }

        [Benchmark]
        public IList<IChromosome> PartiallyMappedCrossover()
        {
            var target = new PartiallyMappedCrossover();
            return target.Cross(_twoParents);
        }

        [Benchmark]
        public IList<IChromosome> PositionBasedCrossover()
        {
            var target = new PositionBasedCrossover();
            return target.Cross(_twoParents);
        }

        [Benchmark]
        public IList<IChromosome> ThreeParentCrossover()
        {
            var target = new ThreeParentCrossover();
            return target.Cross(_threeParents);
        }

        [Benchmark]
        public IList<IChromosome> TwoPointCrossover()
        {
            var target = new TwoPointCrossover();
            return target.Cross(_twoParents);
        }

        [Benchmark]
        public IList<IChromosome> UniformCrossover()
        {
            var target = new UniformCrossover();
            return target.Cross(_twoParents);
        }

        [Benchmark]
        public IList<IChromosome> VotingRecombinationCrossover()
        {
            var target = new VotingRecombinationCrossover();
            return target.Cross(_threeParents);
        }
    }
}