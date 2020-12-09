using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// The ScopedBasedMetaHeuristic provides a base class to target specifically some MetaHeuristics operators, while using a fallback Metaheuristic for others
    /// </summary>
    public abstract class ScopedMetaHeuristic : ContainerMetaHeuristic
    {

        public ScopedMetaHeuristic()
        { }

        public ScopedMetaHeuristic(IMetaHeuristic subMetaHeuristic):base(subMetaHeuristic) {}

        /// <summary>
        /// The scope for the current MetaHeuristic behavior. Container fallback is used for other operators
        /// </summary>
        public EvolutionStage Scope { get; set; } = EvolutionStage.All;

        public sealed override  IList<IChromosome> SelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            if ((Scope & EvolutionStage.Selection) == EvolutionStage.Selection)
            {
                return ScopedSelectParentPopulation(ctx, selection);
            }

            return base.SelectParentPopulation(ctx, selection);
        }

        public sealed override IList<IChromosome> MatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            if ((Scope & EvolutionStage.Crossover) == EvolutionStage.Crossover)
            {
                return ScopedMatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
            }

            return base.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
        }

        public sealed override void MutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            if ((Scope & EvolutionStage.Mutation) == EvolutionStage.Mutation)
            {
                ScopedMutateChromosome(ctx, mutation, mutationProbability, offSprings);
            }
            else
            {
                base.MutateChromosome(ctx, mutation, mutationProbability, offSprings);
            }
        }

        public sealed override IList<IChromosome> Reinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            if ((Scope & EvolutionStage.Reinsertion) == EvolutionStage.Reinsertion)
            {
               return ScopedReinsert(ctx, reinsertion, offspring, parents);
            }

            return base.Reinsert(ctx, reinsertion, offspring, parents);

        }

        public abstract IList<IChromosome> ScopedSelectParentPopulation(IEvolutionContext ctx, ISelection selection);

        public abstract IList<IChromosome> ScopedMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover,
            float crossoverProbability, IList<IChromosome> parents);

        public abstract  void ScopedMutateChromosome(IEvolutionContext ctx, IMutation mutation,
            float mutationProbability, IList<IChromosome> offSprings);

        public abstract IList<IChromosome> ScopedReinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents);

    }
}