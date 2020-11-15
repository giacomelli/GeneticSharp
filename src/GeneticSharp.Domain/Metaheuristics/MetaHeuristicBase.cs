using System;
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
    /// A base class for Metaheuristics. Provides an ID, and a helper to add and get items from generation cache.
    /// </summary>
    public abstract class MetaHeuristicBase : IMetaHeuristic
    {
        /// <summary>
        /// An ID to identify the current metaheuristic, useful for caching
        /// </summary>
        public Guid Guid { get; set; } = Guid.NewGuid();

        /// <inheritdoc />
        public abstract IList<IChromosome> SelectParentPopulation(IPopulation population, ISelection selection);

        /// <inheritdoc />
        public abstract IList<IChromosome> MatchParentsAndCross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex);

        /// <inheritdoc />
        public abstract void MutateChromosome(IPopulation population, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex);

        /// <inheritdoc />
        public abstract IList<IChromosome> Reinsert(IPopulation population, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents);


        /// <summary>
        /// Allows storing and retrieving objects in a generation based cache, specific to the heuristics or to be used in any heuristics for the current generation
        /// </summary>
        /// <typeparam name="TItemType">The type of object to store</typeparam>
        /// <param name="isHeuristicsSpecific">specifies if the object is specific to the heuristics or to be used in any heuristics for the current generation</param>
        /// <param name="population">the population, the current generation of which offers the current storage</param>
        /// <param name="key">the key for the object storage and retrieval</param>
        /// <param name="factory">the factory to build the object if not found in the cache</param>
        /// <returns></returns>
        public TItemType GetOrAddContextItem<TItemType>(bool isHeuristicsSpecific,  IPopulation population, string key, Func<TItemType> factory)
        {
            if (isHeuristicsSpecific)
            {
                key = GetHeuristcSpecificKey(key);
            }
            return (TItemType) population.CurrentGeneration.Context.GetOrAdd(key, s => (object) factory());
        }

        //public TItemType AddIfAbsentContextItem<TItemType>(bool isHeuristicsSpecific, IPopulation population, string key, Func<TItemType> factory)
        //{
        //    if (isHeuristicsSpecific)
        //    {
        //        key = GetHeuristcSpecificKey(key);
        //    }
        //    return (TItemType)population.CurrentGeneration.Context.AddOrUpdate(key, s => (object)factory(), (s, o) => o);
        //}

        private string GetHeuristcSpecificKey(string key)
        {
            return $"{Guid.ToString()}-{key}";
        }


        
        public IMetaHeuristic WithParam()
        {
            return this;
        }

    }
}