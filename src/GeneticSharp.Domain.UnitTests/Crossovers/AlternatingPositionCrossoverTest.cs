using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using NSubstitute;
using System;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Fitnesses;
using System.Linq;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;
using System.Diagnostics;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Domain.UnitTests.Crossovers.Issues;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture]
    [Category("Crossovers")]
    public class AlternatingPositionCrossoverTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Cross_ParentWithNoOrderedGenes_Exception()
        {
            var target = new AlternatingPositionCrossover();

            var chromosome1 = Substitute.For<ChromosomeBase>(8);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(8),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(6),
                new Gene(5),
                new Gene(7),
                new Gene(1)
            });

            // 3 7 5 1 6 8 2 4
            var chromosome2 = Substitute.For<ChromosomeBase>(8);
            chromosome2.ReplaceGenes(0, new Gene[] {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(5),
                new Gene(5),
                new Gene(6),
                new Gene(7)
            });

            Assert.Catch<CrossoverException>(() =>
            {
                target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });
            }, "The Alternating-position (AP) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
        }

        [Test]
        public void Cross_DocumentationSample_Child()
        {
            var target = new AlternatingPositionCrossover();

            // 1 2 3 4 5 6 7 8
            var chromosome1 = Substitute.For<ChromosomeBase>(8);
            chromosome1.ReplaceGenes(0, new Gene[] {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4),
                new Gene(5),
                new Gene(6),
                new Gene(7),
                new Gene(8)
            });

            var child1 = Substitute.For<ChromosomeBase>(8);
            chromosome1.CreateNew().Returns(child1);

            // 3 7 5 1 6 8 2 4
            var chromosome2 = Substitute.For<ChromosomeBase>(8);
            chromosome2.ReplaceGenes(0, new Gene[] {
                new Gene(3),
                new Gene(7),
                new Gene(5),
                new Gene(1),
                new Gene(6),
                new Gene(8),
                new Gene(2),
                new Gene(4)
            });
            var child2 = Substitute.For<ChromosomeBase>(8);
            chromosome2.CreateNew().Returns(child2);

            var actual = target.Cross(new List<IChromosome>() { chromosome1, chromosome2 });

            Assert.AreEqual(2, actual.Count);

            // 1 3 2 7 5 4 6 8
            var actualChild = actual[0];
            Assert.AreEqual(8, actualChild.Length);
           
            Assert.AreEqual(1, actualChild.GetGene(0).Value);
            Assert.AreEqual(3, actualChild.GetGene(1).Value);
            Assert.AreEqual(2, actualChild.GetGene(2).Value);
            Assert.AreEqual(7, actualChild.GetGene(3).Value);
            Assert.AreEqual(5, actualChild.GetGene(4).Value);
            Assert.AreEqual(4, actualChild.GetGene(5).Value);
            Assert.AreEqual(6, actualChild.GetGene(6).Value);
            Assert.AreEqual(8, actualChild.GetGene(7).Value);

            // 3 1 7 2 5 4 6 8
            actualChild = actual[1];
            Assert.AreEqual(8, actualChild.Length);
            Assert.AreEqual(3, actualChild.GetGene(0).Value);
            Assert.AreEqual(1, actualChild.GetGene(1).Value);
            Assert.AreEqual(7, actualChild.GetGene(2).Value);
            Assert.AreEqual(2, actualChild.GetGene(3).Value);
            Assert.AreEqual(5, actualChild.GetGene(4).Value);
            Assert.AreEqual(4, actualChild.GetGene(5).Value);
            Assert.AreEqual(6, actualChild.GetGene(6).Value);
            Assert.AreEqual(8, actualChild.GetGene(7).Value);
        }

        [Test]
        [Repeat(10)]
        public void Cross_TspChromosome_Child()
        {
            var target = new AlternatingPositionCrossover();
            var chromosome1 = new TspChromosome(100);
            var chromosome2 = new TspChromosome(100);
            var actual = target.Cross(new TspChromosome[] { chromosome1, chromosome2 });

            Assert.AreEqual(2, actual.Count);

            CollectionAssert.AllItemsAreUnique(chromosome1.GetGenes());
            CollectionAssert.AllItemsAreUnique(chromosome2.GetGenes());
        }


        [Test]
        public void GA_WithAlternatingPositionCrossover_Evolve()
        {
            var chromosome = new TspChromosome(50);
            var population = new Population(50, 50, chromosome)
            {
                GenerationStrategy = new TrackingGenerationStrategy()
            };
            var fitness = new TspFitness(chromosome.Length, 0, 1000, 0, 1000);
            var crossover = new AlternatingPositionCrossover();
            var ga = new GeneticAlgorithm(population, fitness, new EliteSelection(), crossover, new ReverseSequenceMutation())
            {
                Termination = new GenerationNumberTermination(100)
            };

            ga.Start();
          
            Assert.Less(
                population.Generations.First().BestChromosome.Fitness.Value,
                population.Generations.Last().BestChromosome.Fitness.Value);
        }

        /// <summary>
        /// https://github.com/giacomelli/GeneticSharp/issues/61
        /// </summary>
        [Test]
        public void GA_Issue61_Solved()
        {
            const Int32 FinalAns = 4567213;
            var chromosome = new Issue61.GuessNumberChromosome(FinalAns.ToString().Length);
            var population = new Population(1000, 5000, chromosome);
            var fitness = new Issue61.GuessNumberFitness(FinalAns);
            var selection = new EliteSelection();
            var crossover = new AlternatingPositionCrossover();
            var mutation = new ReverseSequenceMutation();
            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.MutationProbability = 0.2f;
            ga.CrossoverProbability = 0.75f;
            ga.Termination = new OrTermination(
                new FitnessThresholdTermination(1.0),
                new FitnessStagnationTermination(200),
                new GenerationNumberTermination(1000));
            ga.Population.GenerationStrategy = new TrackingGenerationStrategy();
            ga.Start();

            foreach(var gen in ga.Population.Generations)
            {
                foreach(var chromossome in gen.Chromosomes)
                {
                    // Asserts if AlternatingPositionCrossover generated only ordered chromossomes.
                    Assert.AreEqual(chromosome.Length, chromosome.GetGenes().Distinct().Count());
                }
            }
        }
    }
}