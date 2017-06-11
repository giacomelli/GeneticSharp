using System;
using NUnit.Framework;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Mutations;
using Rhino.Mocks;
using GeneticSharp.Domain.Chromosomes;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Mutations
{
	[TestFixture]
	public class FlipBitMutationTest
	{
		[TearDown]
		public void Cleanup()
		{
			RandomizationProvider.Current = new BasicRandomization();
		}

		[Test()]
		public void Mutate_NotBinaryChromosome_Exception()
		{
			var target = new FlipBitMutation();
			var chromosome =  MockRepository.GenerateStub<ChromosomeBase>(3);
			chromosome.ReplaceGenes(0, new Gene[]
				{
					new Gene(0),
					new Gene(0),
					new Gene(0)
				});

			ExceptionAssert.IsThrowing(new MutationException(
				target, 
				"Needs a binary chromosome that implements IBinaryChromosome."), () =>
				{
					target.Mutate(chromosome, 1);
				});
		}

		[Test()]
		public void Mutate_NoArgs_BitMutated()
		{
			RandomizationProvider.Current = MockRepository.GenerateMock<IRandomization>();
			RandomizationProvider.Current.Expect(r => r.GetInt(0, 3)).Return(1);

			var target = new FlipBitMutation();
			var chromosome = new BinaryChromosomeStub(3);
			chromosome.ReplaceGenes(0, new Gene[]
				{
					new Gene(0),
					new Gene(0),
					new Gene(0)
				});
					
			target.Mutate(chromosome, 1);
			Assert.AreEqual(0, chromosome.GetGene(0).Value);
			Assert.AreEqual(1, chromosome.GetGene(1).Value);
			Assert.AreEqual(0, chromosome.GetGene(2).Value);

			RandomizationProvider.Current.VerifyAllExpectations();
		}
	}
}

