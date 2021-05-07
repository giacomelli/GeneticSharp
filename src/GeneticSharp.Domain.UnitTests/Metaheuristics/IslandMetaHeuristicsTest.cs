using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Extensions.Mathematic;
using GeneticSharp.Extensions.Mathematic.Functions;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{

    public class IslandGene
    {

        public bool NewIsland { get; set; }

        public KnownCompoundMetaheuristics IslandMetaheuristic;

        public bool NoMutation { get; set; }

        public IReinsertion Reinsertion { get; set; }

    }


    [TestFixture]
    [Category("MetaHeuristics")]
    public class IslandMetaHeuristicsTest : MetaHeuristicTestBase
    {

        /// <summary>
        /// Island algorithm with 5 default GA islands exhibit evolution
        /// </summary>
        [Test]
        public void Evolve_IslandDefault_Ackley_Small_Optmization()
        {

            var minFit = -8;

            var repeatNb = 1;
            //repeatNb = 3;

            var sizes = new int[] { 10 };

            var knownFunctions = GetKnownFunctions(false);
            var ackley = knownFunctions.Take(1).ToList();


            var fitnessBasedElitist = new FitnessBasedElitistReinsertion();
            var pure = new PureReinsertion();
            var pairwise = new FitnessBasedPairwiseReinsertion();

            var defaultMaxTime = 2;
            var defaultNbGens = 50;
            var defaultPopSize = 100;

            var testParams =
                new (KnownCompoundMetaheuristics kind, double duration, int nbGenerations, bool noMutation, int
                    populationSize, IReinsertion reinsertion, bool forceReinsertion)[]
                    {

                        (KnownCompoundMetaheuristics.Islands5Default, defaultMaxTime, defaultNbGens, false,
                            defaultPopSize, fitnessBasedElitist, false),

                    };

            var sw = Stopwatch.StartNew();
            var functionResults = EvolveMetaHeuristicsFunctionsTestParams(ackley, sizes, repeatNb, testParams);

            sw.Stop();
            var testDuration = sw.Elapsed;

            AssertEvolution(functionResults[0].sizeResults[0][0], minFit, false);

        }


        /// <summary>
        /// Island algorithm with 5 default GA islands and some migrations outperforms same island configuration without migration
        /// </summary>
        [Test]
        public void Compare_IslandDefault_IslandDefaultNoMigration_Ackley_Small_BetterFitness()
        {
            var ratiosBySize = new double[] {1.3, 1.2, 1.2};

            var repeatNb = 5;
            //repeatNb = 3;

            //var sizes = new int[] { 10 };
            var sizes = SmallSizes;

            var knownFunctions = GetKnownFunctions(false);
            var ackley = knownFunctions.Take(1).ToList();


            var fitnessBasedElitist = new FitnessBasedElitistReinsertion();
            var pure = new PureReinsertion();
            var pairwise = new FitnessBasedPairwiseReinsertion();

            var defaultMaxTime = 2;
            var defaultNbGens = 50;
            var defaultPopSize = 250;

            var testParams = new (KnownCompoundMetaheuristics kind, double duration, int nbGenerations, bool noMutation, int populationSize, IReinsertion reinsertion, bool forceReinsertion)[]
            {
                (KnownCompoundMetaheuristics.Islands5Default,  defaultMaxTime , defaultNbGens,  false, defaultPopSize, fitnessBasedElitist, false),
                (KnownCompoundMetaheuristics.Islands5DefaultNoMigration,  defaultMaxTime , defaultNbGens,  false, defaultPopSize, fitnessBasedElitist, false),

            };

            var sw = Stopwatch.StartNew();
            var functionResults = EvolveMetaHeuristicsFunctionsTestParams(ackley, sizes, repeatNb, testParams);

            sw.Stop();
            var testDuration = sw.Elapsed;
            var sizeResults = functionResults[0].sizeResults;
            for (int i = 0; i < sizeResults.Count; i++)
            {
                var paramResults = sizeResults[i];
                AssertIsPerformingLessByRatio(EvolutionMeasure.Fitness, ratiosBySize[i], paramResults[0], paramResults[1]);
            }
            

        }


        [Test]
        public void Compare_IslandDefaultNoMigration_SmallDefault_Ackley_Small_BetterFitness()
        {
            var ratiosBySize = new double[] { 1.01, 1.01, 1.01 };

            var repeatNb = 5;
            //repeatNb = 3;

            //var sizes = new int[] { 10 };
            var sizes = SmallSizes;

            var knownFunctions = GetKnownFunctions(false);
            var ackley = knownFunctions.Take(1).ToList();


            var fitnessBasedElitist = new FitnessBasedElitistReinsertion();
            var pure = new PureReinsertion();
            var pairwise = new FitnessBasedPairwiseReinsertion();

            var defaultMaxTime = 10;
            var defaultNbGens = 50;
            var defaultPopSize = 250;
            var smallPopSize = defaultPopSize / 5;

            var testParams = new (KnownCompoundMetaheuristics kind, double duration, int nbGenerations, bool noMutation, int populationSize, IReinsertion reinsertion, bool forceReinsertion)[]
            {
                (KnownCompoundMetaheuristics.Islands5DefaultNoMigration,  defaultMaxTime , defaultNbGens,  false, defaultPopSize, fitnessBasedElitist, false),
                (KnownCompoundMetaheuristics.Default,  defaultMaxTime , defaultNbGens,  false, smallPopSize, fitnessBasedElitist, false),

            };

            var sw = Stopwatch.StartNew();
            var functionResults = EvolveMetaHeuristicsFunctionsTestParams(ackley, sizes, repeatNb, testParams);

            sw.Stop();
            var testDuration = sw.Elapsed;
            var sizeResults = functionResults[0].sizeResults;
            for (int i = 0; i < sizeResults.Count; i++)
            {
                var paramResults = sizeResults[i];
                AssertIsPerformingLessByRatio(EvolutionMeasure.Fitness, ratiosBySize[i], paramResults[0], paramResults[1]);
            }


        }

        [Test]
        public void Compare_IslandDefault_Default_Ackley_Small_BetterFitness()
        {
            var ratiosBySize = new double[] { 1.05, 1.001, 1.001 };

            var repeatNb = 5;
            //repeatNb = 3;

            //var sizes = new int[] { 10 };
            var sizes = SmallSizes;

            var knownFunctions = GetKnownFunctions(false);
            var ackley = knownFunctions.Take(1).ToList();


            var fitnessBasedElitist = new FitnessBasedElitistReinsertion();
            var pure = new PureReinsertion();
            var pairwise = new FitnessBasedPairwiseReinsertion();

            var defaultMaxTime = 10;
            var defaultNbGens = 50;
            var defaultPopSize = 250;

            var testParams = new (KnownCompoundMetaheuristics kind, double duration, int nbGenerations, bool noMutation, int populationSize, IReinsertion reinsertion, bool forceReinsertion)[]
            {
                (KnownCompoundMetaheuristics.Islands5Default,  defaultMaxTime , defaultNbGens,  false, defaultPopSize, fitnessBasedElitist, false),
                (KnownCompoundMetaheuristics.Default,  defaultMaxTime , defaultNbGens,  false, defaultPopSize, fitnessBasedElitist, false),

            };

            var sw = Stopwatch.StartNew();
            var functionResults = EvolveMetaHeuristicsFunctionsTestParams(ackley, sizes, repeatNb, testParams);

            sw.Stop();
            var testDuration = sw.Elapsed;
            var sizeResults = functionResults[0].sizeResults;
            for (int i = 0; i < sizeResults.Count; i++)
            {
                var paramResults = sizeResults[i];
                AssertIsPerformingLessByRatio(EvolutionMeasure.Fitness, ratiosBySize[i], paramResults[0], paramResults[1]);
            }


        }
    }
}


