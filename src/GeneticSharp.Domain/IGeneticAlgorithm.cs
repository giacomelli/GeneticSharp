using System;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain
{
    /// <summary>
    /// Defines a interface for a genetic algorithm.
    /// </summary>
    public interface IGeneticAlgorithm
    {
        #region Properties
        /// <summary>
        /// Gets the generations number.
        /// </summary>
        /// <value>The generations number.</value>
        int GenerationsNumber { get; }

        /// <summary>
        /// Gets the best chromosome.
        /// </summary>
        /// <value>The best chromosome.</value>
        IChromosome BestChromosome { get; }

        /// <summary>
        /// Gets the time evolving.
        /// </summary>
        /// <value>The time evolving.</value>
        TimeSpan TimeEvolving { get; }
        #endregion
    }
}
