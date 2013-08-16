using System;
using NUnit.Framework;
using GeneticSharp.Domain.Mutations;
using Rhino.Mocks;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;
using GeneticSharp.Domain.Randomizations;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Mutations
{
	[TestFixture()]
	public class UniformMutationTest
	{

		[Test()]
		public void Mutate_NoIndexes_RandomOneIndex ()
		{
			var target = new UniformMutation ();
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(3);
			chromosome.ReplaceGenes(0, new Gene[] 
			{ 
				new Gene() { Value = 1 },
				new Gene() { Value = 1 },
				new Gene() { Value = 1 } 
			});

			chromosome.Expect (c => c.GenerateGene(1)).Return (new Gene() { Value = 0 });
			RandomizationProvider.Current = MockRepository.GenerateMock<IRandomization> ();
			RandomizationProvider.Current.Expect (r => r.GetInts (1, 0, 3)).Return(new int[] {1});

			target.Mutate (chromosome, 1);
			Assert.AreEqual (1, chromosome.GetGene(0).Value);
			Assert.AreEqual (0, chromosome.GetGene(1).Value);
			Assert.AreEqual (1, chromosome.GetGene(2).Value);

			chromosome.VerifyAllExpectations ();
			RandomizationProvider.Current.VerifyAllExpectations ();
		}

		[Test()]
		public void Mutate_InvalidIndexes_Exception ()
		{
			var target = new UniformMutation (0, 3);
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(3);
			chromosome.ReplaceGenes(0, new Gene[] 
			                                         { 
				new Gene() { Value = 1 },
				new Gene() { Value = 1 },
				new Gene() { Value = 1 } 
			});

            chromosome.Expect(c => c.GenerateGene(0)).Return(new Gene() { Value = 0 });

			RandomizationProvider.Current = MockRepository.GenerateMock<IRandomization> ();

			ExceptionAssert.IsThrowing (new MutationException (target, "The chromosome has no gene on index 3. The chromosome genes lenght is 3."), () => {
				target.Mutate (chromosome, 1);
			});

			chromosome.VerifyAllExpectations ();
			RandomizationProvider.Current.VerifyAllExpectations ();
		}

		[Test()]
		public void Mutate_Indexes_RandomIndexes ()
		{
			var target = new UniformMutation (0, 2);
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(3);
			chromosome.ReplaceGenes(0, new Gene[] 
			                                         { 
				new Gene() { Value = 1 },
				new Gene() { Value = 1 },
				new Gene() { Value = 1 } 
			});

			chromosome.Expect (c => c.GenerateGene(0)).Return (new Gene() { Value = 0 });
			chromosome.Expect (c => c.GenerateGene(2)).Return (new Gene() { Value = 10 });
			RandomizationProvider.Current = MockRepository.GenerateMock<IRandomization> ();

			target.Mutate (chromosome, 1);
			Assert.AreEqual (0, chromosome.GetGene(0).Value);
			Assert.AreEqual (1, chromosome.GetGene(1).Value);
			Assert.AreEqual (10, chromosome.GetGene(2).Value);

			chromosome.VerifyAllExpectations ();
			RandomizationProvider.Current.VerifyAllExpectations ();
		}

		[Test()]
		public void Mutate_AllGenesMutablesTrue_AllGenesMutaed ()
		{
			var target = new UniformMutation (true);
			var chromosome = MockRepository.GenerateStub<ChromosomeBase>(3);
			chromosome.ReplaceGenes(0, new Gene[] 
			                     { 
				new Gene() { Value = 1 },
				new Gene() { Value = 1 },
				new Gene() { Value = 1 } 
			});

			chromosome.Expect (c => c.GenerateGene(0)).Return (new Gene() { Value = 0 });
			chromosome.Expect (c => c.GenerateGene(1)).Return (new Gene() { Value = 10 });
			chromosome.Expect (c => c.GenerateGene(2)).Return (new Gene() { Value = 20 });
			RandomizationProvider.Current = MockRepository.GenerateMock<IRandomization> ();

			target.Mutate (chromosome, 1);
			Assert.AreEqual (0, chromosome.GetGene(0).Value);
			Assert.AreEqual (10, chromosome.GetGene(1).Value);
			Assert.AreEqual (20, chromosome.GetGene(2).Value);

			chromosome.VerifyAllExpectations ();
			RandomizationProvider.Current.VerifyAllExpectations ();
		}
	}
}

