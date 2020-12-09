using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{

    /// <summary>
    /// Provides a base class with mechanism to compute the current phase and corresponding phase Metaheuristic from population and current individuals
    /// </summary>
    public class IfElseMetaHeuristic:SwitchMetaHeuristic<bool> {}



    /// <summary>
    /// Provides a base class with mechanism to compute the current phase and corresponding phase Metaheuristic from population and current individuals
    /// </summary>
    public class SwitchMetaHeuristic<TIndex> : PhaseMetaHeuristicBase<TIndex>
    {
        public IMetaHeuristicParameterGenerator<TIndex> DynamicParameter { get; set; }



        public override IList<IChromosome> ScopedSelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            var phaseItemIdx = DynamicParameter.GetGenerator(ctx)(this, ctx);
            IMetaHeuristic currentHeuristic = GetCurrentHeuristic(phaseItemIdx);
            if (currentHeuristic != null)
            {
                return currentHeuristic.SelectParentPopulation(ctx, selection);
            }

            throw new ApplicationException($"No phase heuristic for MetaHeuristic {Guid} and phase index {phaseItemIdx}");



        }

        public override IList<IChromosome> ScopedMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
           
            var phaseItemIdx = DynamicParameter.GetGenerator(ctx)(this, ctx);
            IMetaHeuristic currentHeuristic = GetCurrentHeuristic(phaseItemIdx);
            if (currentHeuristic != null)
            {
                return currentHeuristic.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
            }

            throw new ApplicationException($"No phase heuristic for MetaHeuristic {Guid} and phase index {phaseItemIdx}");

        }

        public override void ScopedMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            var phaseItemIdx = DynamicParameter.GetGenerator(ctx)(this, ctx);
            IMetaHeuristic currentHeuristic = GetCurrentHeuristic(phaseItemIdx);
            if (currentHeuristic != null)
            {
                currentHeuristic.MutateChromosome(ctx, mutation, mutationProbability, offSprings);
            }
            else
            {
                throw new ApplicationException($"No phase heuristic for MetaHeuristic {Guid} and phase index {phaseItemIdx}");
            }
           
        }

        public override IList<IChromosome> ScopedReinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            var phaseItemIdx = DynamicParameter.GetGenerator(ctx)(this, ctx);
            IMetaHeuristic currentHeuristic = GetCurrentHeuristic(phaseItemIdx);
            if (currentHeuristic != null)
            {
                return currentHeuristic.Reinsert(ctx, reinsertion, offspring, parents);
            }

            throw new ApplicationException($"No phase heuristic for MetaHeuristic {Guid} and phase index {phaseItemIdx}");
        }


        protected virtual IMetaHeuristic GetCurrentHeuristic(TIndex phaseItemIndex)
        {
            if (PhaseHeuristics.TryGetValue(phaseItemIndex, out var currentHeuristic))
            {
                return currentHeuristic;
            }

            return null;
        }

       
    }
}