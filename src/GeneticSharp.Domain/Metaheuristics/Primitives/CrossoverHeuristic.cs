using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Metaheuristic to provide a specific Crossover operator
    /// </summary>
    public class CrossoverHeuristic : OperatorHeuristic<ICrossover>
    {

      
        public override IList<IChromosome> MatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            return base.MatchParentsAndCross(ctx, GetOperator( ctx), crossoverProbability, parents, firstParentIndex);

        }
    }
}