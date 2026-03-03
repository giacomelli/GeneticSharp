using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticSharp
{
    /// <summary>
    /// An IAsyncFitness implementation that defers the fitness evaluation to an async Func.
    /// </summary>
    public class AsyncFuncFitness : IFitness, IAsyncFitness
    {
        private readonly Func<IChromosome, CancellationToken, Task<double>> m_func;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncFuncFitness"/> class.
        /// </summary>
        /// <param name="func">The async fitness evaluation Func.</param>
        public AsyncFuncFitness(Func<IChromosome, CancellationToken, Task<double>> func)
        {
            ExceptionHelper.ThrowIfNull("func", func);
            m_func = func;
        }

        /// <summary>
        /// Evaluate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">Chromosome.</param>
        public double Evaluate(IChromosome chromosome)
        {
            throw new NotSupportedException("Use EvaluateAsync instead.");
        }

        /// <summary>
        /// Evaluate the specified chromosome asynchronously.
        /// </summary>
        /// <param name="chromosome">Chromosome.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<double> EvaluateAsync(IChromosome chromosome, CancellationToken cancellationToken)
        {
            return m_func(chromosome, cancellationToken);
        }
    }
}
