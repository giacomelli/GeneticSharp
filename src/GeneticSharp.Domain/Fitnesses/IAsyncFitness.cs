using System.Threading;
using System.Threading.Tasks;

namespace GeneticSharp
{
    /// <summary>
    /// Defines an interface for asynchronous fitness function.
    /// </summary>
    public interface IAsyncFitness
    {
        /// <summary>
        /// Performs the evaluation against the specified chromosome asynchronously.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The fitness of the chromosome.</returns>
        Task<double> EvaluateAsync(IChromosome chromosome, CancellationToken cancellationToken);
    }
}
