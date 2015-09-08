using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

namespace Issue1Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(0);
            var mutation = new UniformMutation(true);
            var fitness = new Issue1Fitness();
            var chromosome = new Issue1Chromosome();
            var population = new Population(50, 50, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = new GenerationNumberTermination(100);

            Console.WriteLine("GA running...");
            ga.Start();
            Console.WriteLine("GA done in {0} generations.", ga.GenerationsNumber);

            var bestChromosome = ga.BestChromosome as Issue1Chromosome;
            Console.WriteLine("Best solution found is X:{0}, Y:{1} with {2} fitness.", bestChromosome.X, bestChromosome.Y, bestChromosome.Fitness);
		    Console.ReadKey();
        }
    }
}
