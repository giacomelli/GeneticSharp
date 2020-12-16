using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Commons;

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

        

        public static Func<double, int> GeneFromDouble(int maxValue) =>d => Math.Round(d).PositiveMod( maxValue + 1);
    }
}
