using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Reinsertions
{
    [TestFixture()]
    [Category("Reinsertions")]
    public class UniformReinsertionTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void SelectChromosomes_OffspringSizeEqualsZero_Exception()
        {
            var target = new UniformReinsertion();
            var population = new Population(6, 8, MockRepository.GenerateStub<ChromosomeBase>(2));
            var offspring = new List<IChromosome>();

            var parents = new List<IChromosome>() {
                MockRepository.GenerateStub<ChromosomeBase> (5),
                MockRepository.GenerateStub<ChromosomeBase> (6),
                MockRepository.GenerateStub<ChromosomeBase> (7),
                MockRepository.GenerateStub<ChromosomeBase> (8)
            };

            ExceptionAssert.IsThrowing(new ReinsertionException(target, "The minimum size of the offspring is 1."), () =>
            {
                target.SelectChromosomes(population, offspring, parents);
            });
        }

        [Test()]
        public void SelectChromosomes_offspringSizeLowerThanMinSize_Selectoffspring()
        {
            var target = new UniformReinsertion();

            var population = new Population(6, 8, MockRepository.GenerateStub<ChromosomeBase>(2));
            var offspring = new List<IChromosome>() {
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (3),
                MockRepository.GenerateStub<ChromosomeBase> (4)
            };

            var parents = new List<IChromosome>() {
                MockRepository.GenerateStub<ChromosomeBase> (5),
                MockRepository.GenerateStub<ChromosomeBase> (6),
                MockRepository.GenerateStub<ChromosomeBase> (7),
                MockRepository.GenerateStub<ChromosomeBase> (8)
            };

            var rnd = MockRepository.GenerateMock<IRandomization>();
            rnd.Expect(r => r.GetInt(0, 4)).Return(1);
            rnd.Expect(r => r.GetInt(0, 5)).Return(3);
            RandomizationProvider.Current = rnd;

            var selected = target.SelectChromosomes(population, offspring, parents);
            Assert.AreEqual(6, selected.Count);
            Assert.AreEqual(2, selected[0].Length);
            Assert.AreEqual(2, selected[1].Length);
            Assert.AreEqual(3, selected[2].Length);
            Assert.AreEqual(4, selected[3].Length);
            Assert.AreEqual(2, selected[4].Length);
            Assert.AreEqual(4, selected[5].Length);
        }
    }
}

