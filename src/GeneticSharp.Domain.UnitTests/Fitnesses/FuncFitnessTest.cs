using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Fitnesses
{
    [TestFixture]
    public class FuncFitnessTest
    {
        [Test]
        public void Evaluate_Func_CallFunc()
        {
            var target = new FuncFitness((c) =>
            {
                return c.Fitness.Value + 1;
            });

            Assert.AreEqual(3, target.Evaluate(new ChromosomeStub(2d)));
        }
    }
}

