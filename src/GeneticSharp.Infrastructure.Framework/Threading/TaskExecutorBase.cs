using System;
using System.Collections.Generic;

namespace GeneticSharp.Infrastructure.Framework.Threading
{
    /// <summary>
    /// Task executor base.
    /// </summary>
    public abstract class TaskExecutorBase : ITaskExecutor
    {
        #region Fields
        private object m_lock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Infrastructure.Framework.Threading.TaskExecutorBase"/> class.
        /// </summary>
        protected TaskExecutorBase()
        {
            Tasks = new List<Action>();
            Timeout = TimeSpan.MaxValue;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the timeout to execute the tasks.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        protected IList<Action> Tasks { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this
        /// <see cref="GeneticSharp.Infrastructure.Framework.Threading.TaskExecutorBase"/> stop requested.
        /// </summary>
        /// <value><c>true</c> if stop requested; otherwise, <c>false</c>.</value>
        protected bool StopRequested { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Add the specified task to be executed.
        /// </summary>
        /// <param name="task">The task.</param>
        public void Add(Action task)
        {
            Tasks.Add(task);
        }

        /// <summary>
        /// Clear all the tasks.
        /// </summary>
        public void Clear()
        {
            Tasks.Clear();
        }

        /// <summary>
        /// Starts the tasks execution.
        /// </summary>
        /// <returns>If has reach the timeout false, otherwise true.</returns>
        public virtual bool Start()
        {
            lock (m_lock)
            {
                StopRequested = false;
                IsRunning = true;
            }

            return true;
        }

        /// <summary>
        /// Stops the tasks execution.
        /// </summary>
        public virtual void Stop()
        {
            lock (m_lock)
            {
                StopRequested = true;
            }
        }
        #endregion
    }
}
