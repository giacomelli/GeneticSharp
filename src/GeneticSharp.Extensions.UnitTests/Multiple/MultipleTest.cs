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
    public void Evolve_ManyGenerations_Fast()
    {
      int numberOfCities = 50;
      var selection = new EliteSelection();
      var crossover = new UniformCrossover();
      var mutation = new TworsMutation();


      // Given enough generations, the Multiple Chromosome should start exhibiting convergence
      // we compare TSP /100 gen with 5*TSP / 1000 gen

      IChromosome chromosome = new TspChromosome(numberOfCities);
      IFitness fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
      var population = new Population(40, 40, chromosome);
      var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
      ga.Termination = new GenerationNumberTermination(101);
      ga.Start();
      var simpleChromosomeDistance = ((TspChromosome)ga.Population.BestChromosome).Distance;


      chromosome = new MultipleChromosome(()=> new  TspChromosome(numberOfCities), 5);
      fitness = new MultipleFitness(fitness);
      population = new Population(40, 40, chromosome);
      ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
      ga.Termination = new GenerationNumberTermination(1001);
      ga.Start();
      var  bestTSPChromosome = (TspChromosome)((MultipleChromosome) ga.Population.BestChromosome).Chromosomes
        .OrderByDescending(c => c.Fitness).First();
      var multiChromosomesDistance = bestTSPChromosome.Distance;

      Assert.Less(multiChromosomesDistance, simpleChromosomeDistance);
    }



  }
}
