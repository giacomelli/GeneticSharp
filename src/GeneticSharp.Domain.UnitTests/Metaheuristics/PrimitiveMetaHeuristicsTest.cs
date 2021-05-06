using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Infrastructure.Framework.Collections;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture]
    [Category("MetaHeuristics")]
    class PrimitiveMetaHeuristicsTest: MetaHeuristicTestBase
    {

       

        [Test]
        public void ContainerMetaHeuristic_SubMetaheuristic_IsRun()
        {


            var stubParents = GetStubChromosomes(10);

            var testContainer = new ContainerMetaHeuristic
            {
                CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability
            };


            var geomCrossover = new GeometricCrossover<int>().WithLinearGeometricOperator((geneIndex, geneValues) => geneValues[0]);
            testContainer.SubMetaHeuristic = new CrossoverHeuristic().WithCrossover(geomCrossover);
            IEvolutionContext ctx = new EvolutionContext().GetIndividual(0);
            

            //Testing subheuristic, returning first parent genes
            var offSpring = testContainer.MatchParentsAndCross(ctx, null, 1, stubParents);
            Assert.AreEqual(1, offSpring.Count);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual(gene.Value, 0)));

            //Testing subheuristic, returning second parent genes
            geomCrossover.LinearGeometricOperator = (geneIndex, geneValues) => geneValues[1];
            ctx = ctx.GetIndividual(2);
            offSpring = testContainer.MatchParentsAndCross(ctx, null, 1, stubParents);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual(3, gene.Value )));

            ctx = ctx.GetIndividual(0);
            //Testing no-op
            offSpring = testContainer.MatchParentsAndCross(ctx, null, 0, stubParents);
            Assert.IsNull(offSpring);


        }

        [Test]
        public void SwitchMetaHeuristic_Phases_AreRun()
        {


            var stubParents = GetStubChromosomes(10);

            var testHeuristic = new SwitchMetaHeuristic<int>()
                .WithCaseGenerator(ParamScope.None, (heuristic, context) => context.OriginalIndex);
            testHeuristic.CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability;


            var nbPhases = 5;
            var geometricHeuristics = GetGeometricCrossoverStubs(nbPhases);

            //Defining crossovers with constant gene values
            for (int i = 0; i < nbPhases; i++)
            {
                testHeuristic.PhaseHeuristics.Add(i, geometricHeuristics[i]);

            }
            
            IEvolutionContext ctx = new EvolutionContext();

            //Testing subheuristic, trigger switch with constant crossover based on Index
            var phaseIndex = 4;
            ctx = ctx.GetIndividual(phaseIndex);
            var offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 1, stubParents);

            Assert.AreEqual(1, offSpring.Count);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual(phaseIndex, gene.Value)));


            //Testing no-op
            ctx = ctx.GetIndividual(0);
            offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 0, stubParents);
            Assert.IsNull(offSpring);


        }



        [Test]
        public void GenerationMetaHeuristic_Phases_AreRun()
        {


            var stubParents = GetStubChromosomes(100);

            //Defining crossovers with constant gene values
            var nbPhases = 5;
            var geometricHeuristics = GetGeometricCrossoverStubs(nbPhases);

            var phaseDuration = 10;

            var testHeuristic = new GenerationMetaHeuristic(phaseDuration, geometricHeuristics.ToArray())
            {
                CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability
            };

            IEvolutionContext ctx = new EvolutionContext();

            //Testing subheuristic, trigger switch with constant crossover based on Index
            var phaseIndex = 4;
            
            ctx.Population = new Population(10,10, stubParents[0]);
            //Generations have a 1 based index
            ctx.Population.CreateNewGeneration(stubParents);
            //Incrementing generations to reach phase index
            for (int i = 0; i < phaseIndex*phaseDuration; i++)
            {
                ctx.Population.CreateNewGeneration(stubParents);
            }

            ctx = ctx.GetIndividual(0);
            var offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 1, stubParents);
            Assert.AreEqual(1, offSpring.Count);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual(phaseIndex, gene.Value)));

            //Testing no-op
            offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 0, stubParents);
            Assert.IsNull(offSpring);


        }


        [Test]
        public void PopulationMetaHeuristic_Phases_AreRun()
        {
            var groupSize = 10;
            var nbPhases = 5;

            var stubParents = GetStubChromosomes(groupSize * nbPhases);

            //Defining crossovers with constant gene values
            
            var geometricHeuristics = GetGeometricCrossoverStubs(nbPhases);



            var testHeuristic = new PopulationMetaHeuristic(groupSize, geometricHeuristics.ToArray())
            {
                CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability
            };

            IEvolutionContext ctx = new EvolutionContext();

            //Testing subheuristic, trigger switch with constant crossover based on Index
            var phaseIndex = 4;
            var populationSize = groupSize * nbPhases;
            ctx.Population = new Population(populationSize, populationSize, stubParents[0]);
            ctx.Population.CreateNewGeneration(stubParents);

            var itemIndex = phaseIndex * groupSize;
            ctx = ctx.GetIndividual(itemIndex);

            var offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 1, stubParents);
            Assert.AreEqual(1, offSpring.Count);
            offSpring.Each(o => o.GetGenes().Each(gene =>
            {
                Assert.AreEqual(phaseIndex, (int)gene.Value );
            }));

            //Testing no-op
            offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 0, stubParents);
            Assert.IsNull(offSpring);


        }


      



    }
}