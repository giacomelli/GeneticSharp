using System;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Framework.Commons;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Tsp
{
    [TestFixture()]
    [Category("Extensions")]
    public class TspTest
    {
        [Test()]
        public void Evolve_ManyGenerations_Fast()
        {
            int numberOfCities = 40;
            int populationSize = 40;

            var crossover = new OrderedCrossover();
            var mutation = new TworsMutation();

            var terminationCriterium = new GenerationNumberTermination(1001);

            var fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
            var chromosome = new TspChromosome(fitness.Cities.Count).Initialized();
            var result = Evolve_NbCities_Fast(fitness, chromosome, populationSize, null, crossover, mutation, terminationCriterium);
            Assert.GreaterOrEqual(result.Fitness, 0.8);
        }


        [Test()]
        public void Compare_Simple_Ordered_TwoOr_WOA_ManyGenerations_FitnessesBounded()
        {
            // population parameters
            int numberOfCities = 40;
            int populationSize = 100;
            var nbGenerations = 250;
            var nbGenerationsWOA = nbGenerations / 2;

            // Fitness and chromosomes 
            var fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
            var adamChromosome = new TspChromosome(fitness.Cities.Count).Initialized();
            var startFitness = fitness.Evaluate(adamChromosome);

            //start fitness should be random path, which is typically better than worst cases
            Assert.GreaterOrEqual(startFitness, 0.1);

            var termination = new GenerationNumberTermination(nbGenerations);

            // Native operators
            var crossover = new OrderedCrossover();
            var mutation = new TworsMutation();

            // Native evolution

            var resultOriginal = Evolve_NbCities_Fast(fitness, adamChromosome, populationSize, null, crossover, mutation, termination);

            // WOA parameters
            Func<double, int> getGeneValueFunction = d => Math.Round(d).PositiveMod(numberOfCities);

            var helicoidScale = 0.5;
            var metaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(true, nbGenerationsWOA, helicoidScale, geneValue => geneValue, getGeneValueFunction);

            // WOA evolution
            var resultWOA = Evolve_NbCities_Fast(fitness, adamChromosome, populationSize, metaHeuristic, crossover, mutation, termination);

            //Generally
            Assert.LessOrEqual(resultWOA.Fitness, resultOriginal.Fitness * 1.1);
            Assert.GreaterOrEqual(resultWOA.Fitness, resultOriginal.Fitness * 0.9);

        }


        private static TspEvolutionResult Evolve_NbCities_Fast(TspFitness fitness, TspChromosome adamChromosome,  int populationSize, IMetaHeuristic metaHeuristic, ICrossover crossover, IMutation mutation, ITermination termination, Action<IGeneticAlgorithm> generationUpdate = null)
        {
            var selection = new EliteSelection();
            var population = new Population(populationSize, populationSize, adamChromosome);
            population.GenerationStrategy = new TrackingGenerationStrategy();
            
            var ga = new MetaGeneticAlgorithm(population, fitness, selection, crossover, mutation);
            if (metaHeuristic!= null)
            {
                ga.Metaheuristic = metaHeuristic;
            }
            if (generationUpdate != null)
            {
                ga.GenerationRan += (sender, args) => generationUpdate(ga);
            }

            var firstValue = fitness.Evaluate(adamChromosome);
            var firstDistance = adamChromosome.Distance;
            
            ga.Termination = termination;
            ga.Start();
            var lastDistance = ((TspChromosome)ga.Population.BestChromosome).Distance;

            Assert.Less(lastDistance, firstDistance);

            return new TspEvolutionResult(){Population = ga.Population, TimeEvolving = ga.TimeEvolving, Distance =  lastDistance};
        }

      
    }
}

