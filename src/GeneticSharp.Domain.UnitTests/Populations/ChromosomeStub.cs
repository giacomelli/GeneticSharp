using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.UnitTests
{
    public class ChromosomeStub : ChromosomeBase
    {
        private int _maxValue = 5;

        public ChromosomeStub(double fitness)
            : base(2)
        {
            Fitness = fitness;
        }

        public ChromosomeStub() : this(5)
        {
        }

        public ChromosomeStub(int maxValue) : base(4)
        {
            _maxValue = maxValue;
            CreateGene(0);
            CreateGene(1);
            CreateGene(2);
            CreateGene(3);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(0, _maxValue + 1));
        }

        public override IChromosome CreateNew()
        {
            return new ChromosomeStub();
        }
    }
}
