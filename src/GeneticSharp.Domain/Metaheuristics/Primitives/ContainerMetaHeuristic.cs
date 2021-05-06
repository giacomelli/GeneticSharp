using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// The ContainerMetaHeuristic is a common base class to hijack certain operations while providing a default fallback for other operations
    /// </summary>
    [DisplayName("Container")]
    public class ContainerMetaHeuristic : CustomProbabilityMetaHeuristic, IContainerMetaHeuristic
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


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IList<IChromosome> SelectParentPopulation(IEvolutionContext ctx, ISelection selection)
        {
            return SubMetaHeuristic.SelectParentPopulation(ctx, selection);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override IList<IChromosome> DoMatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            return SubMetaHeuristic.MatchParentsAndCross(ctx, crossover, crossoverProbability, parents);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void DoMutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings)
        {
                SubMetaHeuristic.MutateChromosome(ctx, mutation, mutationProbability, offSprings);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IList<IChromosome> Reinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return SubMetaHeuristic.Reinsert(ctx, reinsertion, offspring, parents);
        }


        public override void RegisterParameters(IEvolutionContext ctx)
        {
            base.RegisterParameters(ctx);
            ((MetaHeuristicBase) SubMetaHeuristic).RegisterParameters(ctx);
        }


     

    }
}