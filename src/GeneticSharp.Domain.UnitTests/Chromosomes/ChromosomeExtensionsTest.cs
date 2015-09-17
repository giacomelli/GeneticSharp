using System;
using NUnit.Framework;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [TestFixture()]
    [Category("Chromosomes")]
    public class ChromosomeExtensionsTest
	{
        [Test()]
        public void AnyChromosomeHasRepeatedGene_NonRepeatedGene_False ()
        {
        	var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(3);
        	chromosome1.ReplaceGenes(0, new Gene[] 
        	                         { 
                new Gene(1),
                new Gene(2),
                new Gene(3)
        	});

        	var chromosomes = new List<IChromosome>() { chromosome1 };
        	Assert.IsFalse (chromosomes.AnyHasRepeatedGene ());

        	var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(3);
        	chromosome2.ReplaceGenes(0, new Gene[] 
        	                         { 
                new Gene(1),
                new Gene(2),
                new Gene(4)
        	});

        	chromosomes.Add (chromosome2);
        	Assert.IsFalse (chromosomes.AnyHasRepeatedGene ());
        }

        [Test()]
        public void AnyChromosomeHasRepeatedGene_RepeatedGene_True ()
        {
        	var chromosome1 = MockRepository.GenerateStub<ChromosomeBase>(3);
        	chromosome1.ReplaceGenes(0, new Gene[] 
        	                         { 
                new Gene(1),
                new Gene(2),
                new Gene(3)
        	});

        	var chromosome2 = MockRepository.GenerateStub<ChromosomeBase>(3);
        	chromosome1.ReplaceGenes(0, new Gene[] 
        	{ 
                new Gene(4),
                new Gene(5),
                new Gene(4)
        	});

        	var chromosomes = new List<IChromosome>() { chromosome1, chromosome2 };

        	Assert.IsTrue (chromosomes.AnyHasRepeatedGene ());
        }
	}
}