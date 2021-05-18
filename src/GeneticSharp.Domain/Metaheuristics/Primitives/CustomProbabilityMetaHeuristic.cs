using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics.Probability;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Collections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    public abstract class CustomProbabilityMetaHeuristic : MetaHeuristicBase
    {
        public OperatorsProbabilityConfig ProbabilityConfig { get; set; } = new OperatorsProbabilityConfig(); 


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override IList<IChromosome> MatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            IList<IChromosome> toReturn = null;
            var baseProbability = ProbabilityConfig.Crossover.GetProbability(ctx, crossoverProbability);
            while (ShouldRun(baseProbability, ProbabilityConfig.Crossover.Strategy,  out var subCrossoverProbability))
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
            var baseProbability = ProbabilityConfig.Mutation.GetProbability(ctx, mutationProbability);
            while (ShouldRun(baseProbability, ProbabilityConfig.Mutation.Strategy,  out var subMutationProbability))
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