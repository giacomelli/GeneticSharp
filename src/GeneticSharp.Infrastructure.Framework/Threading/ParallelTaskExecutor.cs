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
        /// Gets or sets the cancellation token source.
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>
        /// Starts the tasks execution.
        /// </summary>
        /// <returns>If has reach the timeout false, otherwise true.</returns>
        public override bool Start()
        {
            SetThreadPoolConfig(out int minWorker, out int minIOC, out int maxWorker, out int maxIOC);

            try
            {
                base.Start();
                CancellationTokenSource = new CancellationTokenSource();
                var parallelTasks = new Task[Tasks.Count];

                for (int i = 0; i < Tasks.Count; i++)
                {
                    parallelTasks[i] = Task.Run(Tasks[i], CancellationTokenSource.Token);
                }

                // Need to verify, because TimeSpan.MaxValue passed to Task.WaitAll throws a System.ArgumentOutOfRangeException.
                if (Timeout == TimeSpan.MaxValue)
                {
                    Task.WaitAll(parallelTasks);
                    return true;
                }

                return Task.WaitAll(parallelTasks, Timeout);
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
            CancellationTokenSource?.Cancel();
            IsRunning = false;
		}

        /// <summary>
        /// Configure the ThreadPool min and max threads number to the define on this instance properties.
        /// </summary>
        /// <param name="minWorker">Minimum worker.</param>
        /// <param name="minIOC">Minimum ioc.</param>
        /// <param name="maxWorker">Max worker.</param>
        /// <param name="maxIOC">Max ioc.</param>
        protected void SetThreadPoolConfig(out int minWorker, out int minIOC, out int maxWorker, out int maxIOC)
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
        protected static void ResetThreadPoolConfig(int minWorker, int minIOC, int maxWorker, int maxIOC)
        {
            ThreadPool.SetMinThreads(minWorker, minIOC);
            ThreadPool.SetMaxThreads(maxWorker, maxIOC);
        }
    }
}