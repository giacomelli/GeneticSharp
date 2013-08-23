using System;
using NUnit.Framework;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Populations;
using Rhino.Mocks;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;

namespace GeneticSharp.Domain.UnitTests.Reinsertions
{
	[TestFixture()]
	public class PureReinsertionTest
	{
		[Test()]
		public void SelectChromosomes_OffspringsSizeEqualsParentsSizeAndGreaterThanMinSizeAndLowerThanMaxSize_SelectOffsprings ()
		{
			var target = new PureReinsertion ();
			var chromosome = MockRepository.GenerateStub<ChromosomeBase> (1);

			var population = new Population (2, 6, chromosome);
			var offsprings = new List<IChromosome> () { 
				chromosome, chromosome, chromosome, chromosome
			};

			var parents = new List<IChromosome> () { 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2)
			};

			var selected = target.SelectChromosomes (population, offsprings, parents);
			Assert.AreEqual (4, selected.Count);
			Assert.AreEqual (1, selected [0].Length);
			Assert.AreEqual (1, selected [1].Length);
			Assert.AreEqual (1, selected [2].Length);
			Assert.AreEqual (1, selected [3].Length);
		}
	}
}