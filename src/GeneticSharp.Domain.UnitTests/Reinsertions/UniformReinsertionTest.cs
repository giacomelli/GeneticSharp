using System;
using NUnit.Framework;
using GeneticSharp.Domain.Reinsertions;
using Rhino.Mocks;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using System.Collections.Generic;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.UnitTests.Reinsertions
{
	[TestFixture()]
	public class UniformReinsertionTest
	{
		[TearDown]
		public void Cleanup()
		{
			RandomizationProvider.Current = new BasicRandomization();
		}

		[Test()]
		public void SelectChromosomes_OffspringsSizeLowerThanMinSize_SelectOffsprings ()
		{
			var target = new UniformReinsertion ();
	
			var population = new Population (6, 8, MockRepository.GenerateStub<ChromosomeBase> (1));
			var offsprings = new List<IChromosome> () { 
				MockRepository.GenerateStub<ChromosomeBase> (1), 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (3), 
				MockRepository.GenerateStub<ChromosomeBase> (4)
			};

			var parents = new List<IChromosome> () { 
				MockRepository.GenerateStub<ChromosomeBase> (5), 
				MockRepository.GenerateStub<ChromosomeBase> (6), 
				MockRepository.GenerateStub<ChromosomeBase> (7), 
				MockRepository.GenerateStub<ChromosomeBase> (8)
			};

			var rnd = MockRepository.GenerateMock<IRandomization> ();
			rnd.Expect (r => r.GetInt (0, 4)).Return (1);
			rnd.Expect (r => r.GetInt (0, 5)).Return (3);
			RandomizationProvider.Current = rnd;

			var selected = target.SelectChromosomes (population, offsprings, parents);
			Assert.AreEqual (6, selected.Count);
			Assert.AreEqual (1, selected [0].Length);
			Assert.AreEqual (2, selected [1].Length);
			Assert.AreEqual (3, selected [2].Length);
			Assert.AreEqual (4, selected [3].Length);
			Assert.AreEqual (2, selected [4].Length);
			Assert.AreEqual (4, selected [5].Length);
		}
	}
}

