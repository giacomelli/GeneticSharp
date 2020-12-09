using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public const double DefaultHelicoidScale = 1;


        /// <summary>
        /// As detailed in <see href="https://en.wikiversity.org/wiki/Whale_Optimization_Algorithm">Whale Optimization Algorithm</see>
        /// Implemented directly from <see href="https://fr.mathworks.com/matlabcentral/fileexchange/55667-the-whale-optimization-algorithm?s_tid=srchtitle">The Whale Optimization Algorithm</see>
        /// This is the default and faster version of the WOA algorithm with Reduced lambda expressions.
        /// </summary>
        /// <param name="ordered">specifies if the resulting Geometric Crossover operation should be made ordered to preserve gene permutations or if genes should be simply overwritten</param>
        /// <param name="maxGenerations">max expected generations for parameter calibration</param>
        /// <param name="helicoidScale">sets the amplitude of the cosine function applied in the bubblenet operator </param>
        /// <param name="geneToDoubleConverter">Converter from typed gene value to double</param>
        /// <param name="doubleToGeneConverter">Converter from double to typed gene value</param>
        /// <param name="geometryEmbedding">an optional domain specific geometrisation operator to process gene values before being converted and processed by the geometric operator and back after conversion </param>
        /// <returns>A MetaHeuristic applying the WOA</returns>
        public static IContainerMetaHeuristic WhaleOptimisationAlgorithm<TGeneValue>(bool ordered, int maxGenerations, 
            Func<TGeneValue, double> geneToDoubleConverter, Func<double, TGeneValue> doubleToGeneConverter, IGeometryEmbedding<TGeneValue> geometryEmbedding = null, double helicoidScale = DefaultHelicoidScale, bool noMutation = true)
        {
            var rnd = RandomizationProvider.Current;

            //Defining the cross operator to be applied with a random or best target, with the EncirclingPreyOperator
            var encirclingHeuristic = new CrossoverHeuristic()
                .WithCrossover(ParamScope.None, 
                    (IMetaHeuristic h, IEvolutionContext ctx, double A, double C) => new GeometricCrossover<TGeneValue>(ordered, 2, false) 
                        .WithGeometricOperator((IList<TGeneValue> geneValues) => EncirclingPreyOperator(geneValues, geneToDoubleConverter, doubleToGeneConverter, A, C))
                        .WithGeometryEmbedding(geometryEmbedding));

            //Defining the main compound Metaheuristic with sub-parts.
            var woaHeuristic = new IfElseMetaHeuristic() 
                .WithName("Whale Optimisation Algorithm", "Optimization algorithm mimicking the hunting mechanism of humpback whales in nature. Mirjalili, S., & Lewis, A. (2016)")
                .WithScope(EvolutionStage.Crossover)
                .WithParam(nameof(WOAParam.a), "a decreases linearly from 2 to 0 in Eq. (2.3)", 
                    ParamScope.Generation, (h, ctx) => 2.0 - ctx.Population.GenerationsNumber * (2.0 / maxGenerations))
                .WithParam(nameof(WOAParam.a2), "a2 linearly dicreases from -1 to -2 to calculate t in Eq. (3.12)", 
                    ParamScope.Generation, (h, ctx) => 1.0 + ctx.Population.GenerationsNumber * (-1.0 / maxGenerations))
                .WithParam(nameof(WOAParam.A), "Eq. (2.3) in the paper",
                    ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, double a) => 2.0 * a * rnd.GetDouble() - a)
                .WithParam(nameof(WOAParam.C), "Eq. (2.4) in the paper",
                    ParamScope.Generation | ParamScope.Individual, (h, ctx) => 2 * rnd.GetDouble())
                .WithParam(nameof(WOAParam.l), "parameters in Eq. (2.5)", 
                    ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, double a2) => (a2 - 1) * rnd.GetDouble() + 1.0)
                .WithCaseGenerator(ParamScope.None, (h, ctx) => (bool)(rnd.GetDouble() < 0.5))
                .WithTrue(new IfElseMetaHeuristic()
                    .WithName("Update Tracking heuristic", "Exploration phase, towards Random or Best individual")
                    .WithCaseGenerator(ParamScope.None, (IMetaHeuristic h, IEvolutionContext ctx, double a) => Math.Abs(a) > 1)
                    .WithTrue(new MatchMetaHeuristic(2)
                        .WithName("Random tracking")
                        .WithMatches(MatchingTechnique.Randomize)
                        .WithSubMetaHeuristic(encirclingHeuristic))
                    .WithFalse(new MatchMetaHeuristic(2)
                        .WithName("Best individual encircling")
                        .WithMatches(MatchingTechnique.Best)
                        .WithSubMetaHeuristic(encirclingHeuristic)))
                .WithFalse(new MatchMetaHeuristic(2)
                    .WithName("Bubble Net heuristic", "Exploitation phase, helicoidal approach")
                    .WithMatches(MatchingTechnique.Best)
                    .WithSubMetaHeuristic(new CrossoverHeuristic()
                        .WithCrossover(ParamScope.None,
                            (IMetaHeuristic h, IEvolutionContext ctx, double l) => GetBubbleNetCrossover(l, helicoidScale, ordered, geneToDoubleConverter, doubleToGeneConverter, geometryEmbedding))));

            //Removing default mutation operator 
            if (noMutation)
            {
                woaHeuristic.SubMetaHeuristic = new DefaultMetaHeuristic().WithScope(EvolutionStage.Selection | EvolutionStage.Reinsertion);
            }

            return woaHeuristic;
        }

        private static TGeneValue EncirclingPreyOperator<TGeneValue>(IList<TGeneValue> geneValues, Func<TGeneValue, double> geneToDoubleConverter, Func<double, TGeneValue> doubleToGeneConverter,
            double A, double C)
        {
            var metricValues = geneValues.Select(geneToDoubleConverter).ToList();
            var geometricValue = metricValues[1] - A * Math.Abs(C * metricValues[1] - metricValues[0]);
            var toReturn = doubleToGeneConverter(geometricValue);
            return toReturn;
        }

        private static GeometricCrossover<TGeneValue> GetBubbleNetCrossover<TGeneValue>(double l, double b, bool ordered, Func<TGeneValue, double> geneToDoubleConverter, Func<double, TGeneValue> doubleToGeneConverter, IGeometryEmbedding<TGeneValue> geometryEmbedding = null)
        {
            return new GeometricCrossover<TGeneValue>(ordered, 2, false)
                .WithGeometricOperator((IList<TGeneValue> geneValues) => BubbleNetOperator(geneValues, geneToDoubleConverter, doubleToGeneConverter, l, b))
                .WithGeometryEmbedding(geometryEmbedding);
        }

        private static TGeneValue BubbleNetOperator<TGeneValue>(IList<TGeneValue> geneValues, Func<TGeneValue, double> geneToDoubleConverter, Func<double, TGeneValue> doubleToGeneConverter, double l, double b)
        {
            var metricValues = geneValues.Select(geneToDoubleConverter).ToList();
            var geometricValue = Math.Abs(metricValues[1] - metricValues[0]) * Math.Exp(b * l) * Math.Cos(l * 2.0 * Math.PI) + metricValues[1];
            var toReturn = doubleToGeneConverter(geometricValue);
            return toReturn;
        }


        /// <summary>
        ///Alternate version with distinct parameter definitions, kept for reference
        /// </summary>
        public static IMetaHeuristic WhaleOptimisationAlgorithmWithParams<TGeneValue>(bool ordered, int maxGenerations, Func<TGeneValue, double> fromGeneConverter, Func<double, TGeneValue> toGeneConverter)
        {
            
            
            var rnd = RandomizationProvider.Current;

            var updateTrackingCrossoverHeuristic = new CrossoverHeuristic()
                .WithCrossover(ParamScope.None, (h,ctx) => new GeometricCrossover<TGeneValue>(ordered, 2, false) //geneValues[1] is from best or random chromosome, geneValues[0] is from current parent
                    .WithGeometricOperator((IList<TGeneValue> geneValues) =>toGeneConverter(fromGeneConverter(geneValues[1]) - ctx.GetParam<double>(h, nameof(WOAParam.A)) * Math.Abs(ctx.GetParam<double>(h,nameof(WOAParam.C)) * fromGeneConverter(geneValues[1]) - fromGeneConverter(geneValues[0])))));

            return new IfElseMetaHeuristic()
                .WithScope(EvolutionStage.Crossover)
                .WithParameter(nameof(WOAParam.a), "a decreases linearly from 2 to 0 in Eq. (2.3)", ParamScope.Generation, (h,ctx) => 2.0 - ctx.Population.GenerationsNumber * (2.0 / maxGenerations))
                .WithParameter(nameof(WOAParam.a2), "a2 linearly dicreases from -1 to -2 to calculate t in Eq. (3.12)", ParamScope.Generation, (h, ctx) => 1.0 + ctx.Population.GenerationsNumber * (-1.0 / maxGenerations))
                .WithParameter(nameof(WOAParam.A), "Eq. (2.3) in the paper", ParamScope.Generation | ParamScope.Individual, (h, ctx) => 2.0 * ctx.GetParam<double>(h,nameof(WOAParam.a)) * rnd.GetDouble() - ctx.GetParam<double>(h,nameof(WOAParam.a)))
                .WithParameter(nameof(WOAParam.C), "Eq. (2.4) in the paper", ParamScope.Generation | ParamScope.Individual, (h, ctx) => 2.0 * rnd.GetDouble())
                .WithParameter(nameof(WOAParam.l), "parameters in Eq. (2.5)", ParamScope.Generation | ParamScope.Individual, (h, ctx) => (ctx.GetParam<double>(h,nameof(WOAParam.a2)) - 1.0) * rnd.GetDouble() + 1.0)
                .WithCaseGenerator(ParamScope.None,(h, ctx) => rnd.GetDouble() < 0.5)
                .WithTrue(new IfElseMetaHeuristic()
                    .WithCaseGenerator(ParamScope.None, (h, ctx) => Math.Abs(ctx.GetParam<double>(h,nameof(WOAParam.a))) > 1)
                    .WithTrue(new MatchMetaHeuristic(2)
                        .WithMatches(MatchingTechnique.Randomize)    
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic))
                    .WithFalse(new MatchMetaHeuristic(2)
                        .WithMatches(MatchingTechnique.Best)
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic)))
                .WithFalse(new MatchMetaHeuristic(2)
                    .WithMatches(MatchingTechnique.Best)
                    .WithSubMetaHeuristic(new CrossoverHeuristic()
                        .WithCrossover(ParamScope.None, (h,ctx) => new GeometricCrossover<TGeneValue>(ordered,2, false)
                            .WithGeometricOperator((IList<TGeneValue> geneValues) => toGeneConverter(Math.Abs(fromGeneConverter(geneValues[1]) - fromGeneConverter(geneValues[0])) 
                                * Math.Exp(ctx.GetParam<double>(h,nameof(WOAParam.l))) *
                                Math.Cos(ctx.GetParam<double>(h,nameof(WOAParam.l)) * 2 * Math.PI)
                                + fromGeneConverter(geneValues[1]))))));
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