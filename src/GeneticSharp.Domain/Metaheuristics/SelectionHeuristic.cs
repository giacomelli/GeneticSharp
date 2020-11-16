using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Metaheuristic to provide a specific selection operator
    /// </summary>
    public class SelectionHeuristic : ContainerMetaHeuristic
    {

        public ISelection Selection { get; set; }

        public SelectionHeuristic() : base() { }

        public SelectionHeuristic(ISelection selection) : base()
        {
            Selection = selection;
        }


        public override IList<IChromosome> SelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection)
        {
            return SubMetaHeuristic.SelectParentPopulation(ctx, Selection);
        }

    }
}