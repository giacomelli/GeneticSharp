using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture]
    [Category("Crossovers")]
    public class CycleCrossoverTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Cross_ParentWithNoOrderedGenes_Exception()
        {
            var target = new CycleCrossover();

            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(10);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(8),
                new Gene(4),
                new Gene(7),
                new Gene(3),
                new Gene(6),
                new Gene(2),
                new Gene(5),
                new Gene(1),
                new Gene(9),
                new Gene(0)
            });
            chromosome1.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(10));

            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(10);
            chromosome2.ReplaceGenes(0, new Gene[]
            {
                new Gene(0),
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(5),
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8),
                new Gene(9),
            });
            chromosome2.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(10));

            ExceptionAssert.IsThrowing(new CrossoverException(target, "The Cycle Crossover (CX) can be only used with ordered chromosomes. The specified chromosome has repeated genes."), () =>
            {
                target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });
            });
        }

        [Test]
        public void Cross_ParentsWith10Genes_Cross()
        {
            var target = new CycleCrossover();

            // 8 4 7 3 6 2 5 1 9 0
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(10);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(8),
                new Gene(4),
                new Gene(7),
                new Gene(3),
                new Gene(6),
                new Gene(2),
                new Gene(5),
                new Gene(1),
                new Gene(9),
                new Gene(0)
            });
            chromosome1.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(10));

            // 0 1 2 3 4 5 6 7 8 9
            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(10);
            chromosome2.ReplaceGenes(0, new Gene[]
            {
                new Gene(0),
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8),
                new Gene(9),
            });
            chromosome2.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(10));

            // Child one: 8 1 2 3 4 5 6 7 9 0
            // Child two: 0 4 7 3 6 2 5 1 8 9
            var rnd = MockRepository.GenerateMock<IRandomization>();
            rnd.Expect(r => r.GetUniqueInts(2, 0, 10)).Return(new int[] { 7, 3 });
            RandomizationProvider.Current = rnd;

            IList<IChromosome> actual = null; ;

            actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(10, actual[0].Length);
            Assert.AreEqual(10, actual[1].Length);

            Assert.AreEqual(10, actual[0].GetGenes().Distinct().Count());
            Assert.AreEqual(10, actual[1].GetGenes().Distinct().Count());

            Assert.AreEqual(8, actual[0].GetGene(0).Value);
            Assert.AreEqual(1, actual[0].GetGene(1).Value);
            Assert.AreEqual(2, actual[0].GetGene(2).Value);
            Assert.AreEqual(3, actual[0].GetGene(3).Value);
            Assert.AreEqual(4, actual[0].GetGene(4).Value);
            Assert.AreEqual(5, actual[0].GetGene(5).Value);
            Assert.AreEqual(6, actual[0].GetGene(6).Value);
            Assert.AreEqual(7, actual[0].GetGene(7).Value);
            Assert.AreEqual(9, actual[0].GetGene(8).Value);
            Assert.AreEqual(0, actual[0].GetGene(9).Value);

            Assert.AreEqual(0, actual[1].GetGene(0).Value);
            Assert.AreEqual(4, actual[1].GetGene(1).Value);
            Assert.AreEqual(7, actual[1].GetGene(2).Value);
            Assert.AreEqual(3, actual[1].GetGene(3).Value);
            Assert.AreEqual(6, actual[1].GetGene(4).Value);
            Assert.AreEqual(2, actual[1].GetGene(5).Value);
            Assert.AreEqual(5, actual[1].GetGene(6).Value);
            Assert.AreEqual(1, actual[1].GetGene(7).Value);
            Assert.AreEqual(8, actual[1].GetGene(8).Value);
            Assert.AreEqual(9, actual[1].GetGene(9).Value);
        }

        [Test]
        public void Cross_ParentsWith5OneCycleGenes_Cross()
        {
            var target = new CycleCrossover();

            // 8 4 7 3 6
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(5);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(8),
                new Gene(4),
                new Gene(7),
                new Gene(3),
                new Gene(6)
            });
            chromosome1.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(5));

            // 4 3 6 7 8
            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(5);
            chromosome2.ReplaceGenes(0, new Gene[]
            {
                new Gene(4),
                new Gene(3),
                new Gene(6),
                new Gene(7),
                new Gene(8)
            });
            chromosome2.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(5));

            // Cycle 1 indexes: 0 1 3 2 4
            // Child one: 4 3 6 7 8  
            // Child two: 8 4 7 3 6
            var rnd = MockRepository.GenerateMock<IRandomization>();
            rnd.Expect(r => r.GetUniqueInts(2, 0, 10)).Return(new int[] { 7, 3 });
            RandomizationProvider.Current = rnd;

            IList<IChromosome> actual = null; ;

            actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(5, actual[0].Length);
            Assert.AreEqual(5, actual[1].Length);

            Assert.AreEqual(5, actual[0].GetGenes().Distinct().Count());
            Assert.AreEqual(5, actual[1].GetGenes().Distinct().Count());

            Assert.AreEqual(8, actual[0].GetGene(0).Value);
            Assert.AreEqual(4, actual[0].GetGene(1).Value);
            Assert.AreEqual(7, actual[0].GetGene(2).Value);
            Assert.AreEqual(3, actual[0].GetGene(3).Value);
            Assert.AreEqual(6, actual[0].GetGene(4).Value);


            Assert.AreEqual(4, actual[1].GetGene(0).Value);
            Assert.AreEqual(3, actual[1].GetGene(1).Value);
            Assert.AreEqual(6, actual[1].GetGene(2).Value);
            Assert.AreEqual(7, actual[1].GetGene(3).Value);
            Assert.AreEqual(8, actual[1].GetGene(4).Value);
        }

        [Test]
        public void Cross_ParentsWith5ThreeCyclesGenes_Cross()
        {
            var target = new CycleCrossover();

            // 8 4 6 7 3
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(5);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(8),
                new Gene(4),
                new Gene(6),
                new Gene(7),
                new Gene(3)
            });
            chromosome1.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(5));

            // 4 3 6 7 8
            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(5);
            chromosome2.ReplaceGenes(0, new Gene[]
            {
                new Gene(4),
                new Gene(3),
                new Gene(6),
                new Gene(7),
                new Gene(8)
            });
            chromosome2.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(5));

            // Cycle 1 indexes: 0 1 4 
            // Cycle 2 indexes: 2 
            // Cycle 3 indexes: 3            
            // Child one: 8 4 6 7 3    
            // Child two: 4 3 6 7 8
            var rnd = MockRepository.GenerateMock<IRandomization>();
            rnd.Expect(r => r.GetUniqueInts(2, 0, 10)).Return(new int[] { 7, 3 });
            RandomizationProvider.Current = rnd;

            IList<IChromosome> actual = null; ;

            actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(5, actual[0].Length);
            Assert.AreEqual(5, actual[1].Length);

            Assert.AreEqual(5, actual[0].GetGenes().Distinct().Count());
            Assert.AreEqual(5, actual[1].GetGenes().Distinct().Count());

            Assert.AreEqual(8, actual[0].GetGene(0).Value);
            Assert.AreEqual(4, actual[0].GetGene(1).Value);
            Assert.AreEqual(6, actual[0].GetGene(2).Value);
            Assert.AreEqual(7, actual[0].GetGene(3).Value);
            Assert.AreEqual(3, actual[0].GetGene(4).Value);


            Assert.AreEqual(4, actual[1].GetGene(0).Value);
            Assert.AreEqual(3, actual[1].GetGene(1).Value);
            Assert.AreEqual(6, actual[1].GetGene(2).Value);
            Assert.AreEqual(7, actual[1].GetGene(3).Value);
            Assert.AreEqual(8, actual[1].GetGene(4).Value);
        }
    }
}