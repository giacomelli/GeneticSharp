using System;

namespace GeneticSharp.Infrastructure.Framework.Threading
{
    /// <summary>
    /// An ITaskExecutor's implementation that executes the tasks in a linear fashion.
    /// </summary>
    public class LinearTaskExecutor : TaskExecutorBase
    {
        #region implemented abstract members of TaskExecutorBase
        /// <summary>
        /// Starts the tasks execution.
        /// </summary>
        /// <returns>If has reach the timeout false, otherwise true.</returns>
        public override bool Start()
        {
            var startTime = DateTime.Now;
            base.Start();

            foreach (var t in Tasks)
            {
                if (StopRequested)
                {
                    return true;
                }

                t();

                if ((DateTime.Now - startTime) > Timeout)
                {
                    return false;
                }
            }

            IsRunning = false;
            return true;
        }
        #endregion
    }
}