using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// The ScopedBasedMetaHeuristic provides a base class to target specifically some MetaHeuristics operators, while using a fallback Metaheuristic for others
    /// </summary>
    public abstract class ScopedBasedMetaHeuristic : ContainerMetaHeuristic
    {

        public ScopedBasedMetaHeuristic():base() { }

        public ScopedBasedMetaHeuristic(IMetaHeuristic subMetaHeuristic):base(subMetaHeuristic) {}

        /// <summary>
        /// The scope for the current MetaHeuristic behavior. Container fallback is used for other operators
        /// </summary>
        public MetaHeuristicsScope Scope { get; set; } = MetaHeuristicsScope.All;

        public sealed override  IList<IChromosome> SelectParentPopulation(IPopulation population, ISelection selection)
        {
            if ((Scope & MetaHeuristicsScope.Selection) == MetaHeuristicsScope.Selection)
            {
                return ScopedSelectParentPopulation(population, selection);
            }
            else
            {
                return SubMetaHeuristic.SelectParentPopulation(population, selection);
            }
        }

        public sealed override IList<IChromosome> MatchParentsAndCross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            if ((Scope & MetaHeuristicsScope.Crossover) == MetaHeuristicsScope.Crossover)
            {
                return ScopedMatchParentsAndCross(population, crossover, crossoverProbability, parents, firstParentIndex);
            }
            else
            {
                return SubMetaHeuristic.MatchParentsAndCross(population, crossover, crossoverProbability, parents, firstParentIndex);
            }
        }

        public sealed override void MutateChromosome(IPopulation population, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex)
        {
            if ((Scope & MetaHeuristicsScope.Mutation) == MetaHeuristicsScope.Mutation)
            {
                ScopedMutateChromosome(population, mutation, mutationProbability, offSprings, offspringIndex);
            }
            else
            {
                SubMetaHeuristic.MutateChromosome(population, mutation, mutationProbability, offSprings, offspringIndex);
            }
        }

        public sealed override IList<IChromosome> Reinsert(IPopulation population, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            if ((Scope & MetaHeuristicsScope.Mutation) == MetaHeuristicsScope.Mutation)
            {
               return ScopedReinsert(population, reinsertion, offspring, parents);
            }
            else
            {
                return SubMetaHeuristic.Reinsert(population, reinsertion, offspring, parents);
            }


            return base.Reinsert(population, reinsertion, offspring, parents);
        }

        public abstract IList<IChromosome> ScopedSelectParentPopulation(IPopulation population, ISelection selection);

        public abstract IList<IChromosome> ScopedMatchParentsAndCross(IPopulation population, ICrossover crossover,
            float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex);

        public abstract  void ScopedMutateChromosome(IPopulation population, IMutation mutation,
            float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex);

        public abstract IList<IChromosome> ScopedReinsert(IPopulation population, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents);

    }
}