using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Metaheuristics.Matching;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;

namespace GeneticSharp.Domain.Metaheuristics.Compound
{
    /// <summary>
    ///  As detailed in <see href="https://ww2.mathworks.cn/matlabcentral/fileexchange/76299-forensic-based-investigation-algorithm-fbi">Forensic-based investigation algorithm</see>
    /// Implemented directly from <see href="https://github.com/thieu1995/mealpy/blob/master/mealpy/human_based/FBIO.py">Mealpy version</see>
    /// This is the default version of the FBI algorithm with reduced lambda expressions.
    /// </summary>
    public class ForensicBasedInvestigation : GeometricMetaHeuristicBase
    {

        //Declare the parameters used for better name access (note that the local variables won't be used directly because they will be thread-safe / context-dependent parameters
        private enum FBIParam
        {
            pbSize,
            nChange,
            pBest,
            pWorst,
            prob,
            randomBetter
        }


       protected virtual object A1StepOperator(int geneIndex, IEnumerable<object> geneValues, IGeometricConverter geometricConverter, int nchange)
        {
            var rnd = RandomizationProvider.Current;
            if (geneIndex != nchange)
            {
                return geneValues.First();
            }
            var metricValues = geneValues.Select(value => geometricConverter.GeneToDouble(geneIndex, value)).ToList();
            var geometricValue = metricValues[0] + rnd.GetNormal(0, 1) * (metricValues[0] - (metricValues[1] + metricValues[2]) / 2);
            //var geometricValue = metricValues[0] + (rnd.GetDouble()-0.5) * (metricValues[0] - (metricValues[1] + metricValues[2]) / 2);
            var toReturn = geometricConverter.DoubleToGene(geneIndex, geometricValue);
            return toReturn;
        }

       protected virtual object A2StepOperator(int geneIndex, IEnumerable<object> geneValues, IGeometricConverter geometricConverter)
        {
            var rnd = RandomizationProvider.Current;
            if (rnd.GetDouble() < 0.5)
            {
                return geneValues.First();
            }
            var metricValues = geneValues.Select(value => geometricConverter.GeneToDouble(geneIndex, value)).ToList();

            var geometricValue = metricValues[1] + metricValues[2] + rnd.GetDouble() * (metricValues[3] - metricValues[4]);
            var toReturn = geometricConverter.DoubleToGene(geneIndex, geometricValue);
            return toReturn;
        }

        /// <summary>
        /// Eq.(6) in FBI Inspired Meta-Optimization
        /// </summary>
        protected virtual object B1StepOperator(int geneIndex, IEnumerable<object> geneValues, IGeometricConverter geometricConverter)
        {
            var rnd = RandomizationProvider.Current;
            var metricValues = geneValues.Select(value => geometricConverter.GeneToDouble(geneIndex, value)).ToList();

            var geometricValue = rnd.GetDouble() * metricValues[0] + rnd.GetDouble() * (metricValues[1] - metricValues[0]);
            var toReturn = geometricConverter.DoubleToGene(geneIndex, geometricValue);
            return toReturn;
        }

        /// <summary>
        /// Eq.(7) and Eq.(8) in FBI Inspired Meta-Optimization
        /// </summary>
        protected virtual object B2StepOperator(int geneIndex, IEnumerable<object> geneValues, IGeometricConverter geometricConverter, bool randomBetter)
        {
            var rnd = RandomizationProvider.Current;
            var metricValues = geneValues.Select(value => geometricConverter.GeneToDouble(geneIndex, value)).ToList();

            double geometricValue;
            if (randomBetter)
            {
                //Eq.(7) in FBI Inspired Meta-Optimization
                geometricValue = metricValues[0] + rnd.GetDouble() * (metricValues[1] - metricValues[0]) + rnd.GetDouble() * (metricValues[2] - metricValues[1]);
                
            }
            else
            {
                //Eq.(8) in FBI Inspired Meta - Optimization
                geometricValue = metricValues[0] + rnd.GetDouble() * (metricValues[0] - metricValues[1]) + rnd.GetDouble() * (metricValues[2] - metricValues[0]);
            }
            
            var toReturn = geometricConverter.DoubleToGene(geneIndex, geometricValue);
            return toReturn;
        }

        protected virtual double GetA2Prob(IEvolutionContext ctx, double pBest, double pWorst)
        {
            return (ctx.Population.CurrentGeneration.Chromosomes[ctx.LocalIndex].Fitness.Value - pWorst) / (pBest - pWorst);
        }

        protected virtual bool GetA2Switch(double prob)
        {
            return RandomizationProvider.Current.GetDouble() > prob;
        }

        protected override IContainerMetaHeuristic BuildMainHeuristic()
        {

            var rnd = RandomizationProvider.Current;

            var a1StepHeuristic = new MatchMetaHeuristic().WithName("A1 Step")
                .WithParam(nameof(FBIParam.pbSize), "Size of pb = nb of genes", ParamScope.Evolution, (h, ctx) => ctx.Population.CurrentGeneration.Chromosomes[0].GetGenes().Length)
                .WithParam(nameof(FBIParam.nChange), "random gene index for update", ParamScope.MetaHeuristic | ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, int pbSize) => rnd.GetInt(0, pbSize))
                .WithMatches(MatchingKind.Current, MatchingKind.Random, MatchingKind.Random)
                .WithCrossoverMetaHeuristic(new CrossoverHeuristic()
                    .WithCrossover(ParamScope.None, (IMetaHeuristic h, IEvolutionContext ctx, int nChange) => new GeometricCrossover<object>(GeometricConverter.IsOrdered, 3, false)
                        .WithLinearGeometricOperator((geneIndex, geneValues) => A1StepOperator(geneIndex, geneValues, GeometricConverter, nChange))
                        .WithGeometryEmbedding(GeometricConverter.GetEmbedding())));

            var a2StepHeuristic = new IfElseMetaHeuristic().WithName("A2 Step")
                .WithScope(EvolutionStage.Crossover)
                .WithParam(nameof(FBIParam.pBest), "best fitness", ParamScope.MetaHeuristic | ParamScope.Generation, (h, ctx) => (ctx.Population.CurrentGeneration.BestChromosome.Fitness.Value))
                .WithParam(nameof(FBIParam.pWorst), "worst fitness", ParamScope.MetaHeuristic | ParamScope.Generation, (h, ctx) => (ctx.Population.CurrentGeneration.GetWorstChromosomes(1).First().Fitness.Value))
                .WithParam(nameof(FBIParam.prob), "fitness scale", ParamScope.MetaHeuristic | ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, double pBest, double pWorst) => GetA2Prob(ctx, pBest, pWorst))
                .WithCaseGenerator(ParamScope.None, (IMetaHeuristic h, IEvolutionContext ctx, double prob) => GetA2Switch(prob) /*rnd.GetDouble() > prob*/)
                .WithTrue(new MatchMetaHeuristic()
                    .WithMatches(MatchingKind.Current, MatchingKind.Best, MatchingKind.Random, MatchingKind.Random, MatchingKind.Random)
                    .WithCrossoverMetaHeuristic(new CrossoverHeuristic()
                        .WithCrossover(ParamScope.Constant, (h, ctx) => new GeometricCrossover<object>(GeometricConverter.IsOrdered, 5, false)
                            .WithLinearGeometricOperator((geneIndex, geneValues) => A2StepOperator(geneIndex, geneValues, GeometricConverter))
                            .WithGeometryEmbedding(GeometricConverter.GetEmbedding()))))
                .WithFalse(new NoOpMetaHeuristic());

            var b1StepHeuristic = new MatchMetaHeuristic().WithName("B1 Step")
                .WithMatches(MatchingKind.Current, MatchingKind.Best)
                .WithCrossoverMetaHeuristic(new CrossoverHeuristic()
                    .WithCrossover(ParamScope.Constant, (h, ctx) =>
                        new GeometricCrossover<object>(GeometricConverter.IsOrdered, 2, false)
                            .WithLinearGeometricOperator((geneIndex, geneValues) =>
                                B1StepOperator(geneIndex, geneValues, GeometricConverter))
                            .WithGeometryEmbedding(GeometricConverter.GetEmbedding())));

            var b2StepHeuristic = new MatchMetaHeuristic().WithName("B2 Step")
                .WithParam(nameof(FBIParam.randomBetter), "random better than current", ParamScope.MetaHeuristic | ParamScope.Generation | ParamScope.Individual, (h, ctx) => (ctx.SelectedParents[0].Fitness > ctx.SelectedParents[1].Fitness))
                .WithMatches(MatchingKind.Current, MatchingKind.Random, MatchingKind.Best)
                .WithCrossoverMetaHeuristic(new CrossoverHeuristic()
                    .WithCrossover(ParamScope.None, (IMetaHeuristic h, IEvolutionContext ctx, bool randomBetter) =>
                        new GeometricCrossover<object>(GeometricConverter.IsOrdered,3 , false)
                            .WithLinearGeometricOperator((geneIndex, geneValues) =>
                                B2StepOperator(geneIndex, geneValues, GeometricConverter, randomBetter))
                            .WithGeometryEmbedding(GeometricConverter.GetEmbedding())));

            var fbiHeuristic = new GenerationMetaHeuristic(1, a1StepHeuristic,a2StepHeuristic, b1StepHeuristic, b2StepHeuristic)
                .WithName("Forensic Based Investigation", "Optimization algorithm, inspired by the suspect investigation–location–pursuit process that is used by police officers. Jui-Sheng Chou, Ngoc-Mai Nguyen (2020)")
                .WithScope(EvolutionStage.Crossover)
                ;

            return fbiHeuristic;
        }

        /// <summary>
        /// The original FBI publication describes a pairwise reinsertion scheme
        /// </summary>
        public override IReinsertion GetDefaultReinsertion()
        {
            return new FitnessBasedPairwiseReinsertion();
        }
    }
}