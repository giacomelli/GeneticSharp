using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// The ContainerMetaHeuristic is a common base class to hijack certain operations while providing a default fallback for other operations
    /// </summary>
    [DisplayName("Container")]
    public class ContainerMetaHeuristic : MetaHeuristicBase, IContainerMetaHeuristic
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


        public override IList<IChromosome> SelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            return SubMetaHeuristic.SelectParentPopulation(ctx, selection);
        }

        public override IList<IChromosome> MatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            if (ShouldRun(crossoverProbability, CrossoverProbabilityStrategy, StaticCrossoverProbability, out crossoverProbability))
            {
                return SubMetaHeuristic.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
            }

            return new List<IChromosome>();

        }


        public override void MutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            if (ShouldRun(mutationProbability, MutationProbabilityStrategy, StaticMutationProbability, out mutationProbability))
            {
                SubMetaHeuristic.MutateChromosome(ctx, mutation, mutationProbability, offSprings);
            }
           
        }

        public override IList<IChromosome> Reinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return SubMetaHeuristic.Reinsert(ctx, reinsertion, offspring, parents);
        }


        public override void RegisterParameters(IEvolutionContext ctx)
        {
            base.RegisterParameters(ctx);
            ((MetaHeuristicBase) SubMetaHeuristic).RegisterParameters(ctx);
        }


        protected bool ShouldRun(float initialProbability, ProbabilityStrategy strategy, float staticProbability, out float subProbability )
        {
            subProbability = (strategy & ProbabilityStrategy.OverwriteProbability) == ProbabilityStrategy.OverwriteProbability ? staticProbability : initialProbability;
            if ((strategy & ProbabilityStrategy.TestProbability) != ProbabilityStrategy.TestProbability) return true;
            if (!(Math.Abs(subProbability - 1) > float.Epsilon) || RandomizationProvider.Current.GetDouble() < subProbability)
            {
                subProbability = 1;
                return true;
            }

            return false;
        }

    }
}