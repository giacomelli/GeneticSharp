using System.ComponentModel;

namespace GeneticSharp.Domain.Terminations
{
    /// <summary>
    /// Generation number termination.
    /// <remarks>
    /// The genetic algorithm will be terminate when reach the expected generation number.
    /// </remarks>
    /// </summary>
    [DisplayName("Generation Number")]
    public class GenerationNumberTermination : TerminationBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.GenerationNumberTermination"/> class.
        /// </summary>
        /// <remarks>
        /// The default expected generation number is 100.
        /// </remarks>
        public GenerationNumberTermination() : this(100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.GenerationNumberTermination"/> class.
        /// </summary>
        /// <param name="expectedGenerationNumber">The generation number to consider the termination has been reached.</param>
        public GenerationNumberTermination(int expectedGenerationNumber)
        {
            ExpectedGenerationNumber = expectedGenerationNumber;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the expected generation number to consider that termination has been reached.
        /// </summary>
        /// <value>The generation number.</value>
        public int ExpectedGenerationNumber { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="geneticAlgorithm">The genetic algorithm.</param>
        protected override bool PerformHasReached(IGeneticAlgorithm geneticAlgorithm)
        {
            return geneticAlgorithm.GenerationsNumber >= ExpectedGenerationNumber;
        }
        #endregion
    }
}