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


        public override IList<IChromosome> SelectParentPopulation(IPopulation population, ISelection selection)
        {
            return SubMetaHeuristic.SelectParentPopulation(population, selection);
        }

        public override IList<IChromosome> MatchParentsAndCross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex)
        {
            return SubMetaHeuristic.MatchParentsAndCross(population, crossover, crossoverProbability, parents,
                firstParentIndex);
        }


        public override void MutateChromosome(IPopulation population, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex)
        {
            SubMetaHeuristic.MutateChromosome(population, mutation, mutationProbability, offSprings, offspringIndex);
        }

        public override IList<IChromosome> Reinsert(IPopulation population, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return SubMetaHeuristic.Reinsert(population, reinsertion, offspring, parents);
        }


        /// <summary>
        /// This fluent helper allows to define the sub metaheuristic after the container definition
        /// </summary>
        /// <param name="subMetaHeuristic">the sub metaheuristic for the current container</param>
        /// <returns>the current container metaheuristic</returns>
        public ContainerMetaHeuristic WithDefault(IMetaHeuristic subMetaHeuristic)
        {
            SubMetaHeuristic = subMetaHeuristic;
            return this;
        }

    }
}