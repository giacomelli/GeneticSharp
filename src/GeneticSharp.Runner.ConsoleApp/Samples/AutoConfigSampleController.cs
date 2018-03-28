using System;
using System.ComponentModel;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.AutoConfig;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Framework.Threading;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    [DisplayName("AutoConfig")]
    public class AutoConfigSampleController : SampleControllerBase
    {
        public override IChromosome CreateChromosome()
        {
            return new AutoConfigChromosome();
        }

        public override ISelection CreateSelection()
        {
            return new EliteSelection();
        }

        public override ICrossover CreateCrossover()
        {
            return new UniformCrossover();
        }

        public override IMutation CreateMutation()
        {
            return new UniformMutation();
        }

		public override ITermination CreateTermination()
		{
            return new FitnessStagnationTermination(200);
		}

		public override IFitness CreateFitness()
        {
            var targetChromosome = new TspChromosome(10);
            var targetFitness = new TspFitness(10, 0, 100, 0, 100);

            var fitness = new AutoConfigFitness(targetFitness, targetChromosome);
            fitness.Termination = new FitnessStagnationTermination(500);
            fitness.PopulationMinSize = 20;
            fitness.PopulationMaxSize = 20;

            return fitness;
        }

        public override void ConfigGA(GeneticAlgorithm ga)
        {
            base.ConfigGA(ga);
            ga.TaskExecutor = new ParallelTaskExecutor()
            {
                MinThreads = 250,
                MaxThreads = 500
            };
        }

        public override void Draw(IChromosome bestChromosome)
        {
            var c = GA.BestChromosome as AutoConfigChromosome;
            Console.WriteLine($"Best GA configuration for TSP: {c.Crossover.GetType().Name} | {c.Mutation.GetType().Name} | {c.Selection.GetType().Name}");
        }
    }
}