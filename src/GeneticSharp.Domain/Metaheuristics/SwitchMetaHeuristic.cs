using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Infrastructure.Framework.Collections;

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
        
        public SwitchMetaHeuristic() : base(){}


        protected ParameterGenerator<TIndex> GetIndexGenerator(IMetaHeuristicContext ctx)
        {
            return IndexGenerator ?? (IndexGenerator =
                ParameterReplacer.ReduceLambda<TIndex>(IndexGeneratorWithArgs, ctx).Compile());
        }


        public ParameterGenerator<TIndex> IndexGenerator { get; set; }

        public LambdaExpression IndexGeneratorWithArgs { get; set; }

        public override IList<IChromosome> ScopedSelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection)
        {
            var phaseItemIdx = GetIndexGenerator(ctx)(this, ctx);
            IMetaHeuristic currentHeuristic = GetCurrentHeuristic(phaseItemIdx);
            if (currentHeuristic != null)
            {
                return currentHeuristic.SelectParentPopulation(ctx, selection);
            }
            else
            {
                throw new ApplicationException($"No phase heuristic for MetaHeuristic {Guid} and phase index {phaseItemIdx}");
            }

            
            
        }

        public override IList<IChromosome> ScopedMatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
           
            var phaseItemIdx = GetIndexGenerator(ctx)(this, ctx);
            IMetaHeuristic currentHeuristic = GetCurrentHeuristic(phaseItemIdx);
            if (currentHeuristic != null)
            {
                return currentHeuristic.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents,
                    firstParentIndex);
            }
            else
            {
                throw new ApplicationException($"No phase heuristic for MetaHeuristic {Guid} and phase index {phaseItemIdx}");
            }
           
        }

        public override void ScopedMutateChromosome(IMetaHeuristicContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex)
        {
            var phaseItemIdx = GetIndexGenerator(ctx)(this, ctx);
            IMetaHeuristic currentHeuristic = GetCurrentHeuristic(phaseItemIdx);
            if (currentHeuristic != null)
            {
                currentHeuristic.MutateChromosome(ctx, mutation, mutationProbability, offSprings, offspringIndex);
            }
            else
            {
                throw new ApplicationException($"No phase heuristic for MetaHeuristic {Guid} and phase index {phaseItemIdx}");
            }
           
        }

        public override IList<IChromosome> ScopedReinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            var phaseItemIdx = GetIndexGenerator(ctx)(this, ctx);
            IMetaHeuristic currentHeuristic = GetCurrentHeuristic(phaseItemIdx);
            if (currentHeuristic != null)
            {
                return currentHeuristic.Reinsert(ctx, reinsertion, offspring, parents);
            }
            else
            {
                throw new ApplicationException($"No phase heuristic for MetaHeuristic {Guid} and phase index {phaseItemIdx}");
            }
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