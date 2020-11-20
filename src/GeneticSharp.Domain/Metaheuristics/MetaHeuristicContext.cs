using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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

        public IMetaHeuristicContext GetIndividual(int index)
        {
            return new IndividualContext(this, index);
        }


        public TValue Get<TValue>(IMetaHeuristic h, string paramName)
        {
            return GetWithIndex<TValue>(h, paramName, Index);
        }

        public TValue GetWithIndex<TValue>(IMetaHeuristic h, string paramName, int index)
        {
            _paramDefinitions.TryGetValue(paramName, out var paramDef);


            if (paramDef != null)
            {
                paramName = GetParamKeyWithIndex(paramDef.Scope, h, paramName, index);
            }

            if (Params.TryGetValue(paramName, out var dicValue))
            {
                return (TValue)dicValue;
            }

            if (paramDef!=null)
            {
                var toReturn = paramDef.ComputeParameter(h, this);
                Params[paramName] = toReturn;
                return (TValue) toReturn;
            }

            return default;
        }


        public TItemType GetOrAdd<TItemType>(ParameterScope scope, IMetaHeuristic heuristic, string key, Func<TItemType> factory)
        {
            return GetOrAddWithIndex(scope, heuristic, key, factory, Index);
        }


        public TItemType GetOrAddWithIndex<TItemType>(ParameterScope scope, IMetaHeuristic heuristic, string key, Func<TItemType> factory, int index)
        {
            if ((scope & ParameterScope.MetaHeuristic) == ParameterScope.MetaHeuristic)
            {
                key = GetParamKeyWithIndex(scope, heuristic, key, index);
            }
            return (TItemType)Params.GetOrAdd(key, s => (object)factory());
        }


        public void RegisterParameter(string key, MetaHeuristicParameter param)
        {
            _paramDefinitions.Add(key, param);
        }

        public MetaHeuristicParameter GetParameter(string key)
        {
            if (_paramDefinitions.TryGetValue(key, out var paramDef))
            {
                return paramDef;
            }
            throw new ArgumentException($"parameter {key} not found in MetaHeuristic expression chain", nameof(key));
        }


        public string GetParamKeyWithIndex(ParameterScope scope, IMetaHeuristic heuristic, string key, int index)
        {
            var sb = new StringBuilder(key);
            if ((scope & ParameterScope.Generation) == ParameterScope.Generation)
            {
                sb.Append("G");
                sb.Append(Population.GenerationsNumber.ToString(CultureInfo.InvariantCulture));
            }
            if ((scope & ParameterScope.Stage) == ParameterScope.Stage)
            {
                sb.Append("S");
                sb.Append(CurrentStage.ToString());
            }
            if ((scope & ParameterScope.MetaHeuristic) == ParameterScope.MetaHeuristic)
            {
                sb.Append("H");
                sb.Append(heuristic.Guid);
            }
            if ((scope & ParameterScope.Individual) == ParameterScope.Individual)
            {
                sb.Append("I");
                sb.Append(index.ToString(CultureInfo.InvariantCulture));
            }

            sb.Append(key);

            var toReturn = sb.ToString();

            return toReturn;

        }


    }
}