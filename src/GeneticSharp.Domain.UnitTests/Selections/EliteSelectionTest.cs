using System;
using NUnit.Framework;
using TestSharp;
using GeneticSharp.Domain.Selections;
using Rhino.Mocks;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;
using GeneticSharp.Domain.Populations;
using System.Linq;

namespace GeneticSharp.Domain.UnitTests.Selections
{
	[TestFixture()]
	public class EliteSelectionTest
	{
		[Test()]
		public void SelectChromosomes_InvalidNumber_Exception ()
		{
			var target = new EliteSelection();

			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException ("number", "The number of selected chromosomes should be at least 2."), () => {
				target.SelectChromosomes (-1, null);
			});

			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException ("number", "The number of selected chromosomes should be at least 2."), () => {
				target.SelectChromosomes (0, null);
			});

			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException ("number", "The number of selected chromosomes should be at least 2."), () => {
				target.SelectChromosomes (1, null);
			});
		}

		[Test()]
		public void SelectChromosomes_NullGeneration_Exception ()
		{
			var target = new EliteSelection();

			ExceptionAssert.IsThrowing (new ArgumentNullException ("generation"), () => {
				target.SelectChromosomes (2, null);
			});
		}

		[Test()]
		public void SelectChromosomes_Generation_ChromosomesSelected ()
		{
			var target = new EliteSelection();
			var c1 = MockRepository.GeneratePartialMock<ChromosomeBase> (2);
			c1.Fitness = 0.1;

			var c2 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
			c2.Fitness = 0.5;

			var c3 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
			c3.Fitness = 0;

			var c4 = MockRepository.GeneratePartialMock<ChromosomeBase>(2);
			c4.Fitness = 0.7;

			var generation = new Generation (1, new List<IChromosome> () {
				c1, c2, c3, c4
			});


			var actual = target.SelectChromosomes(2, generation);
			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual (0.7, actual [0].Fitness);
			Assert.AreEqual (0.5, actual [1].Fitness);

			actual = target.SelectChromosomes(3, generation);
			Assert.AreEqual(3, actual.Count);
			Assert.AreEqual (0.7, actual [0].Fitness);
			Assert.AreEqual (0.5, actual [1].Fitness);
			Assert.AreEqual (0.1, actual [2].Fitness);
		}
	}
}

