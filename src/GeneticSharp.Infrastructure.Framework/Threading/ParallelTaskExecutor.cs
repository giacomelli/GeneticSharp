using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticSharp.Infrastructure.Framework.Threading
{
    /// <summary>
    /// An ITaskExecutor's implementation that executes the tasks in a parallel fashion.
    /// </summary>
    public class ParallelTaskExecutor : TaskExecutorBase
    {
        private CancellationTokenSource m_cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:GeneticSharp.Infrastructure.Framework.Threading.ParallelTaskExecutor"/> class.
        /// </summary>
        public ParallelTaskExecutor()
        {
            MinThreads = 200;
            MaxThreads = 200;
        }

        /// <summary>
        /// Gets or sets the minimum threads.
        /// </summary>
        /// <value>The minimum threads.</value>
        public int MinThreads { get; set; }

        /// <summary>
        /// Gets or sets the max threads.
        /// </summary>
        /// <value>The max threads.</value>
        public int MaxThreads { get; set; }

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
                        if (state.ShouldExitCurrentIteration)
                            if (state.LowestBreakIteration < i)
                                return;

                        Tasks[i].Invoke();

                        if (m_cancellationTokenSource.IsCancellationRequested)
                            if (!state.ShouldExitCurrentIteration)
                                state.Break();

                        // If take more time expected on Timeout property,
                        // then stop the running.
                        if ((DateTime.Now - startTime) > Timeout)
                            if (!state.ShouldExitCurrentIteration)
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

        /// <summary>
        /// Stops the tasks execution.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            m_cancellationTokenSource.Cancel();
            IsRunning = false;
        }

        /// <summary>
        /// Configure the ThreadPool min and max threads number to the define on this instance properties.
        /// </summary>
        /// <param name="minWorker">Minimum worker.</param>
        /// <param name="minIOC">Minimum ioc.</param>
        /// <param name="maxWorker">Max worker.</param>
        /// <param name="maxIOC">Max ioc.</param>
        private void SetThreadPoolConfig(out int minWorker, out int minIOC, out int maxWorker, out int maxIOC)
        {
            // Do not change values if the new values to min and max threads are lower than already configured on ThreadPool.
            ThreadPool.GetMinThreads(out minWorker, out minIOC);

            if (MinThreads > minWorker)
            {
                ThreadPool.SetMinThreads(MinThreads, minIOC);
            }

            ThreadPool.GetMaxThreads(out maxWorker, out maxIOC);

            if (MaxThreads > maxWorker)
            {
                ThreadPool.SetMaxThreads(MaxThreads, maxIOC);
            }
        }

        /// <summary>
        /// Rollback ThreadPool previous min and max threads configuration.
        /// </summary>
        /// <param name="minWorker">Minimum worker.</param>
        /// <param name="minIOC">Minimum ioc.</param>
        /// <param name="maxWorker">Max worker.</param>
        /// <param name="maxIOC">Max ioc.</param>
        private static void ResetThreadPoolConfig(int minWorker, int minIOC, int maxWorker, int maxIOC)
        {
            ThreadPool.SetMinThreads(minWorker, minIOC);
            ThreadPool.SetMaxThreads(maxWorker, maxIOC);
        }
    }
}