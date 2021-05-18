using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Collections;

namespace GeneticSharp.Domain
{
    /// <summary>
    /// An IOperatorsStrategy's implmentation which use Task Parallel Library (TPL) for parallel execution.
    /// </summary>
    public class TplOperatorsStrategy : IOperatorsStrategy
    {


        /// <summary>
        /// Crosses the specified parents.
        /// </summary>
        /// <param name="population">the current population to cross</param>
        /// <param name="crossover">The crossover class.</param>
        /// <param name="crossoverProbability">The crossover probability.</param>
        /// <param name="parents">The parents.</param>
        /// <returns>The result chromosomes.</returns>
        public IList<IChromosome> Cross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            //Switching to dictionary to restore order, which might be important (islands etc.)
            var offspring = new ConcurrentDictionary<int, IList<IChromosome>>();

            Parallel.ForEach(Enumerable.Range(0, population.MinSize / crossover.ParentsNumber).Select(i => i * crossover.ParentsNumber), i =>
            {
                var selectedParents = parents.Skip(i).Take(crossover.ParentsNumber).ToList();

                // If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
                // Checks if the number of selected parents is equal which the crossover expect, because the in the end of the list we can
                // have some rest chromosomes.
                if (selectedParents.Count == crossover.ParentsNumber && RandomizationProvider.Current.GetDouble() <= crossoverProbability)
                {
                    var children = crossover.Cross(selectedParents); 
                    offspring[i]= children;
                }
            });

            return offspring.OrderBy(pair=>pair.Key).Where(pair => pair.Value != null).SelectMany(pair=>pair.Value).ToList();
        }

        /// <summary>
        /// Mutate the specified chromosomes.
        /// </summary>
        /// <param name="mutation">The mutation class.</param>
        /// <param name="mutationProbability">The mutation probability.</param>
        /// <param name="chromosomes">The chromosomes.</param>
        public void Mutate(IMutation mutation, float mutationProbability, IList<IChromosome> chromosomes)
        {
            Parallel.ForEach(chromosomes, c =>
            {
                mutation.Mutate(c, mutationProbability);
            });
        }


        /// <summary>
        /// Crosses the specified parents.
        /// </summary>
        /// <param name="metaHeuristic">the current metaHeuristic being run</param>
        /// <param name="ctx">the current evolution context</param>
        /// <param name="crossover">The crossover class.</param>
        /// <param name="crossoverProbability">The crossover probability.</param>
        /// <param name="parents">The parents.</param>
        /// <returns>The result chromosomes.</returns>
        public IList<IChromosome> MetaCross(IMetaHeuristic metaHeuristic, IEvolutionContext ctx, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            var offspring = new ConcurrentDictionary<int, IList<IChromosome>>();

            Parallel.ForEach(Enumerable.Range(0, ctx.Population.MinSize / crossover.ParentsNumber).Select(i => i * crossover.ParentsNumber), i =>
            {
                var indContext = ctx.GetIndividual(i);
                var children = metaHeuristic.MatchParentsAndCross(indContext, crossover, crossoverProbability, parents);
                //if (children != null)
                //{
                //    foreach (var item in children)
                //        offspring.Add(item);
                //}
                offspring[i] = children;
            });

            return offspring.OrderBy(pair => pair.Key).Where(pair=>pair.Value!=null).SelectMany(pair => pair.Value).ToList();
        }

        /// <summary>
        /// Mutate the specified chromosomes.
        /// </summary>
        /// <param name="metaHeuristic">the current metaHeuristic being run</param>
        /// <param name="ctx">the current evolution context</param>
        /// <param name="mutation">The mutation class.</param>
        /// <param name="mutationProbability">The mutation probability.</param>
        /// <param name="chromosomes">The chromosomes.</param>
        public void MetaMutate(IMetaHeuristic metaHeuristic, IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> chromosomes)
        {
            Parallel.ForEach(Enumerable.Range(0, chromosomes.Count), i =>
            {
                var indContext = ctx.GetIndividual(i);
                metaHeuristic.MutateChromosome(indContext, mutation, mutationProbability, chromosomes);
            });
        }

        
    }
}
