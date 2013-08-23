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
	public class FitnessBasedReinsertionTest
	{

		[Test()]
		public void SelectChromosomes_OffspringsSizeGreaterThanMaxSize_SelectOffsprings ()
		{
			var target = new FitnessBasedReinsertion ();

			var population = new Population (2, 3, MockRepository.GenerateStub<ChromosomeBase> (1));
			var offsprings = new List<IChromosome> () { 
				MockRepository.GenerateStub<ChromosomeBase> (1), 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (3), 
				MockRepository.GenerateStub<ChromosomeBase> (4)
			};

			offsprings [0].Fitness = 0.2;
			offsprings [1].Fitness = 0.3;
			offsprings [2].Fitness = 0.5;
			offsprings [3].Fitness = 0.7;

			var parents = new List<IChromosome> () { 
				MockRepository.GenerateStub<ChromosomeBase> (5), 
				MockRepository.GenerateStub<ChromosomeBase> (6), 
				MockRepository.GenerateStub<ChromosomeBase> (7), 
				MockRepository.GenerateStub<ChromosomeBase> (8)
			};



			var selected = target.SelectChromosomes (population, offsprings, parents);
			Assert.AreEqual (3, selected.Count);
			Assert.AreEqual (4, selected [0].Length);
			Assert.AreEqual (3, selected [1].Length);
			Assert.AreEqual (2, selected [2].Length);
		}
	}
}

