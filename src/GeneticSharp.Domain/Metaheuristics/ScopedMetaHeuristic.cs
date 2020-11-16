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
    /// The ScopedBasedMetaHeuristic provides a base class to target specifically some MetaHeuristics operators, while using a fallback Metaheuristic for others
    /// </summary>
    public abstract class ScopedMetaHeuristic : ContainerMetaHeuristic
    {

        public ScopedMetaHeuristic():base() { }

        public ScopedMetaHeuristic(IMetaHeuristic subMetaHeuristic):base(subMetaHeuristic) {}

        /// <summary>
        /// The scope for the current MetaHeuristic behavior. Container fallback is used for other operators
        /// </summary>
        public MetaHeuristicsScope Scope { get; set; } = MetaHeuristicsScope.All;

        public sealed override  IList<IChromosome> SelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection)
        {
            if ((Scope & MetaHeuristicsScope.Selection) == MetaHeuristicsScope.Selection)
            {
                return ScopedSelectParentPopulation(ctx, selection);
            }
            else
            {
                return SubMetaHeuristic.SelectParentPopulation(ctx, selection);
            }
        }

        public sealed override IList<IChromosome> MatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            if ((Scope & MetaHeuristicsScope.Crossover) == MetaHeuristicsScope.Crossover)
            {
                return ScopedMatchParentsAndCross(ctx, crossover, crossoverProbability, parents, firstParentIndex);
            }
            else
            {
                return SubMetaHeuristic.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents, firstParentIndex);
            }
        }

        public sealed override void MutateChromosome(IMetaHeuristicContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex)
        {
            if ((Scope & MetaHeuristicsScope.Mutation) == MetaHeuristicsScope.Mutation)
            {
                ScopedMutateChromosome(ctx, mutation, mutationProbability, offSprings, offspringIndex);
            }
            else
            {
                SubMetaHeuristic.MutateChromosome(ctx, mutation, mutationProbability, offSprings, offspringIndex);
            }
        }

        public sealed override IList<IChromosome> Reinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            if ((Scope & MetaHeuristicsScope.Reinsertion) == MetaHeuristicsScope.Reinsertion)
            {
               return ScopedReinsert(ctx, reinsertion, offspring, parents);
            }
            else
            {
                return SubMetaHeuristic.Reinsert(ctx, reinsertion, offspring, parents);
            }

        }

        public abstract IList<IChromosome> ScopedSelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection);

        public abstract IList<IChromosome> ScopedMatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover,
            float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex);

        public abstract  void ScopedMutateChromosome(IMetaHeuristicContext ctx, IMutation mutation,
            float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex);

        public abstract IList<IChromosome> ScopedReinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents);

    }
}