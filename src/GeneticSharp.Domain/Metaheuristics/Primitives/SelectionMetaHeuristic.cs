using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// Metaheuristic to provide a specific selection operator
    /// </summary>
    [DisplayName("Selection")]
    public class SelectionMetaHeuristic : OperatorMetaHeuristic<ISelection>
    {


        public override IList<IChromosome> SelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {

            return base.SelectParentPopulation(ctx, GetOperator(ctx));
        }

    }
}