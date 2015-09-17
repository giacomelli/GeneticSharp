using System;
using System.Diagnostics.CodeAnalysis;

namespace GeneticSharp.Infrastructure.Framework.Threading
{
    /// <summary>
    /// Defines a interface to a task executor.
    /// </summary>
    public interface ITaskExecutor
    {
        #region Properties
        /// <summary>
        /// Gets or sets the timeout to execute the tasks.
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        bool IsRunning { get; }
        #endregion

        #region Methods
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
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "there is no better name")]
        void Stop();
        #endregion
    }
}