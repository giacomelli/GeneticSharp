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
    /// Defines an interface for parent matching and crossover application metaheuristics
    /// </summary>
    public interface IMetaHeuristic
    {
        /// <summary>
        /// From current generation, extract the population from which parent will be selected for breeding offsprings
        /// </summary>
        /// <param name="population">the population with current generation</param>
        /// <param name="selection">the selection operator to apply</param>
        /// <returns></returns>
        IList<IChromosome> SelectParentPopulation(IPopulation population, ISelection selection);


        /// <summary>
        /// form parent matches and either executes the corresponding crossover with given probability to produce new children, or returns null children
        /// </summary>
        /// <param name="population">the population from which parents are selected</param>
        /// <param name="crossover">The crossover class.</param>
        /// <param name="crossoverProbability">The crossover probability.</param>
        /// <param name="parents">The parents.</param>
        /// <param name="firstParentIndex">the index of the first parent selected for a crossover</param>
        /// <returns>children for the current crossover if it was performed, null otherwise</returns>
        IList<IChromosome> MatchParentsAndCross(IPopulation population, ICrossover crossover,
            float crossoverProbability, IList<IChromosome> parents, int firstParentIndex);

        /// <summary>
        /// Apply mutation with given probability to target offspring chromosome with parents selected from given population 
        /// </summary>
        /// <param name="population">the population from which the target offspring parents were selected</param>
        /// <param name="mutation">The mutation class.</param>
        /// <param name="mutationProbability">The mutation probability.</param>
        /// <param name="offSprings">The list of offspring chromosomes</param>
        /// <param name="offspringIndex">The target chromosome index to mutate</param>
        void MutateChromosome(IPopulation population, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings, int offspringIndex);


        /// <summary>
        /// Reinsert the specified offspring and parents after evaluation in order to produce a new generation.
        /// </summary>
        /// <param name="population">the population from which the parents were selected</param>
        /// <param name="reinsertion">the reinsertion operator to apply</param>
        /// <param name="offspring">The offspring chromosomes.</param>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>
        /// The reinserted chromosomes.
        /// </returns>
        IList<IChromosome> Reinsert(IPopulation population, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents);





    }
}