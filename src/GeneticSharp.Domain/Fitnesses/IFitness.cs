using System;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Fitnesses
{
    /// <summary>
    /// Defines an interface for fitness function.
    /// <remarks>
    /// A fitness function is a particular type of objective function that is used to summarise, as a single figure of merit, how close a given design solution is to achieving the set aims.
    /// <see href="http://en.wikipedia.org/wiki/Fitness_function"/>http://en.wikipedia.org/wiki/Fitness_function</see>
    /// </remarks>
    /// </summary>
	public interface IFitness
	{
        /// <summary>
        /// Gets if fitness evaluation of all chromosomes in an generation can be done in parallel.
        /// <remarks>
        /// Just return true when your evaluation is very expense and can be done in parallel, otherwise, not use parallel will be faster.
        /// </remarks>
        /// </summary>
		bool SupportsParallel { get; }

        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
		double Evaluate(IChromosome chromosome);
	}
}

