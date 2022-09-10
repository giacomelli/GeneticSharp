using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using GeneticSharp.Extensions;

namespace GeneticSharp.Domain.UnitTests.Selections
{
    [TestFixture()]
    [Category("Selections")]
    public class SelectionIssuesTest
    {
        [SetUp]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }      

        /// <summary>
        /// https://github.com/giacomelli/GeneticSharp/issues/92
        /// </summary>
        [Test]
        [TestCase("Rank")]
        [TestCase("Roulette Wheel")]
        [TestCase("Tournament")]
        public void SelectChromosomes_Issue92_Solved(string selectionName)
        {
            var target = SelectionService.CreateSelectionByName(selectionName);
            var c1 = new SelectionStubChromosome();
            c1.Fitness = 1;

            var c2 = new SelectionStubChromosome();
            var c3 = new SelectionStubChromosome();
            var c4 = new SelectionStubChromosome();

            var generation = new Generation(1, new List<IChromosome>() {
                c1, c2, c3, c4
            });

            var actual = target.SelectChromosomes(10, generation);
            Assert.AreEqual(10, actual.Count);

            var previousChromosomes = actual.Select(c => c.GetGenes().ToArray()).ToArray();
            var mutation = new UniformMutation(true);


            for (int i = 0; i < actual.Count; i++)
            {
                if (actual[i].Fitness == 1)
                {
                    mutation.Mutate(actual[i], 1);

                    Assert.AreEqual(1, actual.Count(c => c.GetGene(0).Value != null), "Mutation has changed more than one chromosome at the time");
                    break;
                }
            }

            for (int i = 0; i < actual.Count; i++)
            {
                for (int j = 0; j < actual.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (object.ReferenceEquals(actual[i], actual[j]))
                        Assert.Fail($"Chromosomes on index {i} and {j} are the same.");
                }
            }
        }
    }
}

