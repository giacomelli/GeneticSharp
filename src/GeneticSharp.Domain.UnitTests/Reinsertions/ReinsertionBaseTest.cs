using System;
using NUnit.Framework;
using Rhino.Mocks;
using GeneticSharp.Domain.Reinsertions;
using TestSharp;
using GeneticSharp.Domain.Populations;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.UnitTests.Reinsertions
{
	[TestFixture()]
	public class ReinsertionBaseTest
	{
		[Test()]
		public void SelectChromosomes_CanExpandFalseWithOffspringsSizeLowerThanMinSize_Exception ()
		{
			var target = MockRepository.GeneratePartialMock<ReinsertionBase>(false, false);
			var chromosome = MockRepository.GenerateStub<ChromosomeBase> (1);
			var population = new Population (5, 6, chromosome);
			var offsprings = new List<IChromosome> () { 
				chromosome, chromosome, chromosome, chromosome
			};

			var parents = new List<IChromosome> () { 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2)
			};


			ExceptionAssert.IsThrowing (new ReinsertionException (target, "Cannot expand the number of chromosome in population. Try another reinsertion!"), () => {
				target.SelectChromosomes(population, offsprings, parents);
			});
		}

		[Test()]
		public void SelectChromosomes_CanCollapseFalseWithOffspringsSizeGreaterThanMaxSize_Exception ()
		{
			var target = MockRepository.GeneratePartialMock<ReinsertionBase>(false, false);
			var chromosome = MockRepository.GenerateStub<ChromosomeBase> (1);
			var population = new Population (2, 3, chromosome);
			var offsprings = new List<IChromosome> () { 
				chromosome, chromosome, chromosome, chromosome
			};

			var parents = new List<IChromosome> () { 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2)
			};


			ExceptionAssert.IsThrowing (new ReinsertionException (target, "Cannot collapse the number of chromosome in population. Try another reinsertion!"), () => {
				target.SelectChromosomes(population, offsprings, parents);
			});
		}

		[Test()]
		public void SelectChromosomes_CanCollapseAndExpandFalseWithOffspringsSizeBetweenMinOrMaxSize_ChromosomesSelected ()
		{
			var target = MockRepository.GeneratePartialMock<ReinsertionBase>(false, false);
		
			var chromosome = MockRepository.GenerateStub<ChromosomeBase> (1);
			var population = new Population (2, 5, chromosome);
			var offsprings = new List<IChromosome> () { 
				chromosome, chromosome, chromosome, chromosome
			};

			var parents = new List<IChromosome> () { 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2), 
				MockRepository.GenerateStub<ChromosomeBase> (2)
			};

			ExceptionAssert.IsThrowing (typeof(Rhino.Mocks.Exceptions.ExpectationViolationException), () => {
				target.SelectChromosomes (population, offsprings, parents);
			});
		}
	}
}

