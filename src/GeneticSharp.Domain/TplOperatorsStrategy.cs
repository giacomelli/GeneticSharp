using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticSharp.Domain {
    /// <summary>
    /// An IOperatorsStrategy's implmentation which use Task Parallel Library (TPL) for parallel execution.
    /// </summary>
    public class TplOperatorsStrategy : IOperatorsStrategy {
        /// <summary>
        /// Crosses the specified parents.
        /// </summary>
        /// <param name="crossover">The crossover class.</param>
        /// <param name="crossoverProbability">The crossover probability.</param>
        /// <param name="parents">The parents.</param>
        /// <returns>The result chromosomes.</returns>
        public IList<IChromosome> Cross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents) {
            var offspring = new ConcurrentBag<IChromosome>();

            Parallel.ForEach(Enumerable.Range(0, population.MinSize / crossover.ParentsNumber).Select(i => i * crossover.ParentsNumber), i => {
                var selectedParents = parents.Skip(i).Take(crossover.ParentsNumber).ToList();

                // If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
                // Checks if the number of selected parents is equal which the crossover expect, because the in the end of the list we can
                // have some rest chromosomes.
                if (selectedParents.Count == crossover.ParentsNumber && RandomizationProvider.Current.GetDouble() <= crossoverProbability) {
                    var children = crossover.Cross(selectedParents);
                    foreach (var child in children) {
                        child.Parents.AddRange(selectedParents);
                        offspring.Add(child);
                    }
                }
            });

            return offspring.ToList();
        }

        /// <summary>
        /// Mutate the specified chromosomes.
        /// </summary>
        /// <param name="mutation">The mutation class.</param>
        /// <param name="mutationProbability">The mutation probability.</param>
        /// <param name="chromosomes">The chromosomes.</param>
        /// <returns>The amount of mutations that occured</returns>
        public int Mutate(IMutation mutation, float mutationProbability, IList<IChromosome> chromosomes) {
            int mutations = 0;
            Parallel.ForEach(chromosomes, c => {
                int m = mutation.Mutate(c, mutationProbability);
                Interlocked.Add(ref mutations, m);
            });
            return mutations;
        }
    }
}
