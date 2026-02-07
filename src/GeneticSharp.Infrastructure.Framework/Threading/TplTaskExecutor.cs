using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticSharp
{
    /// <summary>
    /// An ITaskExecutor's implementation that executes the tasks in a parallel fashion using Task Parallel Library (TPL).
    /// </summary>
    /// <see href="https://github.com/giacomelli/GeneticSharp/wiki/multithreading"/>
    public class TplTaskExecutor : ParallelTaskExecutor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TplTaskExecutor"/> class.
        /// </summary>
        public TplTaskExecutor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TplTaskExecutor"/> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public TplTaskExecutor(CancellationToken cancellationToken)
            : base(cancellationToken)
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
                CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
                var token = CancellationTokenSource.Token;

                try
                {
                    Parallel.ForEachAsync(
                        System.Linq.Enumerable.Range(0, Tasks.Count),
                        new ParallelOptions() { CancellationToken = token },
                        async (i, ct) =>
                        {
                            await Tasks[i](ct);

                            if ((DateTime.Now - startTime) > Timeout)
                                CancellationTokenSource.Cancel();
                        }).GetAwaiter().GetResult();
                }
                catch (OperationCanceledException)
                {
                    return false;
                }

                return true;
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}