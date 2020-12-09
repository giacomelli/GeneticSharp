using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Metaheuristic to provide a specific Crossover operator
    /// </summary>
    public class CrossoverHeuristic : OperatorHeuristic<ICrossover>
    {

      
        public override IList<IChromosome> MatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            return base.MatchParentsAndCross(ctx, GetOperator( ctx), crossoverProbability, parents);

        }
    }
}