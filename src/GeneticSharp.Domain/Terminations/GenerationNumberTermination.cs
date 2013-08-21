using System;
using GeneticSharp.Domain.Populations;
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
		/// The defaul expected generation number is 100.
		/// </remarks>
		public GenerationNumberTermination () : this(100)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.GenerationNumberTermination"/> class.
		/// </summary>
        /// <param name="expectedGenerationNumber">The generation number to consider the termination has been reached.</param>
		public GenerationNumberTermination (int expectedGenerationNumber)
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
        /// Determines whether the specified generation reached the termination condition.
        /// </summary>
        /// <param name="generation">The generation.</param>
        /// <returns>
        /// true
        /// </returns>
        /// <c>false</c>
        protected override bool PerformHasReached(Generation generation)
        {
            return generation.Number >= ExpectedGenerationNumber;
        }
        #endregion
    }
}