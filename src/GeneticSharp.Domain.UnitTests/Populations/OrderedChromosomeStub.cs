using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.UnitTests
{
    public class OrderedChromosomeStub : ChromosomeBase
    {
        public OrderedChromosomeStub()
            : base(6)
        {
            var values = RandomizationProvider.Current.GetUniqueInts(6, 0, 6);
            ReplaceGene(0, new Gene(values[0]));
            ReplaceGene(1, new Gene(values[1]));
            ReplaceGene(2, new Gene(values[2]));
            ReplaceGene(3, new Gene(values[3]));
            ReplaceGene(4, new Gene(values[4]));
            ReplaceGene(5, new Gene(values[5]));
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(0, 6));
        }

        public override IChromosome CreateNew()
        {
            return new OrderedChromosomeStub();
        }

        public override IChromosome Clone()
        {
            var clone = base.Clone() as OrderedChromosomeStub;
            return clone;
        }
    }
}
