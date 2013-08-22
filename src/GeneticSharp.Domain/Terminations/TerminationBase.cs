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
		/// Determines whether the specified generation reached the termination condition.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="generation">The generation.</param>
		public bool HasReached (Generation generation)
		{
			ExceptionHelper.ThrowIfNull ("generation", generation);

			return PerformHasReached (generation);
		}

		/// <summary>
		/// Determines whether the specified generation reached the termination condition.
		/// </summary>
		/// <returns>True if termination has been reached, otherwise false.</returns>
		/// <param name="generation">The generation.</param>
		protected abstract bool PerformHasReached (Generation generation);
		#endregion
	}
}

