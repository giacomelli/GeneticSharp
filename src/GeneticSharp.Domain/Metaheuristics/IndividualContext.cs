using System;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// An individual context adds to a generation context a specific individual Index.
    /// </summary>
    public struct IndividualContext : IEvolutionContext
    {


        public IndividualContext(EvolutionContext populationContext, int index)
        {
            _populationContext = populationContext;
            Index = index;
        }

        public int Index { get; set; }

        private readonly EvolutionContext _populationContext;

        /// <inheritdoc />
        public IGeneticAlgorithm GeneticAlgorithm
        {
            get => _populationContext.GeneticAlgorithm;
            set => _populationContext.GeneticAlgorithm = value;
        }

        /// <inheritdoc />
        public IPopulation Population
        {
            get => _populationContext.Population;
            set => _populationContext.Population = value;
        }

        /// <inheritdoc />
        public EvolutionStage CurrentStage
        {
            get => _populationContext.CurrentStage;
            set => _populationContext.CurrentStage = value;
        }

        /// <inheritdoc />
        public IEvolutionContext GetIndividual(int index)
        {
            if (index!=Index)
            {
                return _populationContext.GetIndividual(index);
            }
            return this;
        }

        /// <inheritdoc />
        public TItemType GetOrAdd<TItemType>((string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) contextKey, Func<TItemType> factory)
        {
            return _populationContext.GetOrAdd(contextKey, factory);
        }

        /// <inheritdoc />
        public TItemType GetParam<TItemType>(IMetaHeuristic h, string paramName)
        {
            return _populationContext.GetParamWithContext<TItemType>(h, paramName, this);
        }

        /// <inheritdoc />
        public void RegisterParameter(string paramName, IMetaHeuristicParameter param)
        {
            _populationContext.RegisterParameter(paramName, param);
        }

        /// <inheritdoc />
        public IMetaHeuristicParameter GetParameterDefinition(string paramName)
        {
            return _populationContext.GetParameterDefinition(paramName);
        }
    }
}