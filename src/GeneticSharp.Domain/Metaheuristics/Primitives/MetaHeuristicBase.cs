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
        public abstract IList<IChromosome> SelectParentPopulation(IEvolutionContext ctx, ISelection selection);

        /// <inheritdoc />
        public abstract IList<IChromosome> MatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents);

        /// <inheritdoc />
        public abstract void MutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings);

        /// <inheritdoc />
        public abstract IList<IChromosome> Reinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents);




        //public TItemType AddIfAbsentContextItem<TItemType>(bool isHeuristicsSpecific, IPopulation population, string key, Func<TItemType> factory)
        //{
        //    if (isHeuristicsSpecific)
        //    {
        //        key = GetHeuristcSpecificKey(key);
        //    }
        //    return (TItemType)population.CurrentGeneration.Context.AddOrUpdate(key, s => (object)factory(), (s, o) => o);
        //}

        public IEvolutionContext GetContext(IGeneticAlgorithm ga, IPopulation population)
        {
            
            if (population.Parameters.TryGetValue(nameof(IEvolutionContext), out var cachedContext))
            {
                return (IEvolutionContext) cachedContext;
            }

            lock (population)
            {
                if (population.Parameters.TryGetValue(nameof(IEvolutionContext), out cachedContext))
                {
                    return (IEvolutionContext)cachedContext;
                }
                var toReturn = new EvolutionContext { GA = ga, Population = population };
                RegisterParameters(toReturn);
                population.Parameters[nameof(IEvolutionContext)] = toReturn;
                return toReturn;
            }
        }



        public virtual void RegisterParameters(IEvolutionContext ctx)
        {
            RegisterParameters(Parameters, ctx);
        }

        protected void RegisterParameters(IDictionary<string, IMetaHeuristicParameter> parameters, IEvolutionContext ctx)
        {
            foreach (var metaHeuristicParameter in parameters)
            {
              ctx.RegisterParameter(metaHeuristicParameter.Key, metaHeuristicParameter.Value);
            }
        }

        

    }
}