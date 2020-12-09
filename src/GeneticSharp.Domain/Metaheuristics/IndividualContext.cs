using System;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    public struct IndividualContext : IEvolutionContext
    {


        public IndividualContext(EvolutionContext populationContext, int index)
        {
            _populationContext = populationContext;
            Index = index;
        }

        public int Index { get; set; }

        private readonly EvolutionContext _populationContext;
        public IGeneticAlgorithm GeneticAlgorithm
        {
            get => _populationContext.GeneticAlgorithm;
            set => _populationContext.GeneticAlgorithm = value;
        }

        public IPopulation Population
        {
            get => _populationContext.Population;
            set => _populationContext.Population = value;
        }

    
        public EvolutionStage CurrentStage
        {
            get => _populationContext.CurrentStage;
            set => _populationContext.CurrentStage = value;
        }

   
        public IEvolutionContext GetIndividual(int index)
        {
            if (index!=Index)
            {
                return _populationContext.GetIndividual(index);
            }
            return this;
        }

        public TItemType GetOrAdd<TItemType>((string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) contextKey, Func<TItemType> factory)
        {
            return _populationContext.GetOrAdd(contextKey, factory);
        }

        public TItemType GetParam<TItemType>(IMetaHeuristic h, string paramName)
        {
            return _populationContext.GetParamWithContext<TItemType>(h, paramName, this);
        }

     
        public void RegisterParameter(string key, IMetaHeuristicParameter param)
        {
            _populationContext.RegisterParameter(key, param);
        }

        public IMetaHeuristicParameter GetParameterDefinition(string key)
        {
            return _populationContext.GetParameterDefinition(key);
        }
    }
}