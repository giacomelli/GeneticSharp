using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.OperatorsStrategies
{
    public class TplOperatorsStrategy : IOperatorsStrategy
    {
        public IList<IChromosome> Cross(ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            var offspring = new ConcurrentBag<IChromosome>();

            Parallel.ForEach(Enumerable.Range(0, parents.Count / crossover.ParentsNumber).Select(i => i * crossover.ParentsNumber), i =>
            {
                var selectedParents = parents.Skip(i).Take(crossover.ParentsNumber).ToList();

                // If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
                // Checks if the number of selected parents is equal which the crossover expect, because the in the end of the list we can
                // have some rest chromosomes.
                if (selectedParents.Count == crossover.ParentsNumber && RandomizationProvider.Current.GetDouble() <= crossoverProbability)
                {
                    var children = crossover.Cross(selectedParents);
                    foreach (var item in children)
                        offspring.Add(item);
                }
            });

            return offspring.ToList();
        }

        public void Mutate(IMutation mutation, float mutationProbability, IList<IChromosome> chromosomes)
        {
            Parallel.ForEach(chromosomes, c =>
            {
                mutation.Mutate(c, mutationProbability);
            });
        }
    }
}
