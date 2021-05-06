using System;
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

    public abstract class GenericFitness<TGene> : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {
            var typedChromosome = chromosome as GenericChromosomeBase<TGene>;
            var typedGenes = typedChromosome.GetTypedGenes();
            return EvaluateGenes(typedGenes);
        }

        protected abstract double EvaluateGenes(IEnumerable<TGene> genes);

    }
}