using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Threading;
using GeneticSharp.Runner.ConsoleApp.Samples;

namespace GeneticSharp.Runner.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("GeneticSharp - ConsoleApp");
            Console.ResetColor();
            Console.WriteLine("Select the sample:");
            Console.WriteLine("1) TSP (Travelling Salesman Problem)");
            Console.WriteLine("2) Ghostwriter");
            var sampleNumber = Console.ReadLine();
            ISampleController sampleController = null;

            switch (sampleNumber)
            {
                case "1":
                    sampleController = new TspSampleController(20);
                    break;

                case "2":
                    sampleController = new GhostwriterSampleController();
                    break;

                default:
                    return;
            }

            var selection = new EliteSelection();
            var crossover = new UniformCrossover();
            var mutation = new UniformMutation(true);
            var fitness = sampleController.CreateFitness();
            var population = new Population(50, 70, sampleController.CreateChromosome());

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.MutationProbability = 0.4f;
            ga.Termination = new FitnessStagnationTermination(100);

            ga.TaskExecutor = new SmartThreadPoolTaskExecutor()
            {
                MinThreads = 25,
                MaxThreads = 50
            };

            ga.GenerationRan += delegate
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 5;

                var bestChromosome = ga.Population.BestChromosome;
                Console.WriteLine("Generations: {0}", ga.Population.GenerationsNumber);
                Console.WriteLine("Fitness: {0:n4}", bestChromosome.Fitness);
                Console.WriteLine("Time: {0}", ga.TimeEvolving);
                sampleController.Draw(bestChromosome);
            };

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
