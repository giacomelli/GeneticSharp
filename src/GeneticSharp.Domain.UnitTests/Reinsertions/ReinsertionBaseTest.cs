using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Reinsertions
{
    [TestFixture()]
    [Category("Reinsertions")]
    public class ReinsertionBaseTest
    {
        [Test()]
        public void SelectChromosomes_PopulationNull_Exception()
        {
            var target = MockRepository.GeneratePartialMock<ReinsertionBase>(false, false);
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(2);
            var offspring = new List<IChromosome>() {
                chromosome, chromosome, chromosome, chromosome
            };

            var parents = new List<IChromosome>() {
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2)
            };


            ExceptionAssert.IsThrowing(new ArgumentNullException("population"), () =>
            {
                target.SelectChromosomes(null, offspring, parents);
            });
        }

        [Test()]
        public void SelectChromosomes_offspringNull_Exception()
        {
            var target = MockRepository.GeneratePartialMock<ReinsertionBase>(false, false);
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(2);
            var population = new Population(5, 6, chromosome);

            var parents = new List<IChromosome>() {
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2)
            };


            ExceptionAssert.IsThrowing(new ArgumentNullException("offspring"), () =>
            {
                target.SelectChromosomes(population, null, parents);
            });
        }

        [Test()]
        public void SelectChromosomes_ParentsNull_Exception()
        {
            var target = MockRepository.GeneratePartialMock<ReinsertionBase>(false, false);
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(2);
            var population = new Population(5, 6, chromosome);
            var offspring = new List<IChromosome>() {
                chromosome, chromosome, chromosome, chromosome
            };

            ExceptionAssert.IsThrowing(new ArgumentNullException("parents"), () =>
            {
                target.SelectChromosomes(population, offspring, null);
            });
        }

        [Test()]
        public void SelectChromosomes_CanExpandFalseWithoffspringSizeLowerThanMinSize_Exception()
        {
            var target = MockRepository.GeneratePartialMock<ReinsertionBase>(false, false);
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(2);
            var population = new Population(5, 6, chromosome);
            var offspring = new List<IChromosome>() {
                chromosome, chromosome, chromosome, chromosome
            };

            var parents = new List<IChromosome>() {
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2)
            };


            ExceptionAssert.IsThrowing(new ReinsertionException(target, "Cannot expand the number of chromosome in population. Try another reinsertion!"), () =>
            {
                target.SelectChromosomes(population, offspring, parents);
            });
        }

        [Test()]
        public void SelectChromosomes_CanCollapseFalseWithoffspringSizeGreaterThanMaxSize_Exception()
        {
            var target = MockRepository.GeneratePartialMock<ReinsertionBase>(false, false);
            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(2);
            var population = new Population(2, 3, chromosome);
            var offspring = new List<IChromosome>() {
                chromosome, chromosome, chromosome, chromosome
            };

            var parents = new List<IChromosome>() {
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2)
            };


            ExceptionAssert.IsThrowing(new ReinsertionException(target, "Cannot collapse the number of chromosome in population. Try another reinsertion!"), () =>
            {
                target.SelectChromosomes(population, offspring, parents);
            });
        }

        [Test()]
        public void SelectChromosomes_CanCollapseAndExpandFalseWithoffspringSizeBetweenMinOrMaxSize_ChromosomesSelected()
        {
            var target = MockRepository.GeneratePartialMock<ReinsertionBase>(false, false);

            var chromosome = MockRepository.GenerateStub<ChromosomeBase>(2);
            var population = new Population(2, 5, chromosome);
            var offspring = new List<IChromosome>() {
                chromosome, chromosome, chromosome, chromosome
            };

            var parents = new List<IChromosome>() {
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2),
                MockRepository.GenerateStub<ChromosomeBase> (2)
            };

            ExceptionAssert.IsThrowing(typeof(Rhino.Mocks.Exceptions.ExpectationViolationException), () =>
            {
                target.SelectChromosomes(population, offspring, parents);
            });
        }
    }
}

