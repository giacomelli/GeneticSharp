using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain;

namespace GeneticSharp.Runner.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfCities = 20;
            var selection = new EliteSelection();
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();
            var chromosome = new TspChromosome(numberOfCities);
            var fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
			var population = new Population (50, 70, chromosome);
	
			var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.MutationProbability = 0.2f;
            ga.Termination = new GenerationNumberTermination(100);
			TspChromosome bestChromosome = null;

            ga.GenerationRan += delegate 
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 2;
                
                bestChromosome =  (TspChromosome) ga.Population.BestChromosome;
                Console.WriteLine("Generations: {0}", ga.Population.GenerationsNumber);
                Console.WriteLine("Fitness: {0:n4}", bestChromosome.Fitness);
                Console.WriteLine("Distance: {0:n2}", bestChromosome.Distance);
                Console.WriteLine("Time: {0}", ga.TimeEvolving);
                Console.WriteLine("City tour: {0}", String.Join(", ", bestChromosome.GetGenes().Select(g => g.Value)));
            };

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("GeneticSharp - ConsoleApp");
            Console.ResetColor();

            try
            {
                ga.Start();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine();
                Console.WriteLine("Error: {0}", ex.Message);
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine();
            Console.WriteLine("Evolved.");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
