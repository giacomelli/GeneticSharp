using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;
using HelperSharp;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture]
    public class OnePointCrossoverTest
    {
        [Test]
        public void Cross_LessGenesThenSwapPoint_Exception()
        {
            var target = new OnePointCrossover(1);
            var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(2);
			var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(2);

			ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("parents", "The swap point index is 1, but there is only 2 genes. The swap should result at least one gene to each side."), () =>
            {
                target.Cross(new List<IChromosome>() {
                    chromosome1,
                    chromosome2
                });
            });
        }

        [Test]
        public void Cross_ParentsWithTwoGenes_Cross()
        {
            var target = new OnePointCrossover(0);
			var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(2);
            chromosome1.ReplaceGenes(0, new Gene[] 
            { 
                new Gene(1),
                new Gene(2)
            });
			chromosome1.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(2));

            var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(2);            		
			chromosome2.ReplaceGenes(0, new Gene[] 
			{ 
				new Gene(3),
				new Gene(4)
			});
			chromosome2.Expect(c => c.CreateNew()).Return(MockRepository.GenerateStub<ChromosomeBase>(2));

            var actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(2, actual[0].Length);
            Assert.AreEqual(2, actual[1].Length);

            Assert.AreEqual(1, actual[0].GetGene(0).Value);
            Assert.AreEqual(4, actual[0].GetGene(1).Value);

            Assert.AreEqual(3, actual[1].GetGene(0).Value);
            Assert.AreEqual(2, actual[1].GetGene(1).Value);
        }
    }
}