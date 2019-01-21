using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;
using NSubstitute;

namespace GeneticSharp.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Declared)]
    public class TerminationsBenchmark
    {
        private static readonly int _numberOfCities = 10;
        private IPopulation _population = new Population(10, 10, new TspChromosome(_numberOfCities));
        private IList<IChromosome> _parents = new IChromosome[]
        {
            new TspChromosome(_numberOfCities),
            new TspChromosome(_numberOfCities),
            new TspChromosome(_numberOfCities),
            new TspChromosome(_numberOfCities),
            new TspChromosome(_numberOfCities)
        };
        private ITermination _trueTermination = Substitute.For<ITermination>();
        private ITermination _falseTermination = Substitute.For<ITermination>();

        public TerminationsBenchmark()
        {
            _trueTermination.HasReached(null).ReturnsForAnyArgs(true);
            _falseTermination.HasReached(null).ReturnsForAnyArgs(false);
        }

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
            ga.BestChromosome.Returns(new TspChromosome(1));
            target.HasReached(ga);
            target.HasReached(ga);

            return target;
        }

        [Benchmark]
        public ITermination FitnessThreshold()
        {
            var target = new FitnessThresholdTermination(1d);
            var ga = Substitute.For<IGeneticAlgorithm>();
            ga.BestChromosome.Returns(new TspChromosome(1) { Fitness = 1d });
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

        [Benchmark]
        public ITermination TimeEvolving()
        {
            var target = new TimeEvolvingTermination(TimeSpan.FromMilliseconds(1));
            var ga = Substitute.For<IGeneticAlgorithm>();
            target.HasReached(ga);

            return target;
        }
    }
}