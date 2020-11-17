using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

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
            for (int i = 0; i < Length; i++)
            {
                CreateGene(i);
            }
            
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
    }
}
