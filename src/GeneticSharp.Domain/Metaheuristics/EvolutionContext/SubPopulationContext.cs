using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// An individual context adds to a generation context a specific individual Index.
    /// </summary>
    public class SubPopulationContext : SubEvolutionContext
    {
        /// <inheritdoc />
        public override IPopulation Population { get; set; }


        /// <inheritdoc />
        public override IList<IChromosome> SelectedParents { get; set; }

        /// <inheritdoc />
        public override IList<IChromosome> GeneratedOffsprings { get; set; }

        /// <summary>
        /// Allows storing and reusing objects during operators evaluation
        /// </summary>
        public ConcurrentDictionary<(string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual), object> Params { get; set; } = new ConcurrentDictionary<(string, int, EvolutionStage, IMetaHeuristic, int), object>();


        public SubPopulationContext(IEvolutionContext populationContext, IPopulation subPopulation) : base(populationContext)
        {

            Population = subPopulation;
            GeneratedOffsprings = new List<IChromosome>();

        }

        public override TItemType GetOrAdd<TItemType>(
            (string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) contextKey,
            Func<TItemType> factory)
        {
            var toReturn = (TItemType)Params.GetOrAdd(contextKey, s => (object)factory());
            return toReturn;
        }

        //public override TItemType GetParam<TItemType>(IMetaHeuristic h, string paramName)
        //{
        //    return PopulationContext.GetParamWithContext<TItemType>(h, paramName, this);
        //}
    }
}