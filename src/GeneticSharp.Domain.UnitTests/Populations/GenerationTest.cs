using NUnit.Framework;
using System;
using GeneticSharp.Domain.Populations;
using TestSharp;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Populations
{
	[TestFixture()]
	public class GenerationTest
	{
		[Test()]
		public void Constructor_ZeroOrNegativeNumber_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException ("number", "Generation number -1 is invalid. Generation number should be positive and start in 1."), () => {
				new Generation (-1, null);
			});

			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException ("number", "Generation number 0 is invalid. Generation number should be positive and start in 1."), () => {
				new Generation (0, null);
			});
		}

		[Test()]
		public void Constructor_InvalidChromosomesQuantity_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException  ("chromosomes", "A generation should have at least 2 chromosomes."), () => {
				new Generation (1, null);
			});

			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException ("chromosomes", "A generation should have at least 2 chromosomes."), () => {
				new Generation (1, new List<IChromosome>());
			});

			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException ("chromosomes", "A generation should have at least 2 chromosomes."), () => {
				new Generation (1, new List<IChromosome>() { MockRepository.GenerateMock<IChromosome>() });
			});
		}

		[Test()]
		public void Constructor_OkArguments_Instanced ()
		{
			var target = new Generation (1, new List<IChromosome> () {
				MockRepository.GenerateMock<IChromosome>(),
				MockRepository.GenerateMock<IChromosome>() 
			});

			Assert.AreEqual (1, target.Number);
			Assert.AreEqual (2, target.Chromosomes.Count);
		}
	}
}

