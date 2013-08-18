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
            Console.WriteLine("GeneticSharp - TSP");

            int numberOfCities = 40;
            var selection = new EliteSelection();
            var crossover = new OrderedCrossover();
            var mutation = new TworsMutation();
            var chromosome = new TspChromosome(numberOfCities);
            var fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
			var population = new Population (40, 40, chromosome);
	
			var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
			ga.Termination = new GenerationNumberTermination (1000);
			ga.GenerationRan += delegate { Console.Write("."); };

            var sw = new Stopwatch();
            sw.Start();
			ga.Evolve();
            sw.Stop();

            Console.WriteLine("\nDone in {0}.", sw.Elapsed);
            Console.ReadKey();
        }
    }
}
