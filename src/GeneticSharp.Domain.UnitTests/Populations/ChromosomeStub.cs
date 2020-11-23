using System;
using System.Linq.Expressions;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using NSubstitute.Routing.Handlers;

namespace GeneticSharp.Domain.UnitTests
{
    public class ChromosomeStub : ChromosomeBase
    {
        public ChromosomeStub(double fitness)
            : base(2)
        {
            Fitness = fitness;
        }

        public ChromosomeStub() : this(5)
        {
        }

        public ChromosomeStub(int maxValue):this(maxValue,4){}

        public ChromosomeStub(int maxValue, int length) : base(length)
        {
            MaxValue = maxValue;
        }

        public int MaxValue { get; } 

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(0, MaxValue + 1));
        }

        public override IChromosome CreateNew()
        {
            return new ChromosomeStub(MaxValue, Length);
        }

        static int mod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }



        public static Func<double, int> GeneFromDouble(int maxValue) =>d => mod((int) Math.Round(d), maxValue + 1);
        

    }
}
