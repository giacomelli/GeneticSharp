using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticSharp.Infrastructure.Framework.Threading
{
    /// <summary>
    /// An ITaskExecutor's implementation that executes the tasks in a parallel fashion using TPL.
    /// </summary>
    public class TplTaskExecutor : ParallelTaskExecutor
    {
        private CancellationTokenSource m_cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:GeneticSharp.Infrastructure.Framework.Threading.TplTaskExecutor"/> class.
        /// </summary>
        public TplTaskExecutor() : base()
        {
        }

        /// <summary>
        /// Starts the tasks execution.
        /// </summary>
        /// <returns>If has reach the timeout or has been interrupted false, otherwise true.</returns>
        public override bool Start()
        {
            try
            {
                var startTime = DateTime.Now;
                m_cancellationTokenSource = new CancellationTokenSource();
                ParallelLoopResult result = new ParallelLoopResult();
                try
                {
                    result = Parallel.For(0, Tasks.Count, new ParallelOptions() { CancellationToken = m_cancellationTokenSource.Token }, (i, state) =>
                    {
                        // Check if any has called Break()
                        if (state.ShouldExitCurrentIteration && state.LowestBreakIteration < i)
                            return;

                        // Execute the target function (fitness)
                        Tasks[i]();

                        // If cancellation token was requested OR take more time expected on Timeout property, then stop the running.
                        if ((m_cancellationTokenSource.IsCancellationRequested && !state.ShouldExitCurrentIteration) || ((DateTime.Now - startTime) > Timeout && !state.ShouldExitCurrentIteration))
                            state.Break();
                    });
                }
                catch (OperationCanceledException)
                {
                }

                return result.IsCompleted;
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}