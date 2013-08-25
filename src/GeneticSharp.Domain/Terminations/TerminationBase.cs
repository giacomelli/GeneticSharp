using System;
using GeneticSharp.Domain.Populations;
using HelperSharp;

namespace GeneticSharp.Domain.Terminations
{
	/// <summary>
	/// Base class for ITerminations's implementations.
	/// </summary>
	public abstract class TerminationBase : ITermination
	{
		#region Methods
		/// <summary>
		/// Determines whether the specified geneticAlgorithm reached the termination condition.
		/// </summary>
		/// <returns>True if termination has been reached, otherwise false.</returns>
		/// <param name="geneticAlgorithm">The genetic algorithm.</param>
		public bool HasReached (IGeneticAlgorithm geneticAlgorithm)
		{
			ExceptionHelper.ThrowIfNull ("geneticAlgorithm", geneticAlgorithm);

			return PerformHasReached (geneticAlgorithm);
		}

		/// <summary>
		/// Determines whether the specified geneticAlgorithm reached the termination condition.
		/// </summary>
		/// <returns>True if termination has been reached, otherwise false.</returns>
		/// <param name="geneticAlgorithm">The genetic algorithm.</param>
		protected abstract bool PerformHasReached (IGeneticAlgorithm geneticAlgorithm);
		#endregion
	}
}

