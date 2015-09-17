using System.ComponentModel;

namespace GeneticSharp.Domain.Populations
{
    /// <summary>
    /// An IGenerationStrategy's implementation that keeps all generations to further evaluation.
    /// <remarks>
    /// This strategy can be slow and can suffer of OutOfMemoryException when you have great population and a long term termination.
    /// </remarks>
    /// </summary>
     [DisplayName("Tracking")]
    public class TrackingGenerationStrategy : IGenerationStrategy
    {
        #region Methods
        /// <summary>
        /// Register that a new generation has been created.
        /// </summary>
        /// <param name="population">The population where the new generation has been created.</param>
        public void RegisterNewGeneration(Population population)
        {
            // Do nothing, because wants to keep all generations in the line.
        }
        #endregion
    }
}
