using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace Issue1Sample
{
    class Issue1Chromosome : ChromosomeBase
    {
        public Issue1Chromosome()
            : base(2)
        {
            ReplaceGene(0, GenerateGene(0));
            ReplaceGene(1, GenerateGene(1));
        }

        // These properties represents your phenotype.
        public int X { get; private set; }
        public int Y { get; private set; }

        public override Gene GenerateGene(int geneIndex)
        {
            if (geneIndex == 0)
            {
                X = RandomizationProvider.Current.GetInt(0, 8);
                return new Gene(X);
            }
            else
            {
                Y = RandomizationProvider.Current.GetInt(0, 101);
                return new Gene(Y);
            }
        }

        public override IChromosome CreateNew()
        {
            return new Issue1Chromosome();
        }

        public override IChromosome Clone()
        {
            var clone = base.Clone() as Issue1Chromosome;
            clone.X = X;
            clone.Y = Y;

            return clone;
        }
    }
}
