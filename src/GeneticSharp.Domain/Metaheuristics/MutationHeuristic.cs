using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Metaheuristic to provide a specific mutation operator
    /// </summary>
    public class MutationHeuristic : OperatorHeuristic<IMutation>
    {

        public override void MutateChromosome(IMetaHeuristicContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex)
        {
            base.MutateChromosome(ctx, GetOperator(ctx), mutationProbability, offSprings, offspringIndex);
        }


    }
}