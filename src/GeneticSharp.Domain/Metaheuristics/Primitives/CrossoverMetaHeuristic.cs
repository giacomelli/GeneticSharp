using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// Metaheuristic to provide a specific Crossover operator
    /// </summary>
    [DisplayName("Container")]
    public class CrossoverMetaHeuristic : OperatorMetaHeuristic<ICrossover>
    {


        protected override IList<IChromosome> DoMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            return base.DoMatchParentsAndCross(ctx, GetOperator( ctx), crossoverProbability, parents);

        }
    }
}