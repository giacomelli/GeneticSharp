using System;
using GeneticSharp.Domain.Populations;
using System.ComponentModel;

namespace GeneticSharp.Domain.Terminations
{
	/// <summary>
	/// Generation number termination.
	/// <remarks>
	/// The genetic algorithm will be terminate when the execution exceed the max time specified.
	/// </remarks>
	/// </summary>
	[DisplayName("Time")]
	public class TimeTermination : TerminationBase
	{
		#region Fields
		private DateTime m_terminationTime;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.TimeTermination"/> class.
		/// </summary>
		/// <remarks>
		/// The default MaxTime is 1 minute.
		/// </remarks>
		public TimeTermination () : this(TimeSpan.FromMinutes(1))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.TimeTermination"/> class.
		/// </summary>
		/// <param name="maxTime">The execution time to consider the termination has been reached.</param>
		public TimeTermination (TimeSpan maxTime)
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

