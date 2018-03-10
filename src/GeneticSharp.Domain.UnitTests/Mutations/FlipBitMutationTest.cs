using NUnit.Framework;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Mutations;
using NSubstitute;
using GeneticSharp.Domain.Chromosomes;

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
			var chromosome =  Substitute.For<ChromosomeBase>(3);
			chromosome.ReplaceGenes(0, new Gene[]
				{
					new Gene(0),
					new Gene(0),
					new Gene(0)
				});

			Assert.Catch<MutationException>(() =>
			{
				target.Mutate(chromosome, 1);
            }, "Needs a binary chromosome that implements IBinaryChromosome.");
		}

		[Test()]
		public void Mutate_NoArgs_BitMutated()
		{
			RandomizationProvider.Current = Substitute.For<IRandomization>();
			RandomizationProvider.Current.GetInt(0, 3).Returns(1);

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
		}
	}
}

