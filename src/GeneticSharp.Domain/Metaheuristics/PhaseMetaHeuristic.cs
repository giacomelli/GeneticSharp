using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class PhaseMetaHeuristic<TIndex> : PhaseMetaHeuristicBase<TIndex>
    {
        
        public PhaseMetaHeuristic() : base(){}

        //public PhaseMetaHeuristic(ParameterGenerator<TIndex> phaseGenerator) : base()
        //{
        //    PhaseGenerator = phaseGenerator;
        //}

        //public PhaseMetaHeuristic(int phaseSize, params IMetaHeuristic[] phaseHeuristics):base(phaseSize, phaseHeuristics) {}

        public ParameterGenerator<TIndex> PhaseGenerator { get; set; } 

        public override IList<IChromosome> ScopedSelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection)
        {
            var phaseItemIdx = PhaseGenerator(ctx);
            //var currentHeuristic = ctx.GetOrAdd<IMetaHeuristic>(ParameterScope.MetaHeuristic, this, phaseItemIdx.ToString(CultureInfo.InvariantCulture), () => GetCurrentHeuristic(phaseItemIdx));
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
            //var phaseItemIdx = PhaseGenerator(ctx);
            //var currentHeuristic = ctx.GetOrAdd<IMetaHeuristic>(ParameterScope.MetaHeuristic, this, phaseItemIdx.ToString(CultureInfo.InvariantCulture), () => GetCurrentHeuristic(phaseItemIdx));
            //return currentHeuristic.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents,
            //    firstParentIndex);
            var phaseItemIdx = PhaseGenerator(ctx);
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
            //var phaseItemIdx = PhaseGenerator(ctx);
            //var currentHeuristic = ctx.GetOrAdd<IMetaHeuristic>(ParameterScope.MetaHeuristic, this, phaseItemIdx.ToString(CultureInfo.InvariantCulture), () => GetCurrentHeuristic(phaseItemIdx));
            //currentHeuristic.MutateChromosome(ctx, mutation, mutationProbability, offSprings, offspringIndex);
            var phaseItemIdx = PhaseGenerator(ctx);
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
            //var phaseItemIdx = PhaseGenerator(ctx);
            //var currentHeuristic = ctx.GetOrAdd<IMetaHeuristic>(ParameterScope.MetaHeuristic, this, phaseItemIdx.ToString(CultureInfo.InvariantCulture), () => GetCurrentHeuristic(phaseItemIdx));
            //return currentHeuristic.Reinsert(ctx, reinsertion, offspring, parents);
            var phaseItemIdx = PhaseGenerator(ctx);
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
            IMetaHeuristic currentHeuristic = null;
            if (PhaseHeuristics.TryGetValue(phaseItemIndex, out currentHeuristic))
            {
                return currentHeuristic;
            }

            return null;
        }

       
    }
}