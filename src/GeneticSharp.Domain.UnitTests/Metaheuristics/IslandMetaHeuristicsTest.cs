using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Reinsertions;
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


        [Test]
        public void Evolve_IslandDefault_Stub_Small_Optmization()
        {
            var islandDEfaultName = nameof(KnownCompoundMetaheuristics.Islands5Default);
            IMetaHeuristic MetaHeuristic(int maxValue) =>  MetaHeuristicsService.CreateMetaHeuristicByName(islandDEfaultName, 50, SmallPopulationSize, null, true); //GetDefaultWhaleHeuristicForChromosomStub(false, 300, maxValue);
            IChromosome AdamChromosome(int maxValue) => new ChromosomeStub(maxValue, maxValue);
            IFitness Fitness(int maxValue) => new FitnessStub(maxValue) { SupportsParallel = false };

            var reinsertion = new FitnessBasedElitistReinsertion();

            var compoundResults = EvolveMetaHeuristicDifferentSizes(5,
                Fitness,
                AdamChromosome,
                SmallSizes,
                MetaHeuristic,
                i => 0.5,
                reinsertion);


            for (int i = 0; i < compoundResults.Count; i++)
            {
                AssertEvolution(compoundResults[i].result, compoundResults[i].minFitness, false);
            }

        }


    }
}