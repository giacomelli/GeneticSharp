using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    public abstract class CustomProbabilityMetaHeuristic : MetaHeuristicBase
    {

        public ProbabilityStrategy CrossoverProbabilityStrategy { get; set; } //= ProbabilityStrategy.TestProbability | ProbabilityStrategy.OverwriteProbability;

        public float StaticCrossoverProbability { get; set; } = 1;

        public ProbabilityStrategy MutationProbabilityStrategy { get; set; }

        public float StaticMutationProbability { get; set; } = 1;


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override IList<IChromosome> MatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            if (ShouldRun(crossoverProbability, CrossoverProbabilityStrategy, StaticCrossoverProbability, out crossoverProbability))
            {
                return DoMatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
            }

            return null;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override void MutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            if (ShouldRun(mutationProbability, MutationProbabilityStrategy, StaticMutationProbability, out mutationProbability))
            {
                DoMutateChromosome(ctx, mutation, mutationProbability, offSprings);
            }
        }


        protected abstract IList<IChromosome> DoMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover,
            float crossoverProbability, IList<IChromosome> parents);


        protected abstract void DoMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability,
            IList<IChromosome> offSprings);


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool ShouldRun(float initialProbability, ProbabilityStrategy strategy, float staticProbability, out float subProbability)
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