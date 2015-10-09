using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture]
    [Category("Crossovers")]
    public class CutAndSpliceCrossoverTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Cross_ParentsWithSameLength_Cross()
        {
            var target = new CutAndSpliceCrossover();
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(4);
            chromosome1.ReplaceGenes(0, new Gene[]
            {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
            });
            chromosome1.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(4));

            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(4);
            chromosome2.ReplaceGenes(0, new Gene[]
            {
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8)
            });
            chromosome2.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(4));

            var repository = new MockRepository();
            var rnd = repository.StrictMock<IRandomization>();

            using (repository.Ordered())
            {
                rnd.Expect(r => r.GetInt(1, 4)).Return(1);
                rnd.Expect(r => r.GetInt(1, 4)).Return(3);
            }

            repository.ReplayAll();

            RandomizationProvider.Current = rnd;

            var actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(2, actual[0].Length);
            Assert.AreEqual(6, actual[1].Length);

            Assert.AreEqual(1, actual[0].GetGene(0).Value);
            Assert.AreEqual(2, actual[0].GetGene(1).Value);

            Assert.AreEqual(5, actual[1].GetGene(0).Value);
            Assert.AreEqual(6, actual[1].GetGene(1).Value);
            Assert.AreEqual(7, actual[1].GetGene(2).Value);
            Assert.AreEqual(8, actual[1].GetGene(3).Value);
            Assert.AreEqual(3, actual[1].GetGene(4).Value);
            Assert.AreEqual(4, actual[1].GetGene(5).Value);
        }

        [Test]
        public void Cross_ParentsWithDiffLength_Cross()
        {
            var target = new CutAndSpliceCrossover();
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(4);
            chromosome1.ReplaceGenes(0, new Gene[]
            {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
            });
            chromosome1.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(4));

            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(5);
            chromosome2.ReplaceGenes(0, new Gene[]
            {
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8),
                new Gene(9),
            });
            chromosome2.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(5));

            var repository = new MockRepository();
            var rnd = repository.StrictMock<IRandomization>();

            using (repository.Ordered())
            {
                rnd.Expect(r => r.GetInt(1, 4)).Return(2);
                rnd.Expect(r => r.GetInt(1, 5)).Return(2);
            }

            repository.ReplayAll();

            RandomizationProvider.Current = rnd;

            var actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(5, actual[0].Length);
            Assert.AreEqual(4, actual[1].Length);

            Assert.AreEqual(1, actual[0].GetGene(0).Value);
            Assert.AreEqual(2, actual[0].GetGene(1).Value);
            Assert.AreEqual(3, actual[0].GetGene(2).Value);
            Assert.AreEqual(8, actual[0].GetGene(3).Value);
            Assert.AreEqual(9, actual[0].GetGene(4).Value);

            Assert.AreEqual(5, actual[1].GetGene(0).Value);
            Assert.AreEqual(6, actual[1].GetGene(1).Value);
            Assert.AreEqual(7, actual[1].GetGene(2).Value);
            Assert.AreEqual(4, actual[1].GetGene(3).Value);
        }
    }
}