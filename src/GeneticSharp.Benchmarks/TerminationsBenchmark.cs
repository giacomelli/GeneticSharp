using System;
using BenchmarkDotNet.Attributes;
using GeneticSharp.Extensions;
using NSubstitute;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class TerminationsBenchmark
    {
        private ITermination _trueTermination = new StubTermination(true);
        private ITermination _falseTermination = new StubTermination(false);

        [Benchmark]
        public ITermination And()
        {
            var target = new AndTermination(_trueTermination, _trueTermination);
            target.HasReached(Substitute.For<IGeneticAlgorithm>());
            return target;
        }

        [Benchmark]
        public ITermination FitnessStagnation()
        {
            var target = new FitnessStagnationTermination(2);
            var ga = Substitute.For<IGeneticAlgorithm>();
            ga.BestChromosome.Returns(new TspChromosome(10) { Fitness = 1 });
            target.HasReached(ga);
            target.HasReached(ga);

            return target;
        }

        [Benchmark]
        public ITermination FitnessThreshold()
        {
            var target = new FitnessThresholdTermination(1d);
            var ga = Substitute.For<IGeneticAlgorithm>();
            ga.BestChromosome.Returns(new TspChromosome(10) { Fitness = 1d });
            target.HasReached(ga);

            return target;
        }

        [Benchmark]
        public ITermination GenerationNumber()
        {
            var target = new GenerationNumberTermination(1);
            var ga = Substitute.For<IGeneticAlgorithm>();
            ga.GenerationsNumber.Returns(1);
            target.HasReached(ga);

            return target;
        }

        [Benchmark]
        public ITermination Or()
        {
            var target = new OrTermination(_falseTermination, _trueTermination);
            var ga = Substitute.For<IGeneticAlgorithm>();
            target.HasReached(ga);

            return target;
        }

        [Benchmark(Baseline = true)]
        public ITermination TimeEvolving()
        {
            var target = new TimeEvolvingTermination(TimeSpan.FromMilliseconds(1));
            var ga = Substitute.For<IGeneticAlgorithm>();
            target.HasReached(ga);

            return target;
        }

        class StubTermination : ITermination
        {
            bool _result;

            public StubTermination(bool result)
            {
                _result = result;
            }

            public bool HasReached(IGeneticAlgorithm geneticAlgorithm) => _result;
        }
    }
}