namespace GeneticSharp.Domain.Terminations
{
    /// <summary>
    /// Defines the interface for a termination condition.
    /// </summary>
    /// <remarks>
    /// <see href="http://en.wikipedia.org/wiki/Genetic_algorithm#Termination">Wikipedia</see> 
    /// </remarks>
    public interface ITermination
    {
        #region Methods
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="geneticAlgorithm">The genetic algorithm.</param>
        bool HasReached(IGeneticAlgorithm geneticAlgorithm);
        #endregion
    }
}
