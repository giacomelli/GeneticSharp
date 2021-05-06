using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// Metaheuristic to provide a specific mutation operator
    /// </summary>
    [DisplayName("Mutation")]
    public class MutationHeuristic : OperatorHeuristic<IMutation>
    {

        protected override void DoMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            base.DoMutateChromosome(ctx, GetOperator(ctx), mutationProbability, offSprings);
        }


    }
}