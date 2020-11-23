using System;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{

   



    public interface IMetaHeuristicContext
    {

        
        IGeneticAlgorithm GA { get; set; }
        IPopulation Population { get; set; }
        int Count { get; set; }

        int Index { get; set; }

        MetaHeuristicsStage CurrentStage { get; set; }

        //TValue Get<TValue>(IMetaHeuristic h, string paramName);

        IMetaHeuristicContext GetIndividual(int index);

        /// <summary>
        /// Allows storing and retrieving objects in a generation based cache, specific to the heuristics or to be used in any heuristics for the current generation
        /// </summary>
        /// <typeparam name="TItemType">The type of object to store</typeparam>
        /// <param name="key">the key for the object storage and retrieval</param>
        /// <param name="factory">the factory to build the object if not found in the cache</param>
        /// <returns></returns>
        TItemType GetOrAdd<TItemType>((string key, int generation, MetaHeuristicsStage stage, IMetaHeuristic heuristic, int individual) contextKey, Func<TItemType> factory);

        TItemType GetParam<TItemType>(IMetaHeuristic h, string paramName);

        void RegisterParameter(string key, IMetaHeuristicParameter param);

        IMetaHeuristicParameter GetParameterDefinition(string key);

    }
}