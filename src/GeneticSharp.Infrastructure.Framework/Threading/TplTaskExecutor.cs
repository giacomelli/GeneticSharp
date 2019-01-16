using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticSharp.Infrastructure.Framework.Threading
{
    /// <summary>
    /// An ITaskExecutor's implementation that executes the tasks in a parallel fashion.
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
        /// <returns>If has reach the timeout false, otherwise true.</returns>
        public override bool Start()
        {
            SetThreadPoolConfig(out int minWorker, out int minIOC, out int maxWorker, out int maxIOC);

            try
            {
                var startTime = DateTime.Now;
                base.Start();
                m_cancellationTokenSource = new CancellationTokenSource();
                ParallelLoopResult result = new ParallelLoopResult();
                try
                {
                    result = Parallel.For(0, Tasks.Count, new ParallelOptions() { CancellationToken = m_cancellationTokenSource.Token }, (i, state) =>
                    {
                        // Check if any has called Break()
                        if (state.ShouldExitCurrentIteration && state.LowestBreakIteration < i)
                            return;

                        Tasks[i].Invoke();

                        if (m_cancellationTokenSource.IsCancellationRequested)
                            if (!state.ShouldExitCurrentIteration)
                                state.Break();

                        // If take more time expected on Timeout property,
                        // then stop the running.
                        if ((DateTime.Now - startTime) > Timeout && !state.ShouldExitCurrentIteration)
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
                ResetThreadPoolConfig(minWorker, minIOC, maxWorker, maxIOC);
                IsRunning = false;
            }
        }
    }
}