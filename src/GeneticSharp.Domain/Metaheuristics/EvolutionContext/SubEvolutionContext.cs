using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// An individual context adds to a generation context a specific individual Index.
    /// </summary>
    public abstract class SubEvolutionContext : IEvolutionContext
    {


        public SubEvolutionContext(IEvolutionContext populationContext)
        {
            _populationContext = populationContext;
            //SelectedParents = populationContext.SelectedParents;
            //GeneratedOffsprings = populationContext.GeneratedOffsprings;
        }

       

        private readonly IEvolutionContext _populationContext;

        /// <inheritdoc />
        public IGeneticAlgorithm GeneticAlgorithm
        {
            get => PopulationContext.GeneticAlgorithm;
            set => PopulationContext.GeneticAlgorithm = value;
        }

        /// <inheritdoc />
        public virtual IPopulation Population
        {
            get => PopulationContext.Population;
            set => PopulationContext.Population = value;
        }

        public virtual int OriginalIndex
        {
            get => PopulationContext.OriginalIndex;
        }


        public virtual int LocalIndex
        {
            get => PopulationContext.LocalIndex;
        }

        /// <inheritdoc />
        public EvolutionStage CurrentStage
        {
            get => PopulationContext.CurrentStage;
            set => PopulationContext.CurrentStage = value;
        }

        /// <inheritdoc />
        public virtual IList<IChromosome> SelectedParents
        {
            get => PopulationContext.SelectedParents;
            set => PopulationContext.SelectedParents = value;
        }

        /// <inheritdoc />
        public virtual IList<IChromosome> GeneratedOffsprings
        {
            get => PopulationContext.GeneratedOffsprings;
            set => PopulationContext.GeneratedOffsprings = value;
        }


        public IEvolutionContext PopulationContext => _populationContext;

       

        public virtual IEvolutionContext GetIndividual(int index)
        {
            //return PopulationContext.GetIndividual(index);
            return new IndividualContext(this, index, index);
        }

        //public IEvolutionContext GetLocal(int index)
        //{
        //    return PopulationContext.GetLocal(index);
        //}

        public IEvolutionContext GetLocal(int index)
        {
            if (OriginalIndex < 0)
            {
                throw new InvalidOperationException("a local context can only be created from an individual context");
            }

            return new IndividualContext(this, OriginalIndex, index);
        }


        /// <inheritdoc />
        public void RegisterParameter(string paramName, IMetaHeuristicParameter param)
        {
            PopulationContext.RegisterParameter(paramName, param);
        }

        /// <inheritdoc />
        public IMetaHeuristicParameter GetParameterDefinition(string paramName)
        {
            return PopulationContext.GetParameterDefinition(paramName);
        }

        /// <inheritdoc />
        public virtual TItemType GetOrAdd<TItemType>((string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) contextKey, Func<TItemType> factory)
        {
            return PopulationContext.GetOrAdd(contextKey, factory);
        }

        /// <inheritdoc />
        public virtual TItemType GetParam<TItemType>(IMetaHeuristic h, string paramName)
        {
            //return PopulationContext.GetParamWithContext<TItemType>(h, paramName, this);
            var paramDef = PopulationContext.GetParameterDefinition(paramName);
            return paramDef.Get<TItemType>(h, this, paramName);
        }

       
    }
}