using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Extensions.Tsp;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class MutationsBenchmark
    {
        private const int _numberOfCities = 100;
        private const float _probability = 1f;

        [Benchmark]
        public IMutation DisplacementMutation()
        {
            var target = new DisplacementMutation();
            target.Mutate(new TspChromosome(_numberOfCities), _probability);

            return target;
        }

        [Benchmark]
        public IMutation FlipBitMutation()
        {
            var target = new FlipBitMutation();
            target.Mutate(new FloatingPointChromosome(0, _numberOfCities, 0), _probability);

            return target;
        }

        [Benchmark]
        public IMutation InsertionMutation()
        {
            var target = new InsertionMutation();
            target.Mutate(new TspChromosome(_numberOfCities), _probability);

            return target;
        }

        [Benchmark]
        public IMutation PartialShuffleMutation()
        {
            var target = new PartialShuffleMutation();
            target.Mutate(new TspChromosome(_numberOfCities), _probability);

            return target;
        }

        [Benchmark]
        public IMutation ReverseSequenceMutation()
        {
            var target = new ReverseSequenceMutation();
            target.Mutate(new TspChromosome(_numberOfCities), _probability);

            return target;
        }

        [Benchmark]
        public IMutation TworsMutation()
        {
            var target = new TworsMutation();
            target.Mutate(new TspChromosome(_numberOfCities), _probability);

            return target;
        }

        [Benchmark]
        public IMutation UniformMutation()
        {
            var target = new UniformMutation();
            target.Mutate(new TspChromosome(_numberOfCities), _probability);

            return target;
        }
    }
}