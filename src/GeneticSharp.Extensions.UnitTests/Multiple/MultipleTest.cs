using System.Linq;
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
            var mutation = new UniformMutation(true);


            // Given enough generations, the Multiple Chromosome should start exhibiting convergence
            // we compare TSP /25 gen with 3*TSP / 500 gen

            TspChromosome simpleChromosome = new TspChromosome(numberOfCities).Initialized();
            // Removing default initial permutation to get random cities
            mutation.Mutate(simpleChromosome, 1.0f);


            IFitness simplefitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
            simpleChromosome.Fitness = simplefitness.Evaluate(simpleChromosome);


            var multiChromosome = new MultipleChromosome((i) => new TspChromosome(numberOfCities), 3);
            //MultiChromosome should create 3 TSP chromosomes and store them in the corresponding property
            Assert.AreEqual(((MultipleChromosome)multiChromosome).Chromosomes.Count, 3);
            var tempMultiFitness = ((MultipleChromosome)multiChromosome).Chromosomes.Sum(c => simplefitness.Evaluate(c));
            var fitness = new MultipleFitness(simplefitness);
            //Multi fitness should sum over the fitnesses of individual chromosomes
            Assert.AreEqual(tempMultiFitness, fitness.Evaluate(multiChromosome));
            var population = new Population(30, 30, multiChromosome);
            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(501)
            };
            ga.Start();
            var bestTSPChromosome = (TspChromosome)((MultipleChromosome)ga.Population.BestChromosome).Chromosomes
              .OrderByDescending(c => c.Fitness).First();

            Assert.Greater(bestTSPChromosome.Fitness, simpleChromosome.Fitness);
        }



    }
}
