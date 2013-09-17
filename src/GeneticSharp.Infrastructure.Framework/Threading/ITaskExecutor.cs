using System;

namespace GeneticSharp.Infrastructure.Framework.Threading
{
	/// <summary>
	/// Defines a interface to a task executor.
	/// </summary>
	public interface ITaskExecutor
	{
		/// <summary>
		/// Add the specified task to be executed.
		/// </summary>
		/// <param name="task">The task.</param>
		void Add(Action task);

		/// <summary>
		/// Clear all the tasks.
		/// </summary>
		void Clear();

		/// <summary>
		/// Starts the tasks execution.
		/// </summary>
        /// <returns>If has reach the timeout false, otherwise true.</returns>
		bool Start();

		/// <summary>
		/// Stops the tasks execution.
		/// </summary>
		void Stop();

		/// <summary>
		/// Gets or sets the timeout to execute the tasks.
		/// </summary>
		TimeSpan Timeout { get; set; }
	}
}