using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Fitnesses
{
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