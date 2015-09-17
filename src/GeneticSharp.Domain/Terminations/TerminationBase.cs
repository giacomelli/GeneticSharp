using HelperSharp;

namespace GeneticSharp.Domain.Terminations
{
    /// <summary>
    /// Base class for ITerminations implementations.
    /// </summary>
    public abstract class TerminationBase : ITermination
    {
        #region Fields
        private bool m_hasReached;
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="geneticAlgorithm">The genetic algorithm.</param>
        public bool HasReached(IGeneticAlgorithm geneticAlgorithm)
        {
            ExceptionHelper.ThrowIfNull("geneticAlgorithm", geneticAlgorithm);

            m_hasReached = PerformHasReached(geneticAlgorithm);

            return m_hasReached;
        }

        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="geneticAlgorithm">The genetic algorithm.</param>
        protected abstract bool PerformHasReached(IGeneticAlgorithm geneticAlgorithm);

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Domain.Terminations.LogicalOperatorTerminationBase"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Domain.Terminations.LogicalOperatorTerminationBase"/>.</returns>
        public override string ToString()
        {
            return "{0} (HasReached: {1})".With(GetType().Name, m_hasReached);
        }
        #endregion
    }
}
