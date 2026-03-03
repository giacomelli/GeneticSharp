using System;
using System.Threading;

namespace GeneticSharp
{
    /// <summary>
    /// An ITaskExecutor's implementation that executes the tasks in a linear fashion.
    /// </summary>
    public class LinearTaskExecutor : TaskExecutorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearTaskExecutor"/> class.
        /// </summary>
        public LinearTaskExecutor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearTaskExecutor"/> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public LinearTaskExecutor(CancellationToken cancellationToken)
            : base(cancellationToken)
        {
        }

        #region implemented abstract members of TaskExecutorBase
        /// <summary>
        /// Starts the tasks execution.
        /// </summary>
        /// <returns>If has reach the timeout false, otherwise true.</returns>
        public override bool Start()
        {
            var startTime = DateTime.Now;
            base.Start();

            // For each Tasks passed to excutor, 
            // run it one in linear way.
            for (int i = 0; i < Tasks.Count; i++)
            {
                // Check if a stop was requested.
                if (StopRequested)
                {
                    return true;
                }

                Tasks[i](CancellationToken).GetAwaiter().GetResult();

                // If take more time expected on Timeout property,
                // tehn stop thre running.
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