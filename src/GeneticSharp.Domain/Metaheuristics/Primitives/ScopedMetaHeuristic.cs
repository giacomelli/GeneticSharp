using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// The ScopedBasedMetaHeuristic provides a base class to target specifically some MetaHeuristics operators, while using a fallback Metaheuristic for others
    /// </summary>
    public abstract class ScopedMetaHeuristic : ContainerMetaHeuristic
    {
        protected ScopedMetaHeuristic()
        { }

        protected ScopedMetaHeuristic(IMetaHeuristic subMetaHeuristic):base(subMetaHeuristic) {}

        /// <summary>
        /// The scope for the current MetaHeuristic behavior. Container fallback is used for other operators
        /// </summary>
        public EvolutionStage Scope { get; set; } = EvolutionStage.All;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override  IList<IChromosome> SelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            if ((Scope & EvolutionStage.Selection) == EvolutionStage.Selection)
            {
                return ScopedSelectParentPopulation(ctx, selection);
            }

            return base.SelectParentPopulation(ctx, selection);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override IList<IChromosome> DoMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            if ((Scope & EvolutionStage.Crossover) == EvolutionStage.Crossover)
            {
                return ScopedMatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
            }

            return base.DoMatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void DoMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            if ((Scope & EvolutionStage.Mutation) == EvolutionStage.Mutation)
            {
                ScopedMutateChromosome(ctx, mutation, mutationProbability, offSprings);
            }
            else
            {
                base.DoMutateChromosome(ctx, mutation, mutationProbability, offSprings);
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override IList<IChromosome> Reinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            if ((Scope & EvolutionStage.Reinsertion) == EvolutionStage.Reinsertion)
            {
               return ScopedReinsert(ctx, reinsertion, offspring, parents);
            }

            return base.Reinsert(ctx, reinsertion, offspring, parents);

        }

        protected abstract IList<IChromosome> ScopedSelectParentPopulation(IEvolutionContext ctx, ISelection selection);

        protected abstract IList<IChromosome> ScopedMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover,
            float crossoverProbability, IList<IChromosome> parents);

        protected abstract  void ScopedMutateChromosome(IEvolutionContext ctx, IMutation mutation,
            float mutationProbability, IList<IChromosome> offSprings);

        protected abstract IList<IChromosome> ScopedReinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents);

    }
}