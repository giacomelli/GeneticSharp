using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.AutoConfig;
using GeneticSharp.Extensions.Tsp;
using NSubstitute;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.AutoConfig
{
    [TestFixture]
    [Category("Extensions")]
    class AutoConfigFitnessTest
    {
        [SetUp]
        public void InitializeTest()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test]
        public void Evaluate_StartOk_Fitness()
        {
            var chromosome = new AutoConfigChromosome();
            chromosome.InitializeGenes();
            var targetChromosome = new TspChromosome(10);
            targetChromosome.InitializeGenes();
            var targetFitness = new TspFitness(10, 0, 100, 0, 100);
            var target = new AutoConfigFitness(targetFitness, targetChromosome)
            {
                PopulationMinSize = 20, PopulationMaxSize = 20, Termination = new FitnessThresholdTermination(0.1f)
            };

            var actual = target.Evaluate(chromosome);
            Assert.AreNotEqual(0, actual);
        }

        [Test]
        public void Evaluate_StartFailed_ZeroFitness()
        {
            var chromosome = new AutoConfigChromosome();
            chromosome.InitializeGenes();
            var targetChromosome = Substitute.For<IChromosome>();
            targetChromosome.InitializeGenes();
            targetChromosome.CreateNew().Returns(x => throw new Exception("TEST"));

            var targetFitness = new TspFitness(10, 0, 100, 0, 100);
            var target = new AutoConfigFitness(targetFitness, targetChromosome);

            var actual = target.Evaluate(chromosome);
            Assert.AreEqual(0, actual);
        }
    }
}
