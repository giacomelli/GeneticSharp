using System;

namespace GeneticSharp.Domain.Randomizations
{
	/// <summary>
	/// The randomization provider use for all elements of current genetic algorithm execution.
	/// </summary>
	public static class RandomizationProvider
	{
		#region Constructors
		/// <summary>
		/// Initializes the <see cref="GeneticSharp.Domain.Randomizations.RandomizationProvider"/> class.
		/// </summary>
		static RandomizationProvider ()
		{
			Current = new BasicRandomization ();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the current IRandomization implementation.
		/// </summary>
		/// <value>The current.</value>
		public static IRandomization Current { get; set; }
		#endregion
	}
}