using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture()]
    [Category("Crossovers")]
    public class PartiallyMappedCrossoverTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Cross_ParentWithNoOrderedGenes_Exception()
        {
            var target = new PartiallyMappedCrossover();

            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(8);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8)
            });
            chromosome1.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(8));

            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(8);
            chromosome2.ReplaceGenes(0, new Gene[]
                                     {
                new Gene(3),
                new Gene(7),
                new Gene(5),
                new Gene(1),
                new Gene(1),
                new Gene(8),
                new Gene(2),
                new Gene(3)
            });
            chromosome2.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(8));

            ExceptionAssert.IsThrowing(new CrossoverException(target, "The Partially Mapped Crossover (PMX) can be only used with ordered chromosomes. The specified chromosome has repeated genes."), () =>
            {
                target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });
            });
        }

        [Test]
        public void Cross_ParentsWith8Genes_Cross()
        {
            var target = new PartiallyMappedCrossover();

            // 1 2 3 4 5 6 7 8 
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(8);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8)
            });
            chromosome1.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(8));

            // 3 7 5 1 6 8 2 4
            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(8);
            chromosome2.ReplaceGenes(0, new Gene[]
            {
                new Gene(3),
                new Gene(7),
                new Gene(5),
                new Gene(1),
                new Gene(6),
                new Gene(8),
                new Gene(2),
                new Gene(4)
            });
            chromosome2.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(8));

            var rnd = MockRepository.GenerateMock<IRandomization>();
            rnd.Expect(r => r.GetUniqueInts(2, 0, 8)).Return(new int[] { 3, 5 });
            RandomizationProvider.Current = rnd;

            IList<IChromosome> actual = null; ;

            TimeAssert.LessThan(30, () =>
            {
                actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });
            });

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(8, actual[0].Length);
            Assert.AreEqual(8, actual[1].Length);

            //Assert.AreEqual(8, actual[0].GetGenes().Distinct().Count());
            //Assert.AreEqual(8, actual[1].GetGenes().Distinct().Count());

            // offspring 1: (4 2 3 1 6 8 7 5)
            Assert.AreEqual(4, actual[0].GetGene(0).Value);
            Assert.AreEqual(2, actual[0].GetGene(1).Value);
            Assert.AreEqual(3, actual[0].GetGene(2).Value);
            Assert.AreEqual(1, actual[0].GetGene(3).Value);
            Assert.AreEqual(6, actual[0].GetGene(4).Value);
            Assert.AreEqual(8, actual[0].GetGene(5).Value);
            Assert.AreEqual(7, actual[0].GetGene(6).Value);
            Assert.AreEqual(5, actual[0].GetGene(7).Value);

            //        	// offspring 2: (3 7 8 4 5 6 2 1)
            Assert.AreEqual(3, actual[1].GetGene(0).Value);
            Assert.AreEqual(7, actual[1].GetGene(1).Value);
            Assert.AreEqual(8, actual[1].GetGene(2).Value);
            Assert.AreEqual(4, actual[1].GetGene(3).Value);
            Assert.AreEqual(5, actual[1].GetGene(4).Value);
            Assert.AreEqual(6, actual[1].GetGene(5).Value);
            Assert.AreEqual(2, actual[1].GetGene(6).Value);
            Assert.AreEqual(1, actual[1].GetGene(7).Value);
        }
    }
}

