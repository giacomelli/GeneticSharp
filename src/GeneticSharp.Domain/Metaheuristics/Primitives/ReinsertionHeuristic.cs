using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Reinsertions;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Metaheuristic to provide a specific Reinsertion operator
    /// </summary>
    public class ReinsertionHeuristic : OperatorHeuristic<IReinsertion>
    {

       
        public override IList<IChromosome> Reinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return base.Reinsert(ctx, GetOperator(ctx), offspring, parents);
        }

       
    }
}