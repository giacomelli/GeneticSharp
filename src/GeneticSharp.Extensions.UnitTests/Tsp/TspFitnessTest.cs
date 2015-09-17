using GeneticSharp.Extensions.Tsp;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Tsp
{
    [TestFixture()]
    [Category("Extensions")]
    public class TspFitnessTest
    {
        [Test()]
        public void Evaluate_ChromosomeWithLowerCities_FitnessDividedByDiff()
        {
            var target = new TspFitness(10, 0, 10, 0, 10);
            var chromosome = new TspChromosome(9);

            var actual = target.Evaluate(chromosome);
            Assert.AreNotEqual(0, actual);
        }

        [Test()]
        public void Evaluate_FitnessLowerThanZero_Zero()
        {
            var target = new TspFitness(10, 0, 10000000, 0, 10000000);
            var chromosome = new TspChromosome(10);

            var actual = target.Evaluate(chromosome);
            Assert.AreEqual(0, actual);
        }
    }
}

