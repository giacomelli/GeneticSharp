using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Reinsertions
{
    [TestFixture()]
    [Category("Reinsertions")]
    public class ReinsertionBaseTest
    {
        [Test()]
        public void SelectChromosomes_PopulationNull_Exception()
        {
            var target = Substitute.ForPartsOf<ReinsertionBase>(false, false);
            var chromosome = Substitute.For<ChromosomeBase>(2);
            var offspring = new List<IChromosome>() {
                chromosome, chromosome, chromosome, chromosome
            };

            var parents = new List<IChromosome>() {
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2)
            };


            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                target.SelectChromosomes(null, offspring, parents);
            });

            Assert.AreEqual(actual.ParamName, "population");
        }

        [Test()]
        public void SelectChromosomes_offspringNull_Exception()
        {
            var target = Substitute.ForPartsOf<ReinsertionBase>(false, false);
            var chromosome = Substitute.For<ChromosomeBase>(2);
            var population = new Population(5, 6, chromosome);

            var parents = new List<IChromosome>() {
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2)
            };


            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                target.SelectChromosomes(population, null, parents);
            });

            Assert.AreEqual(actual.ParamName, "offspring");
        }

        [Test()]
        public void SelectChromosomes_ParentsNull_Exception()
        {
            var target = Substitute.ForPartsOf<ReinsertionBase>(false, false);
            var chromosome = Substitute.For<ChromosomeBase>(2);
            var population = new Population(5, 6, chromosome);
            var offspring = new List<IChromosome>() {
                chromosome, chromosome, chromosome, chromosome
            };

            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                target.SelectChromosomes(population, offspring, null);
            });

            Assert.AreEqual(actual.ParamName, "parents");
        }

        [Test()]
        public void SelectChromosomes_CanExpandFalseWithoffspringSizeLowerThanMinSize_Exception()
        {
            var target = Substitute.ForPartsOf<ReinsertionBase>(false, false);
            var chromosome = Substitute.For<ChromosomeBase>(2);
            var population = new Population(5, 6, chromosome);
            var offspring = new List<IChromosome>() {
                chromosome, chromosome, chromosome, chromosome
            };

            var parents = new List<IChromosome>() {
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2)
            };


            Assert.Catch<ReinsertionException>(() =>
            {
                target.SelectChromosomes(population, offspring, parents);
            }, "Cannot expand the number of chromosome in population. Try another reinsertion!");
        }

        [Test()]
        public void SelectChromosomes_CanCollapseFalseWithoffspringSizeGreaterThanMaxSize_Exception()
        {
            var target = Substitute.ForPartsOf<ReinsertionBase>(false, false);
            var chromosome = Substitute.For<ChromosomeBase>(2);
            var population = new Population(2, 3, chromosome);
            var offspring = new List<IChromosome>() {
                chromosome, chromosome, chromosome, chromosome
            };

            var parents = new List<IChromosome>() {
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2)
            };


            Assert.Catch<ReinsertionException>(() =>
            {
                target.SelectChromosomes(population, offspring, parents);
            }, "Cannot collapse the number of chromosome in population. Try another reinsertion!");
        }

        [Test()]
        public void SelectChromosomes_CanCollapseAndExpandFalseWithoffspringSizeBetweenMinOrMaxSize_ChromosomesSelected()
        {
            var target = Substitute.ForPartsOf<ReinsertionBase>(false, false);

            var chromosome = Substitute.For<ChromosomeBase>(2);
            var population = new Population(2, 5, chromosome);
            var offspring = new List<IChromosome>() {
                chromosome, chromosome, chromosome, chromosome
            };

            var parents = new List<IChromosome>() {
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2),
                Substitute.For<ChromosomeBase> (2)
            };

           
            // Verify if no exception, so procted abstract method PerformSelectChromosomes was called.
            var actual = target.SelectChromosomes(population, offspring, parents);
            Assert.AreEqual(0, actual.Count);
        }
    }
}

