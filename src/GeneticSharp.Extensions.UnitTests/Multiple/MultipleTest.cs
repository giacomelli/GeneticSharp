using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Multiple;
using GeneticSharp.Extensions.Tsp;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Multiple
{


    [TestFixture]
    [Category("Extensions")]
    class MultipleTest
    {

        [Test()]
        public void Evolve_CompareToSingleChromosome_Evolved()
        {
            int numberOfCities = 30;
            var selection = new EliteSelection();
            var crossover = new UniformCrossover();
            var mutation = new TworsMutation();


            // Given enough generations, the Multiple Chromosome should start exhibiting convergence
            // we compare TSP /25 gen with 3*TSP / 500 gen

            IChromosome chromosome = new TspChromosome(numberOfCities);
            IFitness fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
            var population = new Population(30, 30, chromosome);
            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(26)
            };
            ga.Start();
            var simpleChromosomeDistance = ((TspChromosome)ga.Population.BestChromosome).Distance;

            chromosome = new MultipleChromosome((i) => new TspChromosome(numberOfCities), 3);
            //MultiChromosome should create 3 TSP chromosomes and store them in the corresponding property
            Assert.AreEqual(((MultipleChromosome)chromosome).Chromosomes.Count, 3);
            var tempMultiFitness = ((MultipleChromosome)chromosome).Chromosomes.Sum(c => fitness.Evaluate(c));
            fitness = new MultipleFitness(fitness);
            //Multi fitness should sum over the fitnesses of individual chromosomes
            Assert.AreEqual(tempMultiFitness, fitness.Evaluate(chromosome));
            population = new Population(30, 30, chromosome);
            ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(501)
            };
            ga.Start();
            var bestTSPChromosome = (TspChromosome)((MultipleChromosome)ga.Population.BestChromosome).Chromosomes
              .OrderByDescending(c => c.Fitness).First();
            var multiChromosomesDistance = bestTSPChromosome.Distance;

            Assert.Less(multiChromosomesDistance, simpleChromosomeDistance);
        }



    }
}
