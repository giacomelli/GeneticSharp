using System;

namespace GeneticSharp.Domain.Chromosomes
{
    public class DynamicChromosome<TGene> : GenericChromosomeBase<TGene>
    {


        private Func<int, TGene> GeneGenerator { get; set; }


        public DynamicChromosome(int length) : base(length)
        {
        }

        public override IChromosome CreateNew()
        {
            return new DynamicChromosome<TGene>(this.Length){GeneGenerator = GeneGenerator};
        }


        protected override TGene GenerateNewGeneValue(int geneIndex)
        {
            return GeneGenerator(geneIndex);
        }
    }
}