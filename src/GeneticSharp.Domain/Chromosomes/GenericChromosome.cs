using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Domain.Chromosomes
{
    public abstract class GenericChromosomeBase<TGene> : ChromosomeBase
    {
        public GenericChromosomeBase(int length) : base(length)
        {
        }


        public override Gene GenerateGene(int geneIndex)
        {
            var newGeneContent = GenerateNewGeneValue(geneIndex);
            return new Gene(newGeneContent);
        }

        protected abstract TGene GenerateNewGeneValue(int geneIndex);

        public IEnumerable<TGene> GetTypedGenes()
        {
            return GetGenes().Select(g => (TGene) g.Value);
        }
    }
}