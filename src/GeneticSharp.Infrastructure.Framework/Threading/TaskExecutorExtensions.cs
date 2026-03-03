using System;

namespace GeneticSharp
{
    /// <summary>
    /// Extension methods for <see cref="ITaskExecutor"/>.
    /// </summary>
    public static class TaskExecutorExtensions
    {
        /// <summary>
        /// Add the specified synchronous task to be executed.
        /// </summary>
        /// <param name="executor">The task executor.</param>
        /// <param name="task">The synchronous task.</param>
        public static void Add(this ITaskExecutor executor, Action task)
        {
            executor.Add(ct => { task(); return default; });
        }
    }
}
