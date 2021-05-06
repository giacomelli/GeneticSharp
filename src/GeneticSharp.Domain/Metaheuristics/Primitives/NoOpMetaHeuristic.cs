using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// This MetaHeuristic doesn't perform any operation and can be used together with other Metaheuristics to Cancel certain operators
    /// </summary>
    [DisplayName("NoOp")]
    public class NoOpMetaHeuristic : MetaHeuristicBase
    {
        public override IList<IChromosome> SelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            return ctx.Population.CurrentGeneration.Chromosomes.Take(ctx.Population.MinSize).ToList();
        }

        public override IList<IChromosome> MatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            //todo: figure out if best to return parents
            if (parents.Count - ctx.LocalIndex >= crossover.ChildrenNumber)
            {
                return parents.Skip(ctx.LocalIndex).Take(crossover.ChildrenNumber).ToList();
            }

            return parents.Skip(ctx.LocalIndex).ToList();
        }

        public override void MutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
        }

        public override IList<IChromosome> Reinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return parents;
        }
    }
}