using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Metaheuristic to provide a specific selection operator
    /// </summary>
    public class SelectionHeuristic : OperatorHeuristic<ISelection>
    {


        public override IList<IChromosome> SelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {

            return base.SelectParentPopulation(ctx, GetOperator(ctx));
        }

    }
}