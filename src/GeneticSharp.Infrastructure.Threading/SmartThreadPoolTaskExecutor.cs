using System;
using Amib.Threading;
using GeneticSharp.Infrastructure.Framework.Threading;

namespace GeneticSharp.Infrastructure.Threading
{
    /// <summary>
    /// A ITaskExecutor's implemenation using SmartThreadPool to execute tasks in parallel.
    /// </summary>
    public class SmartThreadPoolTaskExecutor : TaskExecutorBase
    {
        #region Fields
        private SmartThreadPool m_threadPool;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Infrastructure.Threading.SmartThreadPoolTaskExecutor"/> class.
        /// </summary>
        public SmartThreadPoolTaskExecutor()
        {
            MinThreads = 2;
            MaxThreads = 2;
        }
        #endregion

        #region Properties
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
        #endregion

        #region Methods
        /// <summary>
        /// Starts the tasks execution.
        /// </summary>
        /// <returns>If has reach the timeout false, otherwise true.</returns>
        public override bool Start()
        {
            base.Start();
            m_threadPool = new SmartThreadPool();

            try
            {
                m_threadPool.MinThreads = MinThreads;
                m_threadPool.MaxThreads = MaxThreads;
                var workItemResults = new IWorkItemResult[Tasks.Count];

                for (int i = 0; i < Tasks.Count; i++)
                {
                    var t = Tasks[i];
                    workItemResults[i] = m_threadPool.QueueWorkItem(new WorkItemCallback(Run), t);
                }

                m_threadPool.Start();

                // Timeout was reach?
                if (!m_threadPool.WaitForIdle(Timeout.TotalMilliseconds > int.MaxValue ? int.MaxValue : Convert.ToInt32(Timeout.TotalMilliseconds)))
                {
                    if (m_threadPool.IsShuttingdown)
                    {
                        return true;
                    }
                    else
                    {
                        m_threadPool.Cancel(true);
                        return false;
                    }
                }

                foreach (var wi in workItemResults)
                {
                    Exception ex;
                    wi.GetResult(out ex);

                    if (ex != null)
                    {
                        throw ex;
                    }
                }

                return true;
            }
            finally
            {
                m_threadPool.Shutdown(true, 1000);
                m_threadPool.Dispose();
                IsRunning = false;
            }
        }

        private object Run(object state)
        {
            ((System.Action)state)();

            return true;
        }

        /// <summary>
        /// Stops the tasks execution.
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            if (m_threadPool != null && !m_threadPool.IsShuttingdown)
            {
                m_threadPool.Shutdown(true, Timeout);
            }

            IsRunning = false;
        }
        #endregion
    }
}