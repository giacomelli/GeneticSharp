using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain
{
    /// <summary>
    /// Defines an operators base strategy to be inherited either with linear or parallel execution
    /// </summary>
    public abstract class OperatorsStrategyBase : IOperatorsStrategy
    {
        /// <summary>
        /// Crosses the specified parents.
        /// </summary>
        /// <param name="population">the population from which parents are selected</param>
        /// <param name="crossover">The crossover class.</param>
        /// <param name="crossoverProbability">The crossover probability.</param>
        /// <param name="parents">The parents.</param>
        /// <returns>The result chromosomes.</returns>
        public abstract IList<IChromosome> Cross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents);

        /// <summary>
        /// Mutate the specified chromosomes.
        /// </summary>
        /// <param name="mutation">The mutation class.</param>
        /// <param name="mutationProbability">The mutation probability.</param>
        /// <param name="chromosomes">The chromosomes.</param>
        public  abstract void Mutate(IMutation mutation, float mutationProbability, IList<IChromosome> chromosomes);


        /// <summary>
        /// form parent matches and either executes the corresponding crossover with given probability to produce new children, or returns null children
        /// </summary>
        /// <param name="population">the population from which parents are selected</param>
        /// <param name="crossover">The crossover class.</param>
        /// <param name="crossoverProbability">The crossover probability.</param>
        /// <param name="parents">The parents.</param>
        /// <param name="firstParentIndex">the index of the first parent selected for a crossover</param>
        /// <returns>children for the current crossover if it was performed, null otherwise</returns>
        protected IList<IChromosome> SelectParentsAndCross(IPopulation population, ICrossover crossover,
            float crossoverProbability, IList<IChromosome> parents, int firstParentIndex)
        {
            var selectedParents = parents.Skip(firstParentIndex).Take(crossover.ParentsNumber).ToList();

            // If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
            // Checks if the number of selected parents is equal which the crossover expect, because the in the end of the list we can
            // have some rest chromosomes.
            if (selectedParents.Count == crossover.ParentsNumber && RandomizationProvider.Current.GetDouble() <= crossoverProbability)
            {
                return crossover.Cross(selectedParents);
            }

            return null;
        }

    }
}