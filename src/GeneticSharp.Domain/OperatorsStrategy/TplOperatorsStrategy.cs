using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Metaheuristics;

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
        /// <param name="population">the population from which parents are selected</param>
        /// <param name="crossover">The crossover class.</param>
        /// <param name="crossoverProbability">The crossover probability.</param>
        /// <param name="parents">The parents.</param>
        /// <returns>The result chromosomes.</returns>
        public  IList<IChromosome> Cross(IMetaHeuristic metaHeuristic, IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            var offspring = new ConcurrentBag<IChromosome>();

            Parallel.ForEach(Enumerable.Range(0, population.MinSize / crossover.ParentsNumber).Select(i => i * crossover.ParentsNumber), i =>
            {
                var children = metaHeuristic.MatchParentsAndCross(population, crossover, crossoverProbability, parents, i);
                if (children != null)
                {
                    foreach (var item in children)
                        offspring.Add(item);
                }
            });

            return offspring.ToList();
        }

        /// <summary>
        /// Mutate the specified chromosomes.
        /// </summary>
        /// <param name="population">the population from which the offspring are mutated</param>
        /// <param name="mutation">The mutation class.</param>
        /// <param name="mutationProbability">The mutation probability.</param>
        /// <param name="chromosomes">The chromosomes.</param>
        public  void Mutate(IMetaHeuristic metaHeuristic, IPopulation population, IMutation mutation, float mutationProbability, IList<IChromosome> chromosomes)
        {
            Parallel.ForEach(Enumerable.Range(0, chromosomes.Count), i =>
            {
                metaHeuristic.MutateChromosome(population, mutation, mutationProbability, chromosomes, i);
            });
        }

        
    }
}
