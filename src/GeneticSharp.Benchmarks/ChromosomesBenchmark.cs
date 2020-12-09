using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class ChromosomesBenchmark
    {
        [Benchmark]
        public void FloatingPoint()
        {
            var target = new FloatingPointChromosome(0, 10, 0);
            _ = target.Clone();
            _ = target.CompareTo(new FloatingPointChromosome(0, 10, 0));
            _ = target.CreateNew();
            _ = target.Fitness;
            target.FlipGene(0);
            _ = target.GenerateGene(0);
            _ = target.GetGene(0);
            _ = target.GetGenes();
            _ = target.GetHashCode();
            _ = target.Length;
            target.ReplaceGene(0, new Gene(1d));
            target.ReplaceGenes(0, new[] { new Gene(1), new Gene(0) });
            target.Resize(20);
            _ = target.ToFloatingPoint();
            _ = target.ToString();
        }

        [Benchmark]
        public void Integer()
        {
            var target = new IntegerChromosome(0, 10);
            _ = target.Clone();
            _ = target.CompareTo(new IntegerChromosome(0, 10));
            _ = target.CreateNew();
            _ = target.Fitness;
            target.FlipGene(0);
            _ = target.GenerateGene(0);
            _ = target.GetGene(0);
            _ = target.GetGenes();
            _ = target.GetHashCode();
            _ = target.Length;
            target.ReplaceGene(0, new Gene(false));
            target.ReplaceGenes(0, new[] { new Gene(false), new Gene(true) });
            target.Resize(20);
            _ = target.ToInteger();
            _ = target.ToString();
        }
    }
}