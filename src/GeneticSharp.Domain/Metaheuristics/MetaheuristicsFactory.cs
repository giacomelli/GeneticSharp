using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Provides a factory to generate common well known Metaheuristics
    /// </summary>
    public static class MetaHeuristicsFactory
    {

        //Declare the parameters used for better name access (note that the local variables won't be used directly because they will be thread-safe / context-dependent parameters
        private enum WOAParam
        {
            a, 
            a2, 
            A, 
            C, 
            l
        }


        /// <summary>
        /// As detailed in <see href="https://en.wikiversity.org/wiki/Whale_Optimization_Algorithm">Whale Optimization Algorithm</see>
        /// Implemented directly from <see href="https://fr.mathworks.com/matlabcentral/fileexchange/55667-the-whale-optimization-algorithm?s_tid=srchtitle">The Whale Optimization Algorithm</see>
        /// </summary>
        /// <param name="maxGenerations">max expected generations for parameter calibration</param>
        /// <param name="converterToDouble"></param>
        /// <param name="converterFromDouble"></param>
        /// <returns>A MetaHeuristic applying the WOA</returns>
        public static IMetaHeuristic WhaleOptimisationAlgorithm<TGeneValue>(int maxGenerations, Func<TGeneValue, double> converterToDouble, Func<double, TGeneValue> converterFromDouble)
        {
            
            
            var rnd = RandomizationProvider.Current;

            var updateTrackingCrossoverHeuristic = new CrossoverHeuristic()
                .WithCrossover((h,ctx) => new GeometricCrossover<TGeneValue>(2, false) //geneValues[1] is from best or random chromosome, geneValues[0] is from current parent
                    .WithGeometricOperator((IList<TGeneValue> geneValues) =>converterFromDouble(converterToDouble(geneValues[1]) - ctx.Get<double>(h, nameof(WOAParam.A)) * Math.Abs(ctx.Get<double>(h,nameof(WOAParam.C)) * converterToDouble(geneValues[1]) - converterToDouble(geneValues[0])))));

            return new IfElseMetaHeuristic()
                .WithScope(MetaHeuristicsStage.Crossover)
                .WithParameter(nameof(WOAParam.a), ParameterScope.Generation, (h,ctx) => 2.0 - ctx.Population.GenerationsNumber * (2 / maxGenerations))
                .WithParameter(nameof(WOAParam.a2), ParameterScope.Generation, (h, ctx) => 1.0 + ctx.Population.GenerationsNumber * (-1 / maxGenerations))
                .WithParameter(nameof(WOAParam.A), ParameterScope.Individual, (h, ctx) => 2 * ctx.Get<double>(h,nameof(WOAParam.a)) * rnd.GetDouble() - ctx.Get<double>(h,nameof(WOAParam.a)))
                .WithParameter(nameof(WOAParam.C), ParameterScope.Individual, (h, ctx) => 2 * rnd.GetDouble())
                .WithParameter(nameof(WOAParam.l), ParameterScope.Individual, (h, ctx) => (ctx.Get<double>(h,nameof(WOAParam.a2)) - 1) * rnd.GetDouble() + 1)
                .WithCaseGenerator((h, ctx) => rnd.GetDouble() < 0.5)
                .WithTrue(new IfElseMetaHeuristic()
                    .WithCaseGenerator((h, ctx) => Math.Abs(ctx.Get<double>(h,nameof(WOAParam.a))) > 1)
                    .WithTrue(new MatchMetaHeuristic(2)
                        .WithMatches(MatchingTechnique.Randomize)    
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic))
                    .WithFalse(new MatchMetaHeuristic(2)
                        .WithMatches(MatchingTechnique.Best)
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic)))
                .WithFalse(new MatchMetaHeuristic(2)
                    .WithMatches(MatchingTechnique.Best)
                    .WithSubMetaHeuristic(new CrossoverHeuristic()
                        .WithCrossover((h,ctx) => new GeometricCrossover<TGeneValue>(2, false)
                            .WithGeometricOperator((IList<TGeneValue> geneValues) => converterFromDouble(Math.Abs(converterToDouble(geneValues[1]) - converterToDouble(geneValues[0])) 
                                * Math.Exp(ctx.Get<double>(h,nameof(WOAParam.l))) *
                                Math.Cos(ctx.Get<double>(h,nameof(WOAParam.l)) * 2 * Math.PI)
                                + converterToDouble(geneValues[1]))))));
        }


        public static IMetaHeuristic WhaleOptimisationAlgorithmReduced<TGeneValue>(int maxGenerations, Func<TGeneValue, double> converterToDouble, Func<double, TGeneValue> converterFromDouble)
        {


            var rnd = RandomizationProvider.Current;

            var updateTrackingCrossoverHeuristic = new CrossoverHeuristic()
                .WithCrossover<CrossoverHeuristic, double, double>((h, ctx, A, C) => new GeometricCrossover<TGeneValue>(2, false) //geneValues[1] is from best or random chromosome, geneValues[0] is from current parent
                    .WithGeometricOperatorDynamic((IList<TGeneValue> geneValues) => converterFromDouble(converterToDouble(geneValues[1]) - A * Math.Abs(C * converterToDouble(geneValues[1]) - converterToDouble(geneValues[0])))));

            return new IfElseMetaHeuristic()
                .WithScope(MetaHeuristicsStage.Crossover)
                .WithParam(nameof(WOAParam.a), ParameterScope.Generation, (h, ctx) => 2.0 - ctx.Population.GenerationsNumber * (2 / maxGenerations))
                .WithParam(nameof(WOAParam.a2), ParameterScope.Generation, (h, ctx) => 1.0 + ctx.Population.GenerationsNumber * (-1 / maxGenerations))
                .WithParam<IfElseMetaHeuristic, double, double>(nameof(WOAParam.A), ParameterScope.Individual, (h, ctx, a) => 2 * a * rnd.GetDouble() - a)
                .WithParam(nameof(WOAParam.C), ParameterScope.Individual, (h, ctx) => 2 * rnd.GetDouble())
                .WithParam<IfElseMetaHeuristic, double, double>(nameof(WOAParam.l), ParameterScope.Individual, (h, ctx, a2) => (a2 - 1) * rnd.GetDouble() + 1)
                .WithCaseGenerator((h, ctx) => rnd.GetDouble() < 0.5)
                .WithTrue(new IfElseMetaHeuristic()
                    .WithCaseGenerator((h, ctx) => Math.Abs(ctx.Get<double>(h, nameof(WOAParam.a))) > 1)
                    .WithTrue(new MatchMetaHeuristic(2)
                        .WithMatches(MatchingTechnique.Randomize)
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic))
                    .WithFalse(new MatchMetaHeuristic(2)
                        .WithMatches(MatchingTechnique.Best)
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic)))
                .WithFalse(new MatchMetaHeuristic(2)
                    .WithMatches(MatchingTechnique.Best)
                    .WithSubMetaHeuristic(new CrossoverHeuristic()
                        .WithCrossover<CrossoverHeuristic, double>((h, ctx, l) => new GeometricCrossover<TGeneValue>(2, false)
                            .WithGeometricOperatorDynamic((IList<TGeneValue> geneValues) => converterFromDouble(Math.Abs(converterToDouble(geneValues[1]) - converterToDouble(geneValues[0]))
                                * Math.Exp(l) *
                                Math.Cos(l) * 2 * Math.PI
                                + converterToDouble(geneValues[1]))))));
        }


        // todo:Other good general purpose metaheuristics to implement

        //public static IMetaHeuristic FederalBureauInvestigation()
        //{
        //}

        //public static IMetaHeuristic EquilibriumOptimizer()
        //{
        //}

        //public static IMetaHeuristic SailfishOptimizer()
        //{
        //}

        //public static IMetaHeuristic SocialSkiDriverOptimization()
        //{
        //}

        //public static IMetaHeuristic SparrowSearchAlgorithm()
        //{
        //}

    }
}