using System;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Terminations
{
	/// <summary>
	/// Defines the interface for a termination condition.
	/// </summary>
	/// <remarks>
	/// <see href="http://en.wikipedia.org/wiki/Genetic_algorithm#Termination">Wikipedia</a> 
	/// </remarks>
	public interface ITermination
	{
		#region Methods
		/// <summary>
		/// Determines whether the specified generation reached the termination condition.
		/// </summary>
		/// <returns><c>true</c> if the generation reached the termination condition; otherwise, <c>false</c>.</returns>
		/// <param name="generation">The generation.</param>
		bool HasReached(Generation generation);
		#endregion
	}
}

