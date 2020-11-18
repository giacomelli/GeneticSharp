using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.UnitTests
{
    public class OrderedChromosomeStub : ChromosomeBase
    {

        public OrderedChromosomeStub(int length)
            : base(length){}

        public OrderedChromosomeStub() : this(6) {}

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

        protected override void CreateGenes()
        {
            var values = RandomizationProvider.Current.GetUniqueInts(Length, 0, Length);
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, new Gene(values[i]));
            }
        }
    }
}
