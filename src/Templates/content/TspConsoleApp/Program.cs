using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;
using System;
using System.Linq;

namespace TspConsoleApp
{
    class Program
    {
        /// <summary>
        /// GeneticSharp TSP Console Application template.
        /// <see href="https://github.com/giacomelli/GeneticSharp"/>
        /// </summary>
        static void Main(string[] args)
        {
            var selection = new EliteSelection();
            var crossover = new OrderedCrossover();
            var mutation = new TworsMutation();

            var fitness = new TspFitness(20, 0, 1000, 0, 1000);
            var chromosome = new TspChromosome(fitness.Cities.Count);

            var population = new Population(50, 70, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new FitnessStagnationTermination(100)
            };
            ga.GenerationRan += (s, e) =>
            {
                Console.Clear();
                Console.WriteLine($"Generation: {ga.GenerationsNumber}");

                var c = ga.BestChromosome as TspChromosome;
                Console.WriteLine("Cities: {0:n0}", c.Length);
                Console.WriteLine("Distance: {0:n2}", c.Distance);

                var cities = c.GetGenes().Select(g => g.Value.ToString()).ToArray();
                Console.WriteLine("City tour: {0}", string.Join(", ", cities));
            };

            Console.WriteLine("GA running...");
            ga.Start();

            Console.WriteLine();
            Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");
            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
