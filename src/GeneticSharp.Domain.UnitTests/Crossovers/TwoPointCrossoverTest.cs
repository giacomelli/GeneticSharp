using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture]
    public class TwoPointCrossoverTest
    {
        [Test]
        public void Cross_SwapPointTwoLowerOrEqualThanPointOne_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("swapPointTwoGeneIndex", "The the swap point two index should be greater than swap point one index."), () =>
            {
                new TwoPointCrossover(1, 0);
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("swapPointTwoGeneIndex", "The the swap point two index should be greater than swap point one index."), () =>
            {
                new TwoPointCrossover(1, 1);
            });
        }

        [Test]
        public void Cross_ChromosomeLengthLowerThan3_Exception()
        {
            var target = new TwoPointCrossover(0, 1);
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>();

            chromosome1.AddGenes(new List<Gene>() { new Gene(), new Gene() });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("parents", "A Two Point Crossover needs chromosomes with, at least, 3 genes."), () =>
            {
                target.Cross(new List<IChromosome>() {
                    chromosome1,
                    chromosome1
                });
            });
        }        

        [Test]
        public void Cross_LessGenesThenSecondSwapPoint_Exception()
        {
            var target = new TwoPointCrossover(1, 3);
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>();

            chromosome1.AddGenes(new List<Gene>() { new Gene(), new Gene(), new Gene() });
            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("parents", "The swap point two index is 3, but there is only 3 genes. The swap should result at least one gene to each sides."), () =>
            {
                target.Cross(new List<IChromosome>() {
                    chromosome1,
                    chromosome1
                });
            });

            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>();
            chromosome2.AddGenes(new List<Gene>() { new Gene(), new Gene(), new Gene(), new Gene() } );
            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("parents", "The swap point two index is 3, but there is only 4 genes. The swap should result at least one gene to each sides."), () =>
            {
                target.Cross(new List<IChromosome>() {
                    chromosome2,
                    chromosome2
                });
            });
        }

        [Test]
        public void Cross_ParentsWithTwoGenes_Cross()
        {
            var target = new TwoPointCrossover(0, 1);
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>();
            chromosome1.AddGenes(new List<Gene>() 
            { 
                new Gene() { Value = 1 },
                new Gene() { Value = 2 },
                new Gene() { Value = 3 },
                new Gene() { Value = 4 },
            });
            chromosome1.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>());

            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>();
            chromosome2.AddGenes(new List<Gene>() 
            { 
                new Gene() { Value = 5 },
                new Gene() { Value = 6 },
                new Gene() { Value = 7 },
                new Gene() { Value = 8 }
            });
            chromosome2.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>());

            var actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(4, actual[0].Length);
            Assert.AreEqual(4, actual[1].Length);

            Assert.AreEqual(1, actual[0].GetGene(0).Value);
            Assert.AreEqual(6, actual[0].GetGene(1).Value);
            Assert.AreEqual(3, actual[0].GetGene(2).Value);
            Assert.AreEqual(4, actual[0].GetGene(3).Value);

            Assert.AreEqual(5, actual[1].GetGene(0).Value);
            Assert.AreEqual(2, actual[1].GetGene(1).Value);
            Assert.AreEqual(7, actual[1].GetGene(2).Value);
            Assert.AreEqual(8, actual[1].GetGene(3).Value);
        }
    }
}