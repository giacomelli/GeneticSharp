using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;
using System;
using HelperSharp;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [TestFixture()]
    [Category("Chromosomes")]
    public class ChromosomeExtensionsTest
    {
        [Test()]
        public void AnyChromosomeHasRepeatedGene_NonRepeatedGene_False()
        {
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(3);
            chromosome1.ReplaceGenes(0, new Gene[]
                                     {
                new Gene(1),
                new Gene(2),
                new Gene(3)
            });

            var chromosomes = new List<IChromosome>() { chromosome1 };
            Assert.IsFalse(chromosomes.AnyHasRepeatedGene());

            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(3);
            chromosome2.ReplaceGenes(0, new Gene[]
                                     {
                new Gene(1),
                new Gene(2),
                new Gene(4)
            });

            chromosomes.Add(chromosome2);
            Assert.IsFalse(chromosomes.AnyHasRepeatedGene());
        }

        [Test()]
        public void AnyChromosomeHasRepeatedGene_RepeatedGene_True()
        {
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(3);
            chromosome1.ReplaceGenes(0, new Gene[]
                                     {
                new Gene(1),
                new Gene(2),
                new Gene(3)
            });

            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(3);
            chromosome1.ReplaceGenes(0, new Gene[]
            {
                new Gene(4),
                new Gene(5),
                new Gene(4)
            });

            var chromosomes = new List<IChromosome>() { chromosome1, chromosome2 };

            Assert.IsTrue(chromosomes.AnyHasRepeatedGene());
        }

		[Test]
		public void ValidateGenes_GenesWithNullValue_Exception()
		{
			var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(3);

			ExceptionAssert.IsThrowing (
				new InvalidOperationException ("The chromosome '{0}' is generating genes with null value.".With(chromosome1.GetType().Name)),
				() => {
					chromosome1.ValidateGenes();
				});
		}

		[Test]
		public void ValidateGenes_AllGenesWithValue_NoException()
		{
			var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(3);
			chromosome1.ReplaceGenes (0, new Gene[] { new Gene (1), new Gene (2), new Gene (3) });

			chromosome1.ValidateGenes();
		}

		[Test]
		public void ValidateGenes_ChromosomesWithGenesWithNullValue_Exception()
		{
			var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(3);
			chromosome1.ReplaceGenes (0, new Gene[] { new Gene (1), new Gene (2), new Gene (3) });
			var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(3);

			ExceptionAssert.IsThrowing (
				new InvalidOperationException ("The chromosome '{0}' is generating genes with null value.".With(chromosome2.GetType().Name)),
				() => {
					(new List<IChromosome>() { chromosome1, chromosome2 }).ValidateGenes();
				});
		}

		[Test]
		public void ValidateGenes_ChromosomesWithAllGenesWithValue_NoException()
		{
			var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(3);
			chromosome1.ReplaceGenes (0, new Gene[] { new Gene (1), new Gene (2), new Gene (3) });
			var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(3);
			chromosome2.ReplaceGenes (0, new Gene[] { new Gene (1), new Gene (2), new Gene (3) });

			(new List<IChromosome>() { chromosome1, chromosome2 }).ValidateGenes();
		}
    }
}