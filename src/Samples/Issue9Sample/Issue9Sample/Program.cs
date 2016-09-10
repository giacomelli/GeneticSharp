using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;

namespace Issue9Sample
{
    class Program
    {
        static void Main(string[] args)
		{
			// Creates and run the first GA.
			var chromosome = new TspChromosome(100);
			var population = new Population(50, 50, chromosome);
			var ga = CreateGA(population);
			ga.Termination = new GenerationNumberTermination(100);
			ga.Start();
			Console.WriteLine("Best chromosome before chromossomes serialization is:");
			ShowChromosome(ga.BestChromosome as TspChromosome);
			SerializeChromosomes(population.CurrentGeneration.Chromosomes);


			// Reload GA with serialized chromossomes from previous GA.
			Console.WriteLine("Deserializing...");
			var chromosomes = DerializeChromosomes();

			// Use a diff IPopulation implementation.
			var preloadPopulation = new PreloadPopulation(50, 50, chromosomes);
			ga = CreateGA(preloadPopulation);
			ga.Termination = new GenerationNumberTermination(1);
			ga.Start();
			Console.WriteLine("Best chromosome is:");
			ShowChromosome(ga.BestChromosome as TspChromosome);

			Console.ReadKey();
		}

		static GeneticAlgorithm CreateGA(IPopulation population)
		{
			var selection = new EliteSelection();
			var crossover = new OrderedCrossover();
			var mutation = new ReverseSequenceMutation();
			var fitness = new TspFitness(100, 0, 1000, 0, 1000);
		
			return new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
		}

		static void SerializeChromosomes(IList<IChromosome> chromosomes)
		{
			using (var stream = File.Create("chromosomes.bin"))
			{
				var bf = new BinaryFormatter();
				bf.Serialize(stream, chromosomes);
			}
		}

		static IList<IChromosome> DerializeChromosomes()
		{
			using (var stream = File.OpenRead("chromosomes.bin"))
			{
				var bf = new BinaryFormatter();
				return bf.Deserialize(stream) as IList<IChromosome>;
			}
		}
		private static void ShowChromosome(TspChromosome c)
		{
			Console.WriteLine("Fitness: {0:n2}", c.Fitness);
			Console.WriteLine("Cities: {0:n0}", c.Length);
			Console.WriteLine("Distance: {0:n2}", c.Distance);

			var cities = c.GetGenes().Select(g => g.Value.ToString()).ToArray();
			Console.WriteLine("City tour: {0}", string.Join(", ", cities));
		}
    }
}
