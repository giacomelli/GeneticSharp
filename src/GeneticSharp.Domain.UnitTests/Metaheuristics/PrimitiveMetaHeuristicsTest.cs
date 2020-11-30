using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Infrastructure.Framework.Collections;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture()]
    [Category("MetaHeuristics")]
    class PrimitiveMetaHeuristicsTest
    {

        private IList<IChromosome> GetStubs()
        {
            IList<IChromosome> stubParents = Enumerable.Range(0, 10).Select(i => (IChromosome)new ChromosomeStub(20, 10).Initialized()).ToList();
            for (int i = 0; i < stubParents.Count; i++)
            {
                for (int g = 0; g < stubParents[0].Length; g++)
                {
                    stubParents[i].ReplaceGene(g, new Gene(i));
                }
            }

            return stubParents;
        }

        [Test()]
        public void ContainerMetaHeuristic_SubMetaheuristic_IsRun()
        {


            var stubParents = GetStubs();

            var testContainer = new ContainerMetaHeuristic();
            testContainer.CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability;


            var geomCrossover = new GeometricCrossover<int>().WithGeometricOperator<int>(geneValues => geneValues[0]);
            testContainer.SubMetaHeuristic = new CrossoverHeuristic().WithCrossover(geomCrossover);
            var ctx = new MetaHeuristicContext();


            //Testing subheuristic, returning first parent genes
            var offSpring = testContainer.MatchParentsAndCross(ctx, null, 1, stubParents, 0);
            Assert.AreEqual(1, offSpring.Count);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual(gene.Value, 0)));

            //Testing subheuristic, returning second parent genes
            geomCrossover.LinearGeometricOperator = geneValues => geneValues[1];
            offSpring = testContainer.MatchParentsAndCross(ctx, null, 1, stubParents, 2);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual(3, gene.Value )));

            //Testing no-op
            offSpring = testContainer.MatchParentsAndCross(ctx, null, 0, stubParents, 0);
            Assert.IsNull(offSpring, "with 0 probability, the cross operator should return null");


        }

        [Test()]
        public void SwitchMetaHeuristic_SubMetaheuristic_IsRun()
        {


            var stubParents = GetStubs();

            var testHeuristic = new SwitchMetaHeuristic<int>()
                .WithCaseGenerator(ParamScope.None, (heuristic, context) => context.Index);
            testHeuristic.CrossoverProbabilityStrategy = ProbabilityStrategy.TestProbability;

             
            //Defining crossovers with constant gene values
            for (int i = 0; i < 5; i++)
            {
                var iClosure = i;
                var geomCrossover = new GeometricCrossover<int>().WithGeometricOperator<int>(geneValues => iClosure);
                var geomHeuristic  = new CrossoverHeuristic().WithCrossover(geomCrossover);
                testHeuristic.PhaseHeuristics.Add(i, geomHeuristic);

            }
            
            var ctx = new MetaHeuristicContext();

            //Testing subheuristic, trigger switch with constant crossover based on Index
            var itemIndex = 4;
            ctx.Index = itemIndex;
            var offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 1, stubParents, itemIndex);
            Assert.AreEqual(1, offSpring.Count);
            offSpring.Each(o => o.GetGenes().Each(gene => Assert.AreEqual( 4, gene.Value)));

            //Testing no-op
            offSpring = testHeuristic.MatchParentsAndCross(ctx, null, 0, stubParents, 0);
            Assert.IsNull(offSpring, "with 0 probability, the cross operator should return null");


        }
    }
}