using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Compound;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Framework.Commons;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Tsp
{
    [TestFixture]
    [Category("Extensions")]
    public class TspTest
    {
        [Test]
        public void Evolve_ManyGenerations_Ordered_Twors_GoodFitness()
        {
            var minFitness = 0.87;
            int numberOfCities = 40;
            int populationSize = 40;

            var crossover = new OrderedCrossover();
            var mutation = new TworsMutation();

            var terminationCriterium = new GenerationNumberTermination(1001);

            var fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
            var chromosome = new TspChromosome(fitness.Cities.Count).Initialized();
            var result = Evolve_NbCities_Fast(fitness, chromosome, populationSize, null, crossover, mutation, terminationCriterium);
            Assert.GreaterOrEqual(result.Fitness, minFitness);
        }

        [Test]
        public void Evolve_Simple_ManyGenerations_Ordered_ReverseSequence_ExcellentFitness()
        {
            var minFitness = 0.93;
            int numberOfCities = 40;
            int populationSize = 40;

            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();

            var terminationCriterium = new GenerationNumberTermination(1001);

            var fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
            var chromosome = new TspChromosome(fitness.Cities.Count).Initialized();
            var result = Evolve_NbCities_Fast(fitness, chromosome, populationSize, null, crossover, mutation, terminationCriterium);
            Assert.GreaterOrEqual(result.Fitness, minFitness);
        }

        /// <summary>
        /// Naive usage of a geometric heuristic with a TSP chromosome projects the city indices that make up into equivalent doubles, which is not a good projection. 
        /// </summary>
        [Test]
        public void Compare_Simple_Ordered_TwoOr_WOA_ManyGenerations_FitnessesBounded()
        {
            var repeatNb = 2;
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
            var mutation = new ReverseSequenceMutation();

            // Native evolution

            var resultOriginal = Evolve_NbCities_Fast_Repeat(repeatNb, fitness, adamChromosome, populationSize, null, crossover, mutation, termination);

            // WhaleOptimisation parameters
            int GetGeneValueFunction(int geneIndex, double d) => Math.Round(d).PositiveMod(numberOfCities);

            var noEmbeddingConverter = new GeometricConverter<int>
            {
                IsOrdered = true,
                DoubleToGeneConverter = GetGeneValueFunction,
                GeneToDoubleConverter = (genIndex, geneValue) => geneValue
            };

            var woa = new WhaleOptimisationAlgorithm()
            {
                MaxGenerations = nbGenerationsWOA,
                ForceReinsertion = false
               };
            woa.SetGeometricConverter(noEmbeddingConverter);
            var metaHeuristic = woa.Build();

            // WhaleOptimisation evolution
            var resultWOA = Evolve_NbCities_Fast(fitness, adamChromosome, populationSize, metaHeuristic, crossover, mutation, termination);

            //Generally
            Assert.LessOrEqual(resultWOA.Fitness, resultOriginal.Fitness * 1.3);
            Assert.GreaterOrEqual(resultWOA.Fitness, resultOriginal.Fitness * 0.7);

        }


        private static ITspEvolutionResult Evolve_NbCities_Fast_Repeat(int repeatNb, TspFitness fitness, TspChromosome adamChromosome, int populationSize, IMetaHeuristic metaHeuristic, ICrossover crossover, IMutation mutation, ITermination termination, Action<IGeneticAlgorithm> generationUpdate = null)
        {
            var meanResult = new TspMeanEvolutionResult();
            for (int i = 0; i < repeatNb; i++)
            {
                var resulti = Evolve_NbCities_Fast(fitness, adamChromosome, populationSize, metaHeuristic, crossover,
                    mutation, termination, generationUpdate);
                meanResult.Results.Add(resulti);
            }

            return meanResult;
        }


        private static TspEvolutionResult Evolve_NbCities_Fast(TspFitness fitness, TspChromosome adamChromosome,  int populationSize, IMetaHeuristic metaHeuristic, ICrossover crossover, IMutation mutation, ITermination termination, Action<IGeneticAlgorithm> generationUpdate = null)
        {
            var selection = new EliteSelection();
            var population = new Population(populationSize, populationSize, adamChromosome)
            {
                GenerationStrategy = new TrackingGenerationStrategy()
            };

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

            return new TspEvolutionResult {Population = ga.Population, TimeEvolving = ga.TimeEvolving};
        }

      
    }
}

