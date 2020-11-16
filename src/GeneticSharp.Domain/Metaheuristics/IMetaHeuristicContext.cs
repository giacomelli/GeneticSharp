using System;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{

    public delegate TParamType ParameterGenerator<TParamType>(IMetaHeuristic h, IMetaHeuristicContext ctx);

    public interface IMetaHeuristicContext
    {
        IGeneticAlgorithm GA { get; set; }
        IPopulation Population { get; set; }
        int Count { get; set; }
        int Index { get; set; }

        MetaHeuristicsStage CurrentStage { get; set; }

        TValue Get<TValue>(IMetaHeuristic h, string paramName);


        /// <summary>
        /// Allows storing and retrieving objects in a generation based cache, specific to the heuristics or to be used in any heuristics for the current generation
        /// </summary>
        /// <typeparam name="TItemType">The type of object to store</typeparam>
        /// <param name="isHeuristicsSpecific">specifies if the object is specific to the heuristics or to be used in any heuristics for the current generation</param>
        /// <param name="population">the population, the current generation of which offers the current storage</param>
        /// <param name="key">the key for the object storage and retrieval</param>
        /// <param name="factory">the factory to build the object if not found in the cache</param>
        /// <returns></returns>
        TItemType GetOrAdd<TItemType>(ParameterScope scope, IMetaHeuristic heuristic, string key,
            Func<TItemType> factory);


        void RegisterParameter(string key, MetaHeuristicParameter param);


    }
}