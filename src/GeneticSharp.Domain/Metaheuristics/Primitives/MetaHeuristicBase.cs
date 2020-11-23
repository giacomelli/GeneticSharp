using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
   


    /// <summary>
    /// A base class for Metaheuristics. Provides an ID, and a helper to add and get items from generation cache.
    /// </summary>
    public abstract class MetaHeuristicBase : NamedEntity, IMetaHeuristic
    {
       

        public Dictionary<string, IMetaHeuristicParameter> Parameters { get; set; } = new Dictionary<string, IMetaHeuristicParameter>();

        /// <inheritdoc />
        public abstract IList<IChromosome> SelectParentPopulation(IMetaHeuristicContext ctx, ISelection selection);

        /// <inheritdoc />
        public abstract IList<IChromosome> MatchParentsAndCross(IMetaHeuristicContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents,
            int firstParentIndex);

        /// <inheritdoc />
        public abstract void MutateChromosome(IMetaHeuristicContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings,
            int offspringIndex);

        /// <inheritdoc />
        public abstract IList<IChromosome> Reinsert(IMetaHeuristicContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents);




        //public TItemType AddIfAbsentContextItem<TItemType>(bool isHeuristicsSpecific, IPopulation population, string key, Func<TItemType> factory)
        //{
        //    if (isHeuristicsSpecific)
        //    {
        //        key = GetHeuristcSpecificKey(key);
        //    }
        //    return (TItemType)population.CurrentGeneration.Context.AddOrUpdate(key, s => (object)factory(), (s, o) => o);
        //}

        public IMetaHeuristicContext GetContext(IGeneticAlgorithm ga, IPopulation population)
        {
            
            if (population.Parameters.TryGetValue(nameof(IMetaHeuristicContext), out var cachedContext))
            {
                return (IMetaHeuristicContext) cachedContext;
            }

            lock (population)
            {
                if (population.Parameters.TryGetValue(nameof(IMetaHeuristicContext), out cachedContext))
                {
                    return (IMetaHeuristicContext)cachedContext;
                }
                var toReturn = new MetaHeuristicContext()
                    { GA = ga, Population = population };
                RegisterParameters(toReturn);
                population.Parameters[nameof(IMetaHeuristicContext)] = toReturn;
                return toReturn;
            }
        }



        public virtual void RegisterParameters(IMetaHeuristicContext ctx)
        {
            RegisterParameters(this.Parameters, ctx);
        }

        protected void RegisterParameters(IDictionary<string, IMetaHeuristicParameter> parameters, IMetaHeuristicContext ctx)
        {
            foreach (var metaHeuristicParameter in parameters)
            {
              ctx.RegisterParameter(metaHeuristicParameter.Key, metaHeuristicParameter.Value);
            }
        }

        

    }
}