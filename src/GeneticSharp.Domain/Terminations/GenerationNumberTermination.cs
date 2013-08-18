using System;
using GeneticSharp.Domain.Populations;
using System.ComponentModel;

namespace GeneticSharp.Domain.Terminations
{
	/// <summary>
	/// Generation number termination.
	/// <remarks>
	/// The genetic algorithm will be terminate when a number of generation be reached.
	/// </remarks>
	/// </summary>
	[DisplayName("Generation Number")]
	public class GenerationNumberTermination : ITermination
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.GenerationNumberTermination"/> class.
		/// </summary>
		/// <remarks>
		/// The defaul generation number is 100.
		/// </remarks>
		public GenerationNumberTermination () : this(100)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.GenerationNumberTermination"/> class.
		/// </summary>
		/// <param name="generationNumber">The generation number to consider the termination has been reached.</param>
		public GenerationNumberTermination (int generationNumber)
		{
			GenerationNumber = generationNumber;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the generation number to consider that termination has been reached.
		/// </summary>
		/// <value>The generation number.</value>
		public int GenerationNumber { get; set; }
		#endregion


		#region ITermination implementation
		/// <summary>
		/// Determines whether the specified generation reached the termination condition.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="generation">The generation.</param>
		public bool HasReached (Generation generation)
		{
			return generation.Number >= GenerationNumber;
		}
		#endregion
	}
}