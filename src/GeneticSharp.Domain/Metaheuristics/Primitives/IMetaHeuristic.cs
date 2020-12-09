using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// Defines an interface for parent matching and crossover application metaheuristics
    /// </summary>
    public interface IMetaHeuristic
    {


        Guid Guid { get; set; }


        /// <summary>
        /// From current generation, extract the population from which parent will be selected for breeding offsprings
        /// </summary>
        /// <param name="ctx">the generation context</param>
        /// <param name="selection">the selection operator to apply</param>
        /// <returns></returns>
        IList<IChromosome> SelectParentPopulation(IEvolutionContext ctx, ISelection selection);


        /// <summary>
        /// form parent matches and either executes the corresponding crossover with given probability to produce new children, or returns null children
        /// </summary>
        /// <param name="ctx">the generation context</param>
        /// <param name="crossover">The crossover class.</param>
        /// <param name="crossoverProbability">The crossover probability.</param>
        /// <param name="parents">The parents.</param>
        /// <returns>children for the current crossover if it was performed, null otherwise</returns>
        IList<IChromosome> MatchParentsAndCross(IEvolutionContext ctx, ICrossover crossover,
            float crossoverProbability, IList<IChromosome> parents);

        /// <summary>
        /// Apply mutation with given probability to target offspring chromosome with parents selected from given population 
        /// </summary>
        /// <param name="ctx">the generation context</param>
        /// <param name="mutation">The mutation class.</param>
        /// <param name="mutationProbability">The mutation probability.</param>
        /// <param name="offSprings">The list of offspring chromosomes</param>
        void MutateChromosome(IEvolutionContext ctx, IMutation mutation, float mutationProbability, IList<IChromosome> offSprings);


        /// <summary>
        /// Reinsert the specified offspring and parents after evaluation in order to produce a new generation.
        /// </summary>
        /// <param name="ctx">the generation context</param>
        /// <param name="reinsertion">the reinsertion operator to apply</param>
        /// <param name="offspring">The offspring chromosomes.</param>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>
        /// The reinserted chromosomes.
        /// </returns>
        IList<IChromosome> Reinsert(IEvolutionContext ctx, IReinsertion reinsertion, IList<IChromosome> offspring, IList<IChromosome> parents);


        /// <summary>
        /// Creates an evolution context that will serve controlling the flow of dynamic operators and subheuristics with parameters passing to each other
        /// </summary>
        /// <param name="algorithm">the parent genetic algorithm (may be null in certain cases)</param>
        /// <param name="population">the population to evolve (must not be null)</param>
        /// <returns></returns>
        IEvolutionContext GetContext(IGeneticAlgorithm algorithm, IPopulation population);



        void RegisterParameters(IEvolutionContext ctx);

    }
}