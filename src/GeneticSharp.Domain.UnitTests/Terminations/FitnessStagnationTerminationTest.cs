using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture]
    public class FitnessStagnationTerminationTest
    {
        [Test()]
        public void HasReached_NoStagnation_False()
        {
            var target = new FitnessStagnationTermination(3);

            var generation1 = new Generation(1, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.2 } });
            generation1.End(2);

            Assert.IsFalse(target.HasReached(generation1));

            var generation2 = new Generation(2, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.2 } });
            generation2.End(2);

            Assert.IsFalse(target.HasReached(generation2));

            var generation3 = new Generation(3, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.3 } });
            generation3.End(2);

            Assert.IsFalse(target.HasReached(generation3)); 
        }

        [Test()]
        public void HasReached_StagnantButNotReachTheGenerationsNumber_False()
        {
            var target = new FitnessStagnationTermination(4);

            var generation1 = new Generation(1, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.2 } });
            generation1.End(2);

            Assert.IsFalse(target.HasReached(generation1));

            var generation2 = new Generation(2, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.2 } });
            generation2.End(2);

            Assert.IsFalse(target.HasReached(generation2));

            var generation3 = new Generation(3, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.2 } });
            generation3.End(2);

            Assert.IsFalse(target.HasReached(generation3));
        }

        [Test()]
        public void HasReached_StagnantAndReachGenerationNumber_True()
        {
            var target = new FitnessStagnationTermination(3);

            var generation1 = new Generation(1, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.2 } });
            generation1.End(2);

            Assert.IsFalse(target.HasReached(generation1));

            var generation2 = new Generation(2, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.2 } });
            generation2.End(2);

            Assert.IsFalse(target.HasReached(generation2));

            var generation3 = new Generation(3, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.3 } });
            generation3.End(2);

            Assert.IsFalse(target.HasReached(generation3));

            var generation4 = new Generation(4, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.3 } });
            generation4.End(2);

            Assert.IsFalse(target.HasReached(generation4));


            var generation5 = new Generation(5, new List<IChromosome>() { new ChromosomeStub() { Fitness = 0.1 }, new ChromosomeStub() { Fitness = 0.3 } });
            generation5.End(2);

            Assert.IsTrue(target.HasReached(generation5));
        }
    }
}