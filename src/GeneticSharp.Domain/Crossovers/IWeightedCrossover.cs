using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// This interface represent crossover that can accept geometric coefficients modulating parents weights to apply while creating offspring
    /// </summary>
    public interface IWeightedCrossover: ICrossover
    {
        /// <summary>
        /// Generate children chromosomes by crossing the parents according to given weights.
        /// </summary>
        /// <param name="parents">the parent chromosomes to cross</param>
        /// <param name="weights">the weight for each parent chromosome</param>
        /// <returns>the children of the crossover applied to the parents, given the parents weights</returns>
        IList<IChromosome> PerformWeightedCross(IList<IChromosome> parents, IList<double> weights);
    }
}