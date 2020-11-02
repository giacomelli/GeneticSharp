using System.Linq;
using System.Threading;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Domain.UnitTests
{
    public class FitnessStub : IFitness
    {
        public FitnessStub()
        {
            ParallelSleep = 500;
        }

        #region IFitness implementation
        public bool SupportsParallel { get; set; }
        public int ParallelSleep { get; set; }
        public double Evaluate(IChromosome chromosome)
        {
            if (SupportsParallel)
            {
                Thread.Sleep(ParallelSleep);
            }

            var genes = chromosome.GetGenes();
            double f = genes.Sum(g => (int)g.Value) / 20f;

            if (f > 1)
            {
                f = 0;
            }

            return f;
        }
        #endregion
    }
}