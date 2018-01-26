using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Texts;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Mutations
{
    [TestFixture()]
    [Category("Mutations")]
    public class PartialShuffleMutationTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void Mutate_LessThanThreeGenes_Exception()
        {
            var target = new PartialShuffleMutation();
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(2);
            chromosome.ReplaceGenes(0, new Gene[]
                                    {
                new Gene(1),
            });

            ExceptionAssert.IsThrowing(new MutationException(target, "A chromosome should have, at least, 3 genes. {0} has only 2 gene.".With(chromosome.GetType().Name)), () =>
            {
                target.Mutate(chromosome, 0);
            });
        }

        [Test()]
        public void Mutate_NoProbality_NoPartialShuffle()
        {
            var target = new PartialShuffleMutation();
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(4);
            chromosome.ReplaceGenes(0, new Gene[]
                                    {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
            });

            var rnd = MockRepository.GenerateMock<IRandomization>();
            rnd.Expect(r => r.GetDouble()).Return(0.1);
            RandomizationProvider.Current = rnd;

            target.Mutate(chromosome, 0);

            Assert.AreEqual(4, chromosome.Length);
            Assert.AreEqual(1, chromosome.GetGene(0).Value);
            Assert.AreEqual(2, chromosome.GetGene(1).Value);
            Assert.AreEqual(3, chromosome.GetGene(2).Value);
            Assert.AreEqual(4, chromosome.GetGene(3).Value);

            rnd.VerifyAllExpectations();
            chromosome.VerifyAllExpectations();
        }

        [Test()]
        public void Mutate_ValidChromosome_PartialShuffle()
        {
            var target = new PartialShuffleMutation();
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(6);
            chromosome.ReplaceGenes(0, new Gene[]
                                    {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(5),
                new Gene(6),
            });

            var rnd = MockRepository.GenerateMock<IRandomization>();
            rnd.Expect(r => r.GetUniqueInts(2, 0, 6)).Return(new int[] { 1, 4 });
            rnd.Expect(r => r.GetInt(0, 4)).Return(2);
            rnd.Expect(r => r.GetInt(0, 3)).Return(1);
            rnd.Expect(r => r.GetInt(0, 2)).Return(1);
            rnd.Expect(r => r.GetInt(0, 1)).Return(0);
            RandomizationProvider.Current = rnd;

            target.Mutate(chromosome, 1);

            Assert.AreEqual(6, chromosome.Length);
            Assert.AreEqual(1, chromosome.GetGene(0).Value);
            Assert.AreEqual(4, chromosome.GetGene(1).Value);
            Assert.AreEqual(3, chromosome.GetGene(2).Value);
            Assert.AreEqual(5, chromosome.GetGene(3).Value);
            Assert.AreEqual(2, chromosome.GetGene(4).Value);
            Assert.AreEqual(6, chromosome.GetGene(5).Value);

            rnd.VerifyAllExpectations();
            chromosome.VerifyAllExpectations();
        }
    }
}
