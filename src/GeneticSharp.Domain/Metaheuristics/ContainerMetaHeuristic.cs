using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
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


        public ProbabilityStrategy CrossoverProbabilityStrategy { get; set; }

        public float StaticCrossoverProbability { get; set; } = 1;

        public ProbabilityStrategy MutationProbabilityStrategy { get; set; }

        public float StaticMutationProbability { get; set; } = 1;


        public override IList<IChromosome> SelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection)
        {
            return SubMetaHeuristic.SelectParentPopulation(ctx, selection);
        }

        public override IList<IChromosome> MatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            if (ShouldRun(crossoverProbability, CrossoverProbabilityStrategy, StaticCrossoverProbability, out crossoverProbability))
            {
                return SubMetaHeuristic.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents,
                    firstParentIndex);
            }

            return null;

        }


        public override void MutateChromosome(IMetaHeuristicContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex)
        {
            if (ShouldRun(mutationProbability, MutationProbabilityStrategy, StaticMutationProbability, out mutationProbability))
            {
                SubMetaHeuristic.MutateChromosome(ctx, mutation, mutationProbability, offSprings, offspringIndex);
            }
           
        }

        public override IList<IChromosome> Reinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return SubMetaHeuristic.Reinsert(ctx, reinsertion, offspring, parents);
        }


        public override void RegisterParameters(IMetaHeuristicContext ctx)
        {
            base.RegisterParameters(ctx);
            ((MetaHeuristicBase) SubMetaHeuristic).RegisterParameters(ctx);
        }


        protected bool ShouldRun(float initialProbability, ProbabilityStrategy strategy, float staticProbability, out float subProbability )
        {
            subProbability = (strategy & ProbabilityStrategy.ReplaceOriginal) == ProbabilityStrategy.ReplaceOriginal ? staticProbability : initialProbability;
            if ((strategy & ProbabilityStrategy.TestProbability) != ProbabilityStrategy.TestProbability) return true;
            return !(Math.Abs(initialProbability - 1) > float.Epsilon) || !(RandomizationProvider.Current.GetDouble() > 0.5);
        }

    }
}