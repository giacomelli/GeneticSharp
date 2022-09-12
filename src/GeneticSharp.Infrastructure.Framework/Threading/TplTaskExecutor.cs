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
        /// Starts the tasks execution.
        /// </summary>
        /// <returns>If has reach the timeout or has been interrupted false, otherwise true.</returns>
        public override bool Start()
        {
            try
            {
                var startTime = DateTime.Now;
                CancellationTokenSource = new CancellationTokenSource();
                var result = new ParallelLoopResult();

                try
                {
                    result = Parallel.For(0, Tasks.Count, new ParallelOptions() { CancellationToken = CancellationTokenSource.Token }, (i, state) =>
                    {
                        // Execute the target function (fitness).
                        Tasks[i]();

                        // If cancellation token was requested OR take more time expected on Timeout property, 
                        // then stop the running.
                        if (CancellationTokenSource.IsCancellationRequested || (DateTime.Now - startTime) > Timeout)
                            state.Break();
                    });
                }
                catch (OperationCanceledException)
                {
                    // Mute cancellation exception.
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