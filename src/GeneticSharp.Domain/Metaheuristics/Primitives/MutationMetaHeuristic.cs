using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// Metaheuristic to provide a specific mutation operator
    /// </summary>
    public class MutationMetaHeuristic : ContainerMetaHeuristic
    {

        public IMutation Mutation { get; set; }

        public MutationMetaHeuristic()
        { }

        public MutationMetaHeuristic(IMutation mutation)
        {
            Mutation = mutation;
           
        }

        public override void MutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            base.MutateChromosome(ctx, Mutation, mutationProbability, offSprings);
        }
    }
}