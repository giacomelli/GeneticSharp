using System;
using System.Collections.Concurrent;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    public interface IMetaHeuristicContext
    {
        IGeneticAlgorithm GA { get; set; }
        IPopulation Population { get; set; }
        int Count { get; set; }
        int Index { get; set; }

        TValue GetParam<TValue>(string paramName);


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


    }


    [Flags()]
    public enum ParameterScope
    {
        Global = 0,
        Generation = 1,
        MetaHeuristic = 2,
        Individual = 4
    }

    public delegate TParamType ParameterGenerator<TParamType>(IMetaHeuristicContext ctx);


    public abstract class MetaHeuristicParameter
    {
        public ParameterScope Scope { get; set; }


    }

    public class MetaHeuristicParameter<TParamType> : MetaHeuristicParameter
    {

        public ParameterGenerator<TParamType> Generator { get; set; }

    }

    


    public class MetaHeuristicContext : IMetaHeuristicContext
    {

        public IGeneticAlgorithm GA { get; set; }

        public IPopulation Population { get; set; }


        //public IMetaHeuristic MetaHeuristic { get; set; }


        public int Count { get; set; }

        public int Index { get; set; }


        /// <summary>
        /// Allows storing and reusing objects during operators evaluation
        /// </summary>
        public ConcurrentDictionary<string, object> Params { get; set; } = new ConcurrentDictionary<string, object>();


        public TValue GetParam<TValue>(string paramName)
        {
            throw new NotImplementedException();
        }

        public TItemType GetOrAdd<TItemType>(ParameterScope scope, IMetaHeuristic heuristic, string key, Func<TItemType> factory)
        {
            if ((scope & ParameterScope.MetaHeuristic) == ParameterScope.MetaHeuristic)
            {
                key = GetHeuristicKey(heuristic, key);
            }
            return (TItemType)Params.GetOrAdd(key, s => (object)factory());
        }


        public string GetHeuristicKey(IMetaHeuristic heuristic, string key)
        {
            return $"{heuristic.Guid}-{key}";
        }


    }


   
}