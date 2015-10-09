using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Extensions.Mathematic;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Mathematic
{
    [TestFixture()]
    [Category("Extensions")]
    public class FunctionBuilderFitnessTest
    {
        [Test()]
        public void Evaluate_InvalidFunction_WorstFitness()
        {
            var target = new FunctionBuilderFitness(
                new FunctionBuilderInput(new double[] { 1 }, 2),
                new FunctionBuilderInput(new double[] { 1 }, 3));

            var c = new FunctionBuilderChromosome(target.AvailableOperations, 2);
            c.ReplaceGene(0, new Gene("-"));
            c.ReplaceGene(1, new Gene(""));

            var actual = target.Evaluate(c);

            Assert.AreEqual(double.MinValue, actual);
        }

        [Test()]
        public void Evaluate_NoneResultsEquals_FitnessIsDiff()
        {
            var target = new FunctionBuilderFitness(
                new FunctionBuilderInput(new double[] { 1 }, 2),
                new FunctionBuilderInput(new double[] { 1 }, 3));

            var c = new FunctionBuilderChromosome(target.AvailableOperations, 2);
            c.ReplaceGene(0, new Gene("A"));
            c.ReplaceGene(1, new Gene(""));

            var actual = target.Evaluate(c);
            Assert.AreEqual(-3, actual);
        }

        [Test()]
        public void Evaluate_AllResultsEquals_MaxFitness()
        {
            var target = new FunctionBuilderFitness(
                new FunctionBuilderInput(new double[] { 1 }, 1),
                new FunctionBuilderInput(new double[] { 2 }, 2));

            var c = new FunctionBuilderChromosome(target.AvailableOperations, 2);
            c.ReplaceGene(0, new Gene("A"));
            c.ReplaceGene(1, new Gene(""));

            var actual = target.Evaluate(c);
            Assert.AreEqual(0, actual);
        }
    }
}