using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{

    public struct IndividualContext : IMetaHeuristicContext
    {


        public IndividualContext(MetaHeuristicContext populationContext, int index)
        {
            _populationContext = populationContext;
            Index = index;
        }

        public int Index { get; set; }

        private MetaHeuristicContext _populationContext;
        public IGeneticAlgorithm GA
        {
            get => _populationContext.GA;
            set => _populationContext.GA = value;
        }

        public IPopulation Population
        {
            get => _populationContext.Population;
            set => _populationContext.Population = value;
        }

        public int Count
        {
            get => _populationContext.Count;
            set => _populationContext.Count = value;
        }

        public MetaHeuristicsStage CurrentStage
        {
            get => _populationContext.CurrentStage;
            set => _populationContext.CurrentStage = value;
        }

        public TValue Get<TValue>(IMetaHeuristic h, string paramName)
        {
            return _populationContext.GetWithIndex<TValue>(h, paramName, Index);
        }

        public IMetaHeuristicContext GetIndividual(int index)
        {
            return _populationContext.GetIndividual(index);
        }

        public TItemType GetOrAdd<TItemType>(ParameterScope scope, IMetaHeuristic heuristic, string key, Func<TItemType> factory)
        {
            return _populationContext.GetOrAddWithIndex(scope, heuristic, key, factory, Index);
        }

        public void RegisterParameter(string key, MetaHeuristicParameter param)
        {
            _populationContext.RegisterParameter(key, param);
        }
    }

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



        public string GetParamKeyWithIndex(ParameterScope scope, IMetaHeuristic heuristic, string key, int index)
        {

            if ((scope & ParameterScope.Individual) == ParameterScope.Individual)
            {
                key = $"Ind{index}-{key}";
            }
            if ((scope & ParameterScope.MetaHeuristic) == ParameterScope.MetaHeuristic)
            {
                key = $"Heur{heuristic.Guid}-{key}";
            }
            if ((scope & ParameterScope.Stage) == ParameterScope.Stage)
            {
                key = $"Stage{CurrentStage}-{key}";
            }
            if ((scope & ParameterScope.Generation) == ParameterScope.Generation)
            {
                key = $"Gen{Population.GenerationsNumber}-{key}";
            }

            return key;
        }


    }
}