using GeneticSharp.Domain.Terminations;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture()]
    [Category("Terminations")]
    public class GenerationNumberTerminationTest
    {
        [Test()]
        public void HasReached_GenerationNumberLowerThanCondition_False()
        {
            var ga = Substitute.For<IGeneticAlgorithm>();

            ga.GenerationsNumber.Returns(1, 2, 3, 4, 5, 6, 7, 8, 0);

            var target = new GenerationNumberTermination(10);
            Assert.IsFalse(target.HasReached(ga));

            for (int i = 0; i < 8; i++)
            {
                Assert.IsFalse(target.HasReached(ga));
            }

        }

        [Test()]
        public void HasReached_GenerationNumberGreaterOrEqualThanCondition_True()
        {
            var ga = Substitute.For<IGeneticAlgorithm>();

            ga.GenerationsNumber.Returns(10, 11);

            var target = new GenerationNumberTermination(10);
            Assert.IsTrue(target.HasReached(ga));
            Assert.IsTrue(target.HasReached(ga));
        }
    }
}

