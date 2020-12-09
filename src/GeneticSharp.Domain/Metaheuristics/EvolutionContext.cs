using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    public class EvolutionContext : IEvolutionContext
    {

        public IGeneticAlgorithm GA { get; set; }

        public IPopulation Population { get; set; }


       
        public int Index { get; set; }

        public EvolutionStage CurrentStage { get; set; }


        /// <summary>
        /// Allows storing and reusing objects during operators evaluation
        /// </summary>
        public ConcurrentDictionary<(string, int, EvolutionStage, IMetaHeuristic, int), object> Params { get; set; } = new ConcurrentDictionary<(string, int, EvolutionStage, IMetaHeuristic, int), object>();

        private readonly Dictionary<string, IMetaHeuristicParameter> _paramDefinitions = new Dictionary<string, IMetaHeuristicParameter>();

        public IEvolutionContext GetIndividual(int index)
        {
            return new IndividualContext(this, index);
        }

        public TItemType GetOrAdd<TItemType>((string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) contextKey, Func<TItemType> factory)
        {
            var toReturn = (TItemType)Params.GetOrAdd(contextKey, s => (object)factory());
            return toReturn;
        }

        public TItemType GetParam<TItemType>(IMetaHeuristic h, string paramName)
        {
            return GetParamWithContext<TItemType>(h, paramName, this);
        }

        public TItemType GetParamWithContext<TItemType>(IMetaHeuristic h, string paramName, IEvolutionContext ctx)
        {
            _paramDefinitions.TryGetValue(paramName, out var paramDef);
            if (paramDef == null)
            {
                throw new ArgumentException($"parameter {paramName} was not registered", nameof(paramName));
            }
            return paramDef.GetOrAdd<TItemType>(h, ctx, paramName);
        }


        //public TValue Get<TValue>(IMetaHeuristic h, string paramName)
        //{
        //    return GetWithIndex<TValue>(h, paramName, Index);
        //}

        //public TValue GetWithIndex<TValue>(IMetaHeuristic h, string paramName, int index)
        //{
        //    _paramDefinitions.TryGetValue(paramName, out var paramDef);

        //    var key = paramName;
        //    if (paramDef != null)
        //    {
        //        key = paramDef.GetKey(key, Population, CurrentStage, h, index);
        //    }

        //    if (Params.TryGetValue(key, out var dicValue))
        //    {
        //        return (TValue)dicValue;
        //    }

        //    if (paramDef!=null)
        //    {
        //        var toReturn = paramDef.ComputeParameter(h, this);
        //        Params[key] = toReturn;
        //        return (TValue) toReturn;
        //    }

        //    return default;
        //}



        public void RegisterParameter(string key, IMetaHeuristicParameter param)
        {
            _paramDefinitions.Add(key, param);
        }

        public IMetaHeuristicParameter GetParameterDefinition(string key)
        {
            if (_paramDefinitions.TryGetValue(key, out var paramDef))
            {
                return paramDef;
            }
            throw new ArgumentException($"parameter {key} not found in MetaHeuristic expression chain", nameof(key));
        }


       


    }
}