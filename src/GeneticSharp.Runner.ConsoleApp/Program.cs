using System;
using System.IO;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Infrastructure.Framework.Reflection;
using GeneticSharp.Infrastructure.Threading;
using GeneticSharp.Runner.ConsoleApp.Samples;

namespace GeneticSharp.Runner.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            Console.SetError(TextWriter.Null);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("GeneticSharp - ConsoleApp");
            Console.ResetColor();
            Console.WriteLine("Select the sample:");

            var sampleNames = TypeHelper.GetDisplayNamesByInterface<ISampleController>();

            for (int i = 0; i < sampleNames.Count; i++)
            {
                Console.WriteLine("{0}) {1}", i + 1, sampleNames[i]);
            }

            int sampleNumber = 0;
            string selectedSampleName = string.Empty;

            try
            {
                sampleNumber = Convert.ToInt32(Console.ReadLine());
                selectedSampleName = sampleNames[sampleNumber - 1];
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid option.");
            }

            var sampleController = TypeHelper.CreateInstanceByName<ISampleController>(selectedSampleName);
            DrawSampleName(selectedSampleName);
            sampleController.Initialize();

            Console.WriteLine("Starting...");

            var selection = sampleController.CreateSelection();
            var crossover = sampleController.CreateCrossover();
            var mutation = sampleController.CreateMutation();
            var fitness = sampleController.CreateFitness();
            var population = new Population(100, 200, sampleController.CreateChromosome());
            population.GenerationStrategy = new PerformanceGenerationStrategy();

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = sampleController.CreateTermination();            

            var terminationName = ga.Termination.GetType().Name;

            ga.GenerationRan += delegate
            {
                DrawSampleName(selectedSampleName);

                var bestChromosome = ga.Population.BestChromosome;
                Console.WriteLine("Termination: {0}", terminationName);
                Console.WriteLine("Generations: {0}", ga.Population.GenerationsNumber);
                Console.WriteLine("Fitness: {0,10}", bestChromosome.Fitness);
                Console.WriteLine("Time: {0}", ga.TimeEvolving);
                sampleController.Draw(bestChromosome);
            };

            try
            {
                sampleController.ConfigGA(ga);
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
            Run();
        }

        private static void DrawSampleName(string selectedSampleName)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("GeneticSharp - ConsoleApp");
            Console.WriteLine();
            Console.WriteLine(selectedSampleName);
            Console.ResetColor();
        }
    }
}
