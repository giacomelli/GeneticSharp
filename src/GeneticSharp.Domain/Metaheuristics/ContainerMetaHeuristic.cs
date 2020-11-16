using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// The ContainerMetaHeuristic is a common base class to hijack certain operations while providing a default fallback for other operations
    /// </summary>
    public class ContainerMetaHeuristic : MetaHeuristicBase
    {

        public ContainerMetaHeuristic(): this( new DefaultMetaHeuristic()){}


        public ContainerMetaHeuristic(IMetaHeuristic subMetaHeuristic)
        {
            SubMetaHeuristic = subMetaHeuristic;
        }

        /// <summary>
        /// This sub metaheuristic is used by for all operators, except for those overriden
        /// </summary>
        public IMetaHeuristic SubMetaHeuristic { get; set; }


        public override IList<IChromosome> SelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection)
        {
            return SubMetaHeuristic.SelectParentPopulation(ctx, selection);
        }

        public override IList<IChromosome> MatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            return SubMetaHeuristic.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents,
                firstParentIndex);
        }


        public override void MutateChromosome(IMetaHeuristicContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex)
        {
            SubMetaHeuristic.MutateChromosome(ctx, mutation, mutationProbability, offSprings, offspringIndex);
        }

        public override IList<IChromosome> Reinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return SubMetaHeuristic.Reinsert(ctx, reinsertion, offspring, parents);
        }


        

    }
}