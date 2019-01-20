using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class ChromosomesBenchmark
    {
        private readonly FloatingPointChromosome _floatingPoint;

        public ChromosomesBenchmark()
        {
            _floatingPoint = new FloatingPointChromosome(0, 10, 0);
        }

        [Benchmark]
        public void ReplaceGene()
        {
            _floatingPoint.ReplaceGene(0, new Gene(1d));
        }
    }
}