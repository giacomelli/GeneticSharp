using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Reinsertions;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// Metaheuristic to provide a specific Reinsertion operator
    /// </summary>
    [DisplayName("Reinsertion")]
    public class ReinsertionHeuristic : OperatorHeuristic<IReinsertion>
    {

       
        public override IList<IChromosome> Reinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return base.Reinsert(ctx, GetOperator(ctx), offspring, parents);
        }

       
    }
}