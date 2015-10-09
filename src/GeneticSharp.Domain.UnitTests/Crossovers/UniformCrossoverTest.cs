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
    public class UniformCrossoverTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Cross_ParentsWithTwoGenesProbabilityDiffPercents_DiffChildren()
        {
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
            var parents = new List<IChromosome>() { chromosome1, chromosome2 };

            var mock = new MockRepository();
            var rnd = mock.StrictMock<IRandomization>();

            using (mock.Ordered())
            {
                rnd.Expect(r => r.GetDouble()).Return(0);
                rnd.Expect(r => r.GetDouble()).Return(0.49);
                rnd.Expect(r => r.GetDouble()).Return(0.5);
                rnd.Expect(r => r.GetDouble()).Return(1);
            }

            RandomizationProvider.Current = rnd;

            // 50%
            var target = new UniformCrossover(0.5f);
            mock.ReplayAll();

            var actual = target.Cross(parents);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(4, actual[0].Length);
            Assert.AreEqual(4, actual[1].Length);

            Assert.AreEqual(1, actual[0].GetGene(0).Value);
            Assert.AreEqual(2, actual[0].GetGene(1).Value);
            Assert.AreEqual(7, actual[0].GetGene(2).Value);
            Assert.AreEqual(8, actual[0].GetGene(3).Value);

            Assert.AreEqual(5, actual[1].GetGene(0).Value);
            Assert.AreEqual(6, actual[1].GetGene(1).Value);
            Assert.AreEqual(3, actual[1].GetGene(2).Value);
            Assert.AreEqual(4, actual[1].GetGene(3).Value);


            // 70%
            mock = new MockRepository();
            rnd = mock.StrictMock<IRandomization>();

            using (mock.Ordered())
            {
                rnd.Expect(r => r.GetDouble()).Return(0);
                rnd.Expect(r => r.GetDouble()).Return(0.49);
                rnd.Expect(r => r.GetDouble()).Return(0.5);
                rnd.Expect(r => r.GetDouble()).Return(1);
            }

            RandomizationProvider.Current = rnd;
            mock.ReplayAll();

            target = new UniformCrossover(0.7f);
            actual = target.Cross(parents);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(4, actual[0].Length);
            Assert.AreEqual(4, actual[1].Length);

            Assert.AreEqual(1, actual[0].GetGene(0).Value);
            Assert.AreEqual(2, actual[0].GetGene(1).Value);
            Assert.AreEqual(3, actual[0].GetGene(2).Value);
            Assert.AreEqual(8, actual[0].GetGene(3).Value);

            Assert.AreEqual(5, actual[1].GetGene(0).Value);
            Assert.AreEqual(6, actual[1].GetGene(1).Value);
            Assert.AreEqual(7, actual[1].GetGene(2).Value);
            Assert.AreEqual(4, actual[1].GetGene(3).Value);
        }
    }
}