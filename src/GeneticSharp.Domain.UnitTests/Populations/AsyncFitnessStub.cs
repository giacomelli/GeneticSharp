using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticSharp.Domain.UnitTests
{
    public class AsyncFitnessStub : IFitness, IAsyncFitness
    {
        public AsyncFitnessStub()
        {
            ParallelSleep = 500;
        }

        public bool SupportsParallel { get; set; }
        public int ParallelSleep { get; set; }

        public double Evaluate(IChromosome chromosome)
        {
            throw new NotSupportedException("Use EvaluateAsync instead.");
        }

        public async Task<double> EvaluateAsync(IChromosome chromosome, CancellationToken cancellationToken)
        {
            if (SupportsParallel)
            {
                await Task.Delay(ParallelSleep, cancellationToken);
            }

            var genes = chromosome.GetGenes();
            double f = genes.Sum(g => (int)g.Value) / 20f;

            if (f > 1)
            {
                f = 0;
            }

            return f;
        }
    }
}
