using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Metaheuristic to provide a specific selection operator
    /// </summary>
    public class SelectionHeuristic : OperatorHeuristic<ISelection>
    {

        public SelectionHeuristic() {}

        public SelectionHeuristic(ISelection crossover) : base(crossover) { }

        public SelectionHeuristic(ParameterGenerator<ISelection> crossover) : base(crossover) { }


        public override IList<IChromosome> SelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection)
        {

            return base.SelectParentPopulation(ctx, GetOperator(ctx));
        }

    }
}