using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Mutations
{
    [TestFixture]
    [Category("Mutations")]
    public class TworsMutationTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void Mutate_NoProbality_NoExchangeGenes()
        {
            var target = new TworsMutation();
            var chromosome = Substitute.For<ChromosomeBase>(4);
            chromosome.ReplaceGenes(0, new Gene[]
            {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
            });

            var rnd = Substitute.For<IRandomization>();
            rnd.GetDouble().Returns(0.1);
            RandomizationProvider.Current = rnd;

            target.Mutate(chromosome, 0);

            Assert.AreEqual(4, chromosome.Length);
            Assert.AreEqual(1, chromosome.GetGene(0).Value);
            Assert.AreEqual(2, chromosome.GetGene(1).Value);
            Assert.AreEqual(3, chromosome.GetGene(2).Value);
            Assert.AreEqual(4, chromosome.GetGene(3).Value);
        }

        [Test()]
        public void Mutate_ValidChromosome_ExchangeGenes()
        {
            var target = new TworsMutation();
            var chromosome = Substitute.For<ChromosomeBase>(4);
            chromosome.ReplaceGenes(0, new Gene[]
                                                     {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
            });

            var rnd = Substitute.For<IRandomization>();
            rnd.GetUniqueInts(2, 0, 4).Returns(new int[] { 0, 2 });
            RandomizationProvider.Current = rnd;

            target.Mutate(chromosome, 1);

            Assert.AreEqual(4, chromosome.Length);
            Assert.AreEqual(3, chromosome.GetGene(0).Value);
            Assert.AreEqual(2, chromosome.GetGene(1).Value);
            Assert.AreEqual(1, chromosome.GetGene(2).Value);
            Assert.AreEqual(4, chromosome.GetGene(3).Value);
        }
    }
}