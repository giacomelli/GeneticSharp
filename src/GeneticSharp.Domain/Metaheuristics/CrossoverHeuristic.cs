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
    public class CrossoverHeuristic : ContainerMetaHeuristic
    {

        public Func<IMetaHeuristicContext, ICrossover> DynamicCrossover { get; set; }

        public  ICrossover Crossover { get; set; }

        public CrossoverHeuristic() : base() { }

        public CrossoverHeuristic(ICrossover crossover) : base()
        {
            Crossover = crossover;
        }


        public override IList<IChromosome> MatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            if (Crossover!=null)
            {
                return SubMetaHeuristic.MatchParentsAndCross(ctx, Crossover, crossoverProbability, parents,
                    firstParentIndex);
            }
            return SubMetaHeuristic.MatchParentsAndCross(ctx, DynamicCrossover(ctx), crossoverProbability, parents,
                firstParentIndex);

        }
    }
}