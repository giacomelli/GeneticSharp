using System;
using GeneticSharp.Domain.Populations;
using System.ComponentModel;

namespace GeneticSharp.Domain.Terminations
{
	/// <summary>
	/// Time Evolving Termination.
	/// <remarks>
	/// The genetic algorithm will be terminate when the evolving exceed the max time specified.
	/// </remarks>
	/// </summary>
	[DisplayName("Time Evolving")]
	public class TimeEvolvingTermination : TerminationBase
	{
		#region Fields
		private DateTime m_terminationTime;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.TimeEvolvingTermination"/> class.
		/// </summary>
		/// <remarks>
		/// The default MaxTime is 1 minute.
		/// </remarks>
		public TimeEvolvingTermination () : this(TimeSpan.FromMinutes(1))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.TimeEvolvingTermination"/> class.
		/// </summary>
		/// <param name="maxTime">The execution time to consider the termination has been reached.</param>
		public TimeEvolvingTermination (TimeSpan maxTime)
		{
			MaxTime = maxTime;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the execution max time.
		/// </summary>
		/// <value>The max time.</value>
		public TimeSpan MaxTime { get; set; }
		#endregion

		#region implemented abstract members of TerminationBase
		/// <summary>
		/// Determines whether the specified generation reached the termination condition.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="generation">The generation.</param>
		protected override bool PerformHasReached (Generation generation)
		{
			if (generation.Number == 1) {
				m_terminationTime = generation.CreationDate.Add (MaxTime);
			} 

			return DateTime.Now > m_terminationTime;
		}
		#endregion
	}
}

