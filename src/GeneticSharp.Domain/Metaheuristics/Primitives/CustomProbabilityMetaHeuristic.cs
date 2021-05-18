using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Collections;

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
            IList<IChromosome> toReturn = null;
            var baseProbability =  (CrossoverProbabilityStrategy & ProbabilityStrategy.OverwriteProbability) == ProbabilityStrategy.OverwriteProbability ? StaticCrossoverProbability : crossoverProbability;
            while (ShouldRun(baseProbability, CrossoverProbabilityStrategy,  out var subCrossoverProbability))
            {
                var newChildren = DoMatchParentsAndCross(ctx, crossover, subCrossoverProbability, parents);
                if (toReturn == null)
                {
                    toReturn = newChildren;
                }
                else
                {
                    toReturn.AddRange(newChildren);
                }
                baseProbability--;
            }

            return toReturn;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override void MutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
            var baseProbability = (MutationProbabilityStrategy & ProbabilityStrategy.OverwriteProbability) == ProbabilityStrategy.OverwriteProbability ? StaticMutationProbability : mutationProbability;
            while (ShouldRun(baseProbability, MutationProbabilityStrategy,  out var subMutationProbability))
            {
                DoMutateChromosome(ctx, mutation, subMutationProbability, offSprings);
                baseProbability--;
            }
        }


        protected abstract IList<IChromosome> DoMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover,
            float crossoverProbability, IList<IChromosome> parents);


        protected abstract void DoMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability,
            IList<IChromosome> offSprings);


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool ShouldRun(float baseProbability, ProbabilityStrategy strategy, out float subProbability)
        {
            subProbability = baseProbability;
            if (baseProbability<0)
            {
                return false;
            }
            if ((strategy & ProbabilityStrategy.TestProbability) != ProbabilityStrategy.TestProbability) return true;
            if ( 1 - subProbability < float.Epsilon || RandomizationProvider.Current.GetDouble() < subProbability)
            {
                subProbability = 1;
                return true;
            }
            return false;
        }


    }
}