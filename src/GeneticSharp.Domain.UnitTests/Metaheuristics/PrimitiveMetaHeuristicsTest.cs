using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Infrastructure.Framework.Collections;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture()]
    [Category("MetaHeuristics")]
    class PrimitiveMetaHeuristicsTest: MetaHeuristicTestBase
    {

       

        [Test()]
        public void ContainerMetaHeuristic_SubMetaheuristic_IsRun()
        {


            var stubParents = GetStubs(10);

            var testContainer = new ContainerMetaHeuristic();
            testContainer.CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability;


            var geomCrossover = new GeometricCrossover<int>().WithGeometricOperator<int>(geneValues => geneValues[0]);
            testContainer.SubMetaHeuristic = new CrossoverHeuristic().WithCrossover(geomCrossover);
            var ctx = new MetaHeuristicContext();
            

            //Testing subheuristic, returning first parent genes
            var offSpring = testContainer.MatchParentsAndCross(ctx, null, 1, stubParents);
            Assert.AreEqual(1, offSpring.Count);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual(gene.Value, 0)));

            //Testing subheuristic, returning second parent genes
            geomCrossover.LinearGeometricOperator = geneValues => geneValues[1];
            ctx.Index = 2;
            offSpring = testContainer.MatchParentsAndCross(ctx, null, 1, stubParents);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual(3, gene.Value )));

            ctx.Index = 0;
            //Testing no-op
            offSpring = testContainer.MatchParentsAndCross(ctx, null, 0, stubParents);
            Assert.IsNull(offSpring, "with 0 probability, the cross operator should return null");


        }

        [Test()]
        public void SwitchMetaHeuristic_Phases_AreRun()
        {


            var stubParents = GetStubs(10);

            var testHeuristic = new SwitchMetaHeuristic<int>()
                .WithCaseGenerator(ParamScope.None, (heuristic, context) => context.Index);
            testHeuristic.CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability;


            var nbPhases = 5;
            var geometricHeuristics = GetGeometricCrossoverStubs(nbPhases);

            //Defining crossovers with constant gene values
            for (int i = 0; i < nbPhases; i++)
            {
                testHeuristic.PhaseHeuristics.Add(i, geometricHeuristics[i]);

            }
            
            var ctx = new MetaHeuristicContext();

            //Testing subheuristic, trigger switch with constant crossover based on Index
            var phaseIndex = 4;
            ctx.Index = phaseIndex;
            var offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 1, stubParents);

            Assert.AreEqual(1, offSpring.Count);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual(phaseIndex, gene.Value)));


            //Testing no-op
            ctx.Index = 0;
            offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 0, stubParents);
            Assert.IsNull(offSpring, "with 0 probability, the cross operator should return null");


        }



        [Test()]
        public void GenerationMetaHeuristic_Phases_AreRun()
        {


            var stubParents = GetStubs(100);

            //Defining crossovers with constant gene values
            var nbPhases = 5;
            var geometricHeuristics = GetGeometricCrossoverStubs(nbPhases);

            var phaseDuration = 10;

            var testHeuristic = new GenerationMetaHeuristic(phaseDuration, geometricHeuristics.ToArray());
            testHeuristic.CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability;

            var ctx = new MetaHeuristicContext();

            //Testing subheuristic, trigger switch with constant crossover based on Index
            var phaseIndex = 4;
            
            ctx.Population = new Population(10,10, stubParents[0]);
            for (int i = 0; i < phaseIndex*phaseDuration; i++)
            {
                ctx.Population.CreateNewGeneration(stubParents);
            }

            ctx.Index = 0;
            var offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 1, stubParents);
            Assert.AreEqual(1, offSpring.Count);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual(phaseIndex, gene.Value)));

            //Testing no-op
            offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 0, stubParents);
            Assert.IsNull(offSpring, "with 0 probability, the cross operator should return null");


        }


        [Test()]
        public void PopulationMetaHeuristic_Phases_AreRun()
        {
            var groupSize = 10;
            var nbPhases = 5;

            var stubParents = GetStubs(groupSize * nbPhases);

            //Defining crossovers with constant gene values
            
            var geometricHeuristics = GetGeometricCrossoverStubs(nbPhases);

            

            var testHeuristic = new PopulationMetaHeuristic(groupSize , geometricHeuristics.ToArray());
            testHeuristic.CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability;

            var ctx = new MetaHeuristicContext();

            //Testing subheuristic, trigger switch with constant crossover based on Index
            var phaseIndex = 4;
            var populationSize = groupSize * nbPhases;
            ctx.Population = new Population(populationSize, populationSize, stubParents[0]);
            ctx.Population.CreateNewGeneration(stubParents);

            var itemIndex = phaseIndex * groupSize;
            ctx.Index = itemIndex;

            var offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 1, stubParents);
            Assert.AreEqual(1, offSpring.Count);
            offSpring.Each(o => o.GetGenes().Each(gene =>
            {
                Assert.AreEqual(phaseIndex, (int)gene.Value );
            }));

            //Testing no-op
            offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 0, stubParents);
            Assert.IsNull(offSpring, "with 0 probability, the cross operator should return null");


        }


      



    }
}