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

        public ChromosomeStub() : base(4)
        {
            CreateGene(0);
            CreateGene(1);
            CreateGene(2);
            CreateGene(3);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(0, 6));
        }

        public override IChromosome CreateNew()
        {
            return new ChromosomeStub();
        }
    }
}
