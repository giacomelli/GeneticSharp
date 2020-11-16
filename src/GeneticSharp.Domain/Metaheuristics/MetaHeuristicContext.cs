using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    public class MetaHeuristicContext : IMetaHeuristicContext
    {

        public IGeneticAlgorithm GA { get; set; }

        public IPopulation Population { get; set; }


        //public IMetaHeuristic MetaHeuristic { get; set; }


        public int Count { get; set; }

        public int Index { get; set; }

        public MetaHeuristicsStage CurrentStage { get; set; }


        /// <summary>
        /// Allows storing and reusing objects during operators evaluation
        /// </summary>
        public ConcurrentDictionary<string, object> Params { get; set; } = new ConcurrentDictionary<string, object>();

        private readonly Dictionary<string, MetaHeuristicParameter> _paramDefinitions = new Dictionary<string, MetaHeuristicParameter>();


        public TValue Get<TValue>(IMetaHeuristic h, string paramName)
        {
            _paramDefinitions.TryGetValue(paramName, out var paramDef);
            if (paramDef!=null)
            {
                paramName = GetParamKey(paramDef.Scope, h, paramName);
            }
            if (Params.TryGetValue(paramName, out var dicValue))
            {
                return (TValue)dicValue;
            }

            return default;
        }

        public TItemType GetOrAdd<TItemType>(ParameterScope scope, IMetaHeuristic heuristic, string key, Func<TItemType> factory)
        {
            if ((scope & ParameterScope.MetaHeuristic) == ParameterScope.MetaHeuristic)
            {
                key = GetParamKey(scope, heuristic, key);
            }
            return (TItemType)Params.GetOrAdd(key, s => (object)factory());
        }

        public void RegisterParameter(string key, MetaHeuristicParameter param)
        {
            _paramDefinitions.Add(key, param);
        }


        public string GetParamKey(ParameterScope scope, IMetaHeuristic heuristic, string key)
        {
            if ((scope & ParameterScope.Individual) == ParameterScope.Individual)
            {
                key = $"{Index}-{key}";
            }
            if ((scope & ParameterScope.MetaHeuristic) == ParameterScope.MetaHeuristic)
            {
                key = $"{heuristic.Guid}-{key}";
            }
            if ((scope & ParameterScope.Stage) == ParameterScope.Stage)
            {
                key = $"{CurrentStage}-{key}";
            }

            return key;
        }


    }
}