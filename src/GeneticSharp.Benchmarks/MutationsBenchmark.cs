using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Extensions.Tsp;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class MutationsBenchmark
    {
        [Params(10, 100)]
        public int NumberOfCities { get; set; }

        [Params(1f)]
        public float Probability { get; set; }

        [Benchmark]
        public IMutation DisplacementMutation()
        {
            var target = new DisplacementMutation();
            target.Mutate(new TspChromosome(NumberOfCities), Probability);

            return target;
        }

        [Benchmark]
        public IMutation FlipBitMutation()
        {
            var target = new FlipBitMutation();
            target.Mutate(new FloatingPointChromosome(0, NumberOfCities, 0), Probability);

            return target;
        }

        [Benchmark]
        public IMutation InsertionMutation()
        {
            var target = new InsertionMutation();
            target.Mutate(new TspChromosome(NumberOfCities), Probability);

            return target;
        }

        [Benchmark]
        public IMutation PartialShuffleMutation()
        {
            var target = new PartialShuffleMutation();
            target.Mutate(new TspChromosome(NumberOfCities), Probability);

            return target;
        }

        [Benchmark]
        public IMutation ReverseSequenceMutation()
        {
            var target = new ReverseSequenceMutation();
            target.Mutate(new TspChromosome(NumberOfCities), Probability);

            return target;
        }

        [Benchmark]
        public IMutation TworsMutation()
        {
            var target = new TworsMutation();
            target.Mutate(new TspChromosome(NumberOfCities), Probability);

            return target;
        }

        [Benchmark]
        public IMutation UniformMutation()
        {
            var target = new UniformMutation();
            target.Mutate(new TspChromosome(NumberOfCities), Probability);

            return target;
        }
    }
}