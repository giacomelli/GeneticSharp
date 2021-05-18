using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// The default implementation of the <see cref="IEvolutionContext"/> uses a Concurrent Dictionary for parameters to allow for parallel Operators to consume the same context.
    /// </summary>
    public class EvolutionContext : IEvolutionContext
    {

        #region ctors

        public EvolutionContext()
        {
        }

       

        #endregion

        #region Fields


        private readonly Dictionary<string, IMetaHeuristicParameter> _paramDefinitions = new Dictionary<string, IMetaHeuristicParameter>();

        #endregion


        #region Properties

        /// <inheritdoc />
        public IGeneticAlgorithm GeneticAlgorithm { get; set; }


        /// <inheritdoc />
        public IPopulation Population { get; set; }


        /// <inheritdoc />
        public int OriginalIndex { get; set; } = -1;


        /// <inheritdoc />
        public int LocalIndex { get; set; } = -1;


        /// <inheritdoc />
        public EvolutionStage CurrentStage { get; set; }

        /// <inheritdoc />
        public IList<IChromosome> SelectedParents { get; set; }

        /// <inheritdoc />
        public IList<IChromosome> GeneratedOffsprings { get; set; }


        /// <summary>
        /// Allows storing and reusing objects during operators evaluation
        /// </summary>
        public ConcurrentDictionary<(string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual), object> Params { get; set; } = new ConcurrentDictionary<(string, int, EvolutionStage, IMetaHeuristic, int), object>();



        #endregion

        #region Public Methods

        /// <inheritdoc />
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEvolutionContext GetIndividual(int index)
        {
            return new IndividualContext(this, index, index);
        }

        public IEvolutionContext GetLocal(int index)
        {
            if (OriginalIndex<0)
            {
                throw new InvalidOperationException("a local context can only be created from an individual context");
            }

            return new IndividualContext(this, OriginalIndex, index);
        }


        /// <inheritdoc />
        public TItemType GetOrAdd<TItemType>((string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) contextKey, Func<TItemType> factory)
        {
            var toReturn = (TItemType)Params.GetOrAdd(contextKey, s => (object)factory());
            return toReturn;
        }

        /// <inheritdoc />
        public TItemType GetParam<TItemType>(IMetaHeuristic h, string paramName)
        {
            var paramDef = GetParameterDefinition(paramName);
            return paramDef.Get<TItemType>(h, this, paramName);
            //return GetParamWithContext<TItemType>(h, paramName, this);
        }


      

        /// <inheritdoc />
        public void RegisterParameter(string paramName, IMetaHeuristicParameter param)
        {
            //_paramDefinitions.Add(paramName, param);
            //todo: better handler collisions
            _paramDefinitions[paramName]= param;
        }

        /// <inheritdoc />
        public IMetaHeuristicParameter GetParameterDefinition(string paramName)
        {
            if (_paramDefinitions.TryGetValue(paramName, out var paramDef))
            {
                return paramDef;
            }
            throw new ArgumentException($"parameter {paramName} not found in MetaHeuristic expression chain", nameof(paramName));
        }

      

        #endregion




    }
}