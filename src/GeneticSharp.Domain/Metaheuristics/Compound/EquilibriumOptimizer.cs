using System;
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
    ///  As detailed in <see href="https://www.afshinfaramarzi.com/equilibrium-optimizer">Equilibrium Optimizer</see>
    /// Implemented directly from <see href="https://github.com/thieu1995/mealpy/blob/master/mealpy/physics_based/EO.py">Mealpy version</see>
    /// This is the default version of the Equilibrium Optimizer algorithm with Reduced lambda expressions.
    /// </summary>
    public class EquilibriumOptimizer : GeometricMetaHeuristicBase
    {

        //Declare the parameters used for better name access (note that the local variables won't be used directly because they will be thread-safe / context-dependent parameters
        private enum EOParam
        {
            pbSize,
            t,
            lambda,
            r,
            f,
            r1,
            r2,
            gcp,
        }

        private static object DefaultEquilibriumOperator(int geneIndex, IEnumerable<object> geneValues, IGeometricConverter geometricConverter, IList<double> gcp, double[] lambda, double[] f, double V)
        {
            var metricValues = geneValues.Select(value => geometricConverter.GeneToDouble(geneIndex, value)).ToList();
            var g0 = gcp[geneIndex] * (metricValues[1] - lambda[geneIndex] * metricValues[0]);
            var g = f[geneIndex] * g0;
            var geometricValue = metricValues[1] + f[geneIndex] * (metricValues[0] - metricValues[1]) +
                                 (g * V / lambda[geneIndex]) * (1.0 - f[geneIndex]);
            var toReturn = geometricConverter.DoubleToGene(geneIndex, geometricValue);
            return toReturn;
        }

        private static double GetTime(double a2, IEvolutionContext ctx, int maxGenerations)
        {
            var t = Math.Pow(
                Math.Max(0,
                    1.0 - ((double)ctx.Population.GenerationsNumber / maxGenerations)),
                a2 * ((double)ctx.Population.GenerationsNumber / maxGenerations));
            //if (double.IsNaN(t))
            //{
            //    Debugger.Break();
            //}
            return t;
        }


        private static double[] GetExponentialRate(double a1, double t, double[] lambda, double[] r)
        {
            var f = lambda.Select((lambdai, i) => a1 * Math.Sign(r[i] - 0.5) * (Math.Exp(-lambdai * t) - 1.0)).ToArray();
            return f;
        }

        private static double[] GetGCP(int pbSize, double r1, double r2, double gp)
        {
            var gcp = r2 > gp
                ? Enumerable.Repeat(r1 * 0.5, pbSize).ToArray()
                : Enumerable.Repeat(0.0, pbSize).ToArray();
            return gcp;
        }



        public override IContainerMetaHeuristic Build()
        {
            var rnd = RandomizationProvider.Current;


            var V = 1.0;
            var a1 = 2.0;
            var a2 = 1.0;
            var GP = 0.5;


            //Defining the cross operator to be applied to generate centroid from 4 best chromosomes
            var centroidHeuristic = new CrossoverHeuristic()
                .WithCrossover(ParamScope.None,
                    (IMetaHeuristic h, IEvolutionContext ctx) => new GeometricCrossover<object>(GeometricConverter.IsOrdered, 4, false)
                        .WithGeometryEmbedding(GeometricConverter.GetEmbedding()));



            var generationHeuristic = new CrossoverHeuristic()
                .WithCrossover(ParamScope.None,
                    (IMetaHeuristic h, IEvolutionContext ctx, IList<double> gcp, double[] lambda, double[] f) => new GeometricCrossover<object>(GeometricConverter.IsOrdered, 2, false)
                        .WithLinearGeometricOperator((geneIndex, geneValues) => DefaultEquilibriumOperator(geneIndex, geneValues, GeometricConverter, gcp, lambda, f, V))
                        .WithGeometryEmbedding(GeometricConverter.GetEmbedding()));

            //Defining the main compound Metaheuristic with sub-parts.
            var eoHeuristic = new MatchMetaHeuristic()
                .WithName("Equilibrium Optimizer", "Optimization algorithm, inspired by control volume mass balance to estimate both dynamic and equilibrium states. A. Faramarzi, M. Heidarinejad, B. Stephens, S. Mirjalili (2019)")
                
                .WithParam(nameof(EOParam.pbSize), "Used in various places", ParamScope.Evolution, (h, ctx) => ctx.Population.CurrentGeneration.Chromosomes[0].GetGenes().Length)
                .WithParam(nameof(EOParam.t), "# Eq. 9", ParamScope.Generation, (h, ctx) => GetTime(a2, ctx, MaxGenerations))
                .WithParam(nameof(EOParam.lambda), "lambda in Eq. 11",
                    ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, int pbSize) => Enumerable.Range(0, pbSize).Select(i => rnd.GetDouble()).ToArray())
                .WithParam(nameof(EOParam.r), "r in Eq. 11",
                    ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, int pbSize) => Enumerable.Range(0, pbSize).Select(i => rnd.GetDouble()).ToArray())
                .WithParam(nameof(EOParam.f), "Eq. 11",
                    ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, double t, double[] lambda, double[] r) => GetExponentialRate(a1, t, lambda, r))
                .WithParam(nameof(EOParam.r1), "r1 in Eq. 15", ParamScope.Generation | ParamScope.Individual, (h, ctx) => rnd.GetDouble())
                .WithParam(nameof(EOParam.r2), "r2 in Eq. 15", ParamScope.Generation | ParamScope.Individual, (h, ctx) => rnd.GetDouble())
                //.WithParam(nameof(EOParam.gcp), "Eq. 15",
                //    ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, int pbSize, double r1, double r2) => r2 > GP ? Enumerable.Repeat(r1 * 0.5, pbSize).ToArray() : Enumerable.Repeat(0.0, pbSize).ToArray())
                .WithParam(nameof(EOParam.gcp), "Eq. 15",
                    ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, int pbSize, double r1, double r2) => GetGCP(pbSize, r1, r2, GP))
                //Custom pick is random from 4 best + centroid
                .WithMatches(MatchingKind.Current, MatchingKind.Custom)
                //4 best = 1 + 3 additionals
                .WithCustomMatchStep(new[] { new MatchingSettings() { MatchingKind = MatchingKind.Best, AdditionalPicks = 3, CachingScope = ParamScope.Generation } })
                // 4 best + centroid = 1 current + (3 = (1+2)) neighbours + 1 child centroid
                .WithCustomMatchStep(new[]
                {
                    new MatchingSettings() { MatchingKind = MatchingKind.Current},
                    new MatchingSettings() { MatchingKind = MatchingKind.Neighbor, AdditionalPicks = 2},
                    new MatchingSettings() { MatchingKind = MatchingKind.Child, CachingScope = ParamScope.Generation},
                })
                .WithChildMetaHeuristic(centroidHeuristic)
                .WithCustomMatchStep(new[] { new MatchingSettings() { MatchingKind = MatchingKind.Random } })
                .WithSubMetaHeuristic(generationHeuristic);

            //Removing default mutation operator 
            if (NoMutation)
            {
                //Note that generationHeuristic's SubMetaHeuristic is used  instead of the root MatchMetaHeuristc the SubMetaHeuristic of which is the main operator 
                generationHeuristic.SubMetaHeuristic = new DefaultMetaHeuristic().WithScope(EvolutionStage.Selection | EvolutionStage.Crossover | EvolutionStage.Reinsertion).WithName("Not Mutation Heuristic");
            }

            //Enforcing FitnessBasedElitistReinsertion
            if (SetDefaultReinsertion)
            {
                var subHeuristic = generationHeuristic.SubMetaHeuristic;
                generationHeuristic.SubMetaHeuristic = new ReinsertionHeuristic()
                    { StaticOperator = new FitnessBasedElitistReinsertion(), SubMetaHeuristic = subHeuristic }.WithName("Forced Fitness Elitist Reinsertion Heuristic");
            }


           


            return eoHeuristic;
        }
    }


    // todo:Other good general purpose metaheuristics to implement

    // public class SailfishOptimizer : GeometricMetaHeuristicBase
    //{
    //}

    // public class SocialSkiDriverOptimization : GeometricMetaHeuristicBase
    //{
    //}

    // public class SparrowSearchAlgorithm : GeometricMetaHeuristicBase
    //{
    //}



}