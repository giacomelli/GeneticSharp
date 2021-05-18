using System.Linq;
using System.Threading;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Domain.UnitTests
{
    public class FitnessStub : IFitness
    {
        public FitnessStub():this(5) { }

        public FitnessStub(int maxValue)
        {
            MaxValue = maxValue;

        }

        #region IFitness implementation
        public bool SupportsParallel { get; set; }
        public int ParallelSleep { get; set; } = 500;

        public int MaxValue { get; set; }

        public double Evaluate(IChromosome chromosome)
        {
            if (SupportsParallel)
            {
                Thread.Sleep(ParallelSleep);
            }

            var genes = chromosome.GetGenes();
            var f = genes.Sum(g => (int)g.Value) / (double) (chromosome.Length*MaxValue);

            if (f > 1)
            {
                f = 0;
            }

            return f;
        }
        #endregion
    }
    
}