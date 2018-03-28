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
            // Configure the ThreadPool min and max threads number to the define on this instance properties..
            ThreadPool.GetMinThreads(out int minWorker, out int minIOC);
            ThreadPool.SetMinThreads(MinThreads, minIOC);

            ThreadPool.GetMaxThreads(out int maxWorker, out int maxIOC);
            ThreadPool.SetMaxThreads(MaxThreads, maxIOC);

            try
            {
                base.Start();
                m_cancellationTokenSource = new CancellationTokenSource();
                var parallelTasks = new Task[Tasks.Count];

                for (int i = 0; i < Tasks.Count; i++)
                {
                    parallelTasks[i] = Task.Run(Tasks[i], m_cancellationTokenSource.Token);
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
                // Rollback ThreadPool previous min and max threads configuration.
                ThreadPool.SetMinThreads(minWorker, minIOC);
                ThreadPool.SetMaxThreads(maxWorker, maxIOC);

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
	}
}