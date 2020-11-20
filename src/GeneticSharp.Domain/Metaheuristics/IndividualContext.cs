using System;
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

        public MetaHeuristicParameter GetParameter(string key)
        {
            return _populationContext.GetParameter(key);
        }
    }
}