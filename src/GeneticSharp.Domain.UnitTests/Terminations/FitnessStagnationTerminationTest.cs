using GeneticSharp.Domain.Terminations;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture]
    [Category("Terminations")]
    public class FitnessStagnationTerminationTest
    {
        [Test()]
        public void HasReached_NoStagnation_False()
        {
            var target = new FitnessStagnationTermination(3);
            var ga = Substitute.For<IGeneticAlgorithm>();

            ga.BestChromosome.Returns(
                new ChromosomeStub() { Fitness = 0.1 },
                new ChromosomeStub() { Fitness = 0.2 },
                new ChromosomeStub() { Fitness = 0.3 });

            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
        }

        [Test()]
        public void HasReached_StagnantButNotReachTheGenerationsNumber_False()
        {

            var target = new FitnessStagnationTermination(4);
            var ga = Substitute.For<IGeneticAlgorithm>();

            ga.BestChromosome.Returns(
                new ChromosomeStub() { Fitness = 0.1 },
                new ChromosomeStub() { Fitness = 0.1 },
                new ChromosomeStub() { Fitness = 0.1 });

            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
        }

        [Test()]
        public void HasReached_StagnantAndReachGenerationNumber_True()
        {
            var target = new FitnessStagnationTermination(3);
            var ga = Substitute.For<IGeneticAlgorithm>();

            ga.BestChromosome.Returns(new ChromosomeStub() { Fitness = 0.2 },
                                      new ChromosomeStub() { Fitness = 0.2 },
                                      new ChromosomeStub() { Fitness = 0.3 },
                                      new ChromosomeStub() { Fitness = 0.3 },
                                      new ChromosomeStub() { Fitness = 0.3 });

            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
            Assert.IsFalse(target.HasReached(ga));
            Assert.IsTrue(target.HasReached(ga));
        }
    }
}