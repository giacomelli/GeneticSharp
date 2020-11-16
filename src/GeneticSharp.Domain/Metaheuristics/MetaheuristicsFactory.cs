using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Provides a factory to generate common well known Metaheuristics
    /// </summary>
    public static class MetaHeuristicsFactory
    {

        /// <summary>
        /// As detailed in <see href="https://en.wikiversity.org/wiki/Whale_Optimization_Algorithm">Whale Optimization Algorithm</see>
        /// Implemented directly from <see href="https://fr.mathworks.com/matlabcentral/fileexchange/55667-the-whale-optimization-algorithm?s_tid=srchtitle">The Whale Optimization Algorithm</see>
        /// </summary>
        /// <param name="maxGenerations">max expected generations for parameter calibration</param>
        /// <returns>A MetaHeuristic applying the WOA</returns>
        public static IMetaHeuristic WhaleOptimisationAlgorithm(int maxGenerations)
        {
            //Declare the parameters used for better name access (note that the local variables won't be used directly because they will be thread-safe / context-dependent parameters
            double a, a2, A, C, l;
            
            var rnd = RandomizationProvider.Current;

            var updateTrackingCrossoverHeuristic = new CrossoverHeuristic()
                .WithCrossover((h,ctx) => new GeometricCrossover<double>(2) //geneValues[1] is from best or random chromosome, geneValues[0] is from current parent
                    .WithGeometricOperator((IList<double> geneValues) => geneValues[1] - ctx.Get<double>(h, nameof(A)) * Math.Abs(ctx.Get<double>(h,nameof(C)) * geneValues[1] - geneValues[0])));

            return new IfElseMetaHeuristic()
                .WithScope(MetaHeuristicsStage.Crossover)
                .WithParameter(nameof(a), ParameterScope.Generation, (h,ctx) => 2 - ctx.Population.GenerationsNumber * (2 / maxGenerations))
                .WithParameter(nameof(a2), ParameterScope.Generation, (h, ctx) => 1 + ctx.Population.GenerationsNumber * (-1 / maxGenerations))
                .WithParameter(nameof(A), ParameterScope.Individual, (h, ctx) => 2 * ctx.Get<double>(h,nameof(a)) * rnd.GetDouble() - ctx.Get<double>(h,nameof(a)))
                .WithParameter(nameof(C), ParameterScope.Individual, (h, ctx) => 2 * rnd.GetDouble())
                .WithParameter(nameof(l), ParameterScope.Individual, (h, ctx) => (ctx.Get<double>(h,nameof(a2)) - 1) * rnd.GetDouble() + 1)
                .WithCaseGenerator((h, ctx) => rnd.GetDouble() < 0.5)
                .WithTrue(new IfElseMetaHeuristic()
                    .WithCaseGenerator((h, ctx) => Math.Abs(ctx.Get<double>(h,nameof(a))) > 1)
                    .WithTrue(new MatchMetaHeuristic()
                        .WithMatches(MatchingTechnique.Randomize)    
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic))
                    .WithFalse(new MatchMetaHeuristic()
                        .WithMatches(MatchingTechnique.Best)
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic)))
                .WithFalse(new MatchMetaHeuristic()
                    .WithMatches(MatchingTechnique.Best)
                    .WithSubMetaHeuristic(new CrossoverHeuristic()
                        .WithCrossover((h,ctx) => new GeometricCrossover<double>(2)
                            .WithGeometricOperator((IList<double> geneValues) => Math.Abs(geneValues[1] - geneValues[0]) 
                                * Math.Exp(ctx.Get<double>(h,nameof(l))) *
                                Math.Cos(ctx.Get<double>(h,nameof(l)) * 2 * Math.PI)
                                + geneValues[1]))));
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