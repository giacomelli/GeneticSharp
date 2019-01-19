using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Extensions.Tsp;

namespace GeneticSharp.Benchmarks
{
    [MemoryDiagnoser]
    public class CrossoversBenchmark
    {
        private readonly IList<IChromosome> _parents;

        public CrossoversBenchmark()
        {
            _parents = new TspChromosome[]
            {
                new TspChromosome(100),
                new TspChromosome(100),
            };
        }

        [Benchmark]
        public void AlternatingPosition()
        {
            var target = new AlternatingPositionCrossover();
            target.Cross(CreateParents());
        }

        [Benchmark]
        public void CutAndSpliceCrossover()
        {
            var target = new CutAndSpliceCrossover();
            target.Cross(CreateParents());
        }

        [Benchmark]
        public void CycleCrossover()
        {
            var target = new CycleCrossover();
            target.Cross(CreateParents());
        }

        [Benchmark]
        public void OnePointCrossover()
        {
            var target = new OnePointCrossover();
            target.Cross(CreateParents());
        }

        private static IList<IChromosome> CreateParents()
        {
            return new TspChromosome[]
            {
                new TspChromosome(100),
                new TspChromosome(100),
            };
        }
    }
}
