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
            target.Clone();
            target.CompareTo(new FloatingPointChromosome(0, 10, 0));
            target.CreateNew();
            var x = target.Fitness;
            target.FlipGene(0);
            target.GenerateGene(0);
            target.GetGene(0);
            target.GetGenes();
            target.GetHashCode();
            var y = target.Length;
            target.ReplaceGene(0, new Gene(1d));
            target.ReplaceGenes(0, new Gene[] { new Gene(1), new Gene(0) });
            target.Resize(20);
            target.ToFloatingPoint();
            target.ToString();
        }

        [Benchmark]
        public void Integer()
        {
            var target = new IntegerChromosome(0, 10);
            target.Clone();
            target.CompareTo(new IntegerChromosome(0, 10));
            target.CreateNew();
            var x = target.Fitness;
            target.FlipGene(0);
            target.GenerateGene(0);
            target.GetGene(0);
            target.GetGenes();
            target.GetHashCode();
            var y = target.Length;
            target.ReplaceGene(0, new Gene(false));
            target.ReplaceGenes(0, new Gene[] { new Gene(false), new Gene(true) });
            target.Resize(20);
            target.ToInteger();
            target.ToString();
        }
    }
}