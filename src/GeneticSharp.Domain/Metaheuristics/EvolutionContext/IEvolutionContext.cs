using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{

    /// <summary>
    /// The Evolution Context serves the evolution state throughout the generations cycles, with access to the current global and scoped parameters.
    /// </summary>
    public interface IEvolutionContext
    {
        
        //string Guid { get; }
        
        /// <summary>
        /// The genetic algorithm being run
        /// </summary>
        IGeneticAlgorithm GeneticAlgorithm { get; set; }

        /// <summary>
        /// The current population being evolved
        /// </summary>
        IPopulation Population { get; set; }

        /// <summary>
        /// The original individual index among global stage population (parents for crossover and offspring for mutations)
        /// </summary>
        int OriginalIndex { get; }

        /// <summary>
        /// The local individual index for custom matches from filtered parents
        /// </summary>
        int LocalIndex { get; }

        /// <summary>
        /// The current evolution stage among <see cref="EvolutionStage"/>
        /// </summary>
        EvolutionStage CurrentStage { get; set; }

        /// <summary>
        /// Keeps track of the currently selected parents during the Crossover stage
        /// </summary>
        IList<IChromosome> SelectedParents { get; set; }

        /// <summary>
        /// Keeps track of the currently generated offspring during the mutation stage
        /// </summary>
        IList<IChromosome> GeneratedOffsprings { get; set; }

        /// <summary>
        /// Creates an individual specific context from a generation specific context
        /// </summary>
        /// <param name="index">the individual index to personalize the generation context</param>
        /// <returns>A context with Index set to the passed index</returns>
        IEvolutionContext GetIndividual(int index);

        /// <summary>
        /// Creates an local specific context from a individual specific context
        /// </summary>
        /// <param name="index">the individual index to personalize the individual context</param>
        /// <returns>A context with LocalIndex set to the passed index and OriginalIndex kept unchanged</returns>
        IEvolutionContext GetLocal(int index);


        //IEvolutionContext GetSubContext(int subContextId, IPopulation subPopulation);

        /// <summary>
        /// Allows storing and retrieving objects in a generation based cache, specific to the heuristics or to be used in any heuristics for the current generation
        /// </summary>
        /// <typeparam name="TItemType">The type of object to store</typeparam>
        /// <param name="contextKey">the paramName for the object storage and retrieval</param>
        /// <param name="factory">the factory to build the object if not found in the cache</param>
        /// <returns></returns>
        TItemType GetOrAdd<TItemType>((string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) contextKey, Func<TItemType> factory);

        /// <summary>
        /// Allows invoking a parameter previously registered to the context by the corresponding parameter name, for instance in an Expression based Metaheuristic operator
        /// </summary>
        /// <typeparam name="TItemType">The expected parameter type</typeparam>
        /// <param name="h">the current metaheuristic (maybe null if not scoped by metaheuristic)</param>
        /// <param name="paramName">The name of the parameter to invoke</param>
        /// <returns>The typed value of the named parameter invoked</returns>
        TItemType GetParam<TItemType>(IMetaHeuristic h, string paramName);

        /// <summary>
        /// Allows registering a contextual parameter for further invocation.
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="param"></param>
        void RegisterParameter(string paramName, IMetaHeuristicParameter param);


        /// <summary>
        /// Given the name of a previously registered parameter, returns the corresponding definition
        /// </summary>
        /// <param name="paramName">The parameter name</param>
        /// <returns></returns>
        IMetaHeuristicParameter GetParameterDefinition(string paramName);

        
       

    }
}