using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Metaheuristic to provide a specific mutation operator
    /// </summary>
    public class MutationMetaHeuristic : ContainerMetaHeuristic
    {

        public IMutation Mutation { get; set; }

        public MutationMetaHeuristic() : base() { }

        public MutationMetaHeuristic(IMutation mutation):base()
        {
            Mutation = mutation;
           
        }

        public override void MutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            base.MutateChromosome(ctx, Mutation, mutationProbability, offSprings);
        }
    }
}