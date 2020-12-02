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
        public MetaHeuristicsStage Scope { get; set; } = MetaHeuristicsStage.All;

        public sealed override  IList<IChromosome> SelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection)
        {
            if ((Scope & MetaHeuristicsStage.Selection) == MetaHeuristicsStage.Selection)
            {
                return ScopedSelectParentPopulation(ctx, selection);
            }
            else
            {
                return base.SelectParentPopulation(ctx, selection);
            }
        }

        public sealed override IList<IChromosome> MatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            if ((Scope & MetaHeuristicsStage.Crossover) == MetaHeuristicsStage.Crossover)
            {
                return ScopedMatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
            }
            else
            {
                return base.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
            }
        }

        public sealed override void MutateChromosome(IMetaHeuristicContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            if ((Scope & MetaHeuristicsStage.Mutation) == MetaHeuristicsStage.Mutation)
            {
                ScopedMutateChromosome(ctx, mutation, mutationProbability, offSprings);
            }
            else
            {
                base.MutateChromosome(ctx, mutation, mutationProbability, offSprings);
            }
        }

        public sealed override IList<IChromosome> Reinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            if ((Scope & MetaHeuristicsStage.Reinsertion) == MetaHeuristicsStage.Reinsertion)
            {
               return ScopedReinsert(ctx, reinsertion, offspring, parents);
            }
            else
            {
                return base.Reinsert(ctx, reinsertion, offspring, parents);
            }

        }

        public abstract IList<IChromosome> ScopedSelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection);

        public abstract IList<IChromosome> ScopedMatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover,
            float crossoverProbability, IList<IChromosome> parents);

        public abstract  void ScopedMutateChromosome(IMetaHeuristicContext ctx, IMutation mutation,
            float mutationProbability, IList<IChromosome> offSprings);

        public abstract IList<IChromosome> ScopedReinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents);

    }
}