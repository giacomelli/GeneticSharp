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
    ///   As detailed in <see href="https://en.wikiversity.org/wiki/Whale_Optimization_Algorithm">Whale Optimization Algorithm</see>
    /// Implemented directly from <see href="https://fr.mathworks.com/matlabcentral/fileexchange/55667-the-whale-optimization-algorithm?s_tid=srchtitle">The Whale Optimization Algorithm</see>
    /// This is the default and faster version of the WhaleOptimisation algorithm with Reduced lambda expressions.
    /// </summary>
    public class WhaleOptimisationAlgorithm : GeometricMetaHeuristicBase
    {

        //Declare the parameters used for better name access (note that the local variables won't be used directly because they will be thread-safe / context-dependent parameters
        private enum WoaParam
        {
            a,
            a2,
            A,
            C,
            l
        }

        // WOA with dynamic operators

        public delegate object EncirclingPreyOperator(int geneIndex, IEnumerable<object> geneValues, IGeometricConverter geometricConverter, double A, double C);

        public delegate object BubbleNetOperator(int geneIndex, IEnumerable<object> geneValues, IGeometricConverter geometricConverter, double l, double b);


        public const double DefaultHelicoidScale = 1;

        /// <summary>
        /// Defines the default linear operator performing the encircling strategy, to be computed for each gene, converted to a double through the provided converter
        /// </summary>
        private static object DefaultEncirclingPreyOperator(int geneIndex, IEnumerable<object> geneValues, IGeometricConverter geometricConverter,
            double A, double C)
        {
            var metricValues = geneValues.Select(value => geometricConverter.GeneToDouble(geneIndex, value)).ToList();
            var geometricValue = metricValues[1] - A * Math.Abs(C * metricValues[1] - metricValues[0]);
            var toReturn = geometricConverter.DoubleToGene(geneIndex, geometricValue);
            return toReturn;
        }

        private static object DefaultBubbleNetOperator(int geneIndex, IEnumerable<object> geneValues, IGeometricConverter geometricConverter, double l, double b)
        {
            var metricValues = geneValues.Select(value => geometricConverter.GeneToDouble(geneIndex, value)).ToList();
            var geometricValue = Math.Abs(metricValues[1] - metricValues[0]) * Math.Exp(b * l) * Math.Cos(l * 2.0 * Math.PI) + metricValues[1];
            var toReturn = geometricConverter.DoubleToGene(geneIndex, geometricValue);
            return toReturn;
        }



        /// <summary>
        /// sets the amplitude of the cosine function applied in the bubblenet operator
        /// </summary>
        public double HelicoidScale { get; set; } = DefaultHelicoidScale;

        


        /// <summary>
        /// The bubblenet operator defines the specific attack behavior with spiral updating
        /// </summary>
        public BubbleNetOperator BubbleOperator { get; set; } = DefaultBubbleNetOperator;

        /// <summary>
        /// The encircling operator defines how an individual gets closer to a target individual
        /// </summary>
        public EncirclingPreyOperator EncirclingOperator { get; set; } = DefaultEncirclingPreyOperator;


        /// <inheritdoc />
        public override IContainerMetaHeuristic Build()
        {
            var rnd = RandomizationProvider.Current;

            //Defining the cross operator to be applied with a random or best target, with the Encircling Prey Operator
            var encirclingHeuristic = new CrossoverHeuristic()
                .WithName("encircling crossover")
                .WithCrossover(ParamScope.None,
                    (IMetaHeuristic h, IEvolutionContext ctx, double A, double C) => new GeometricCrossover<object>(GeometricConverter.IsOrdered, 2, false)
                        .WithLinearGeometricOperator((geneIndex, geneValues) => EncirclingOperator(geneIndex, geneValues, GeometricConverter, A, C))
                        .WithGeometryEmbedding(GeometricConverter.GetEmbedding()));

            //Defining the main compound Metaheuristic with sub-parts.
            var woaHeuristic = new IfElseMetaHeuristic()
                .WithName("Whale Optimisation Algorithm", "Optimization algorithm mimicking the hunting mechanism of humpback whales in nature. Mirjalili, S., & Lewis, A. (2016)")
                .WithScope(EvolutionStage.Crossover)
                .WithParam(nameof(WoaParam.a), "a decreases linearly from 2 to 0 in Eq. (2.3)",
                    ParamScope.Generation, (h, ctx) => 2.0 - ctx.Population.GenerationsNumber * (2.0 / MaxGenerations))
                .WithParam(nameof(WoaParam.a2), "a2 linearly dicreases from -1 to -2 to calculate t in Eq. (3.12)",
                    ParamScope.Generation, (h, ctx) => -1.0 + ctx.Population.GenerationsNumber * (-1.0 / MaxGenerations))
                .WithParam(nameof(WoaParam.A), "Eq. (2.3) in the paper",
                    ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, double a) => 2.0 * a * rnd.GetDouble() - a)
                .WithParam(nameof(WoaParam.C), "Eq. (2.4) in the paper",
                    ParamScope.Generation | ParamScope.Individual, (h, ctx) => 2 * rnd.GetDouble())
                .WithParam(nameof(WoaParam.l), "parameters in Eq. (2.5)",
                    ParamScope.Generation | ParamScope.Individual, (IMetaHeuristic h, IEvolutionContext ctx, double a2) => (a2 - 1) * rnd.GetDouble() + 1.0)
                .WithCaseGenerator(ParamScope.None, (h, ctx) => rnd.GetDouble() < 0.5)
                .WithTrue(new IfElseMetaHeuristic()
                    .WithName("Update Tracking heuristic", "Exploration phase, towards Random or Best individual")
                    .WithCaseGenerator(ParamScope.Generation, (IMetaHeuristic h, IEvolutionContext ctx, double a) => Math.Abs(a) > 1)
                    .WithTrue(new MatchMetaHeuristic()
                        .WithName("Random tracking")
                        .WithMatches(MatchingKind.Current, MatchingKind.Random)
                        .WithSubMetaHeuristic(encirclingHeuristic))
                    .WithFalse(new MatchMetaHeuristic()
                        .WithName("Best individual encircling")
                        .WithMatches(MatchingKind.Current, MatchingKind.Best)
                        .WithSubMetaHeuristic(encirclingHeuristic)))
                .WithFalse(new MatchMetaHeuristic()
                    .WithName("Bubble Net heuristic", "Exploitation phase, helicoidal approach")
                    .WithMatches(MatchingKind.Current, MatchingKind.Best)
                    .WithSubMetaHeuristic(new CrossoverHeuristic()
                        .WithName("Bubble Net crossover")
                        .WithCrossover(ParamScope.None,
                            (IMetaHeuristic h, IEvolutionContext ctx, double l) => new GeometricCrossover<object>(GeometricConverter.IsOrdered, 2, false)
                                .WithLinearGeometricOperator((geneIndex, geneValues) => BubbleOperator(geneIndex, geneValues, GeometricConverter, l, HelicoidScale))
                                .WithGeometryEmbedding(GeometricConverter.GetEmbedding()))));

            //Removing default mutation operator 
            if (NoMutation)
            {
                woaHeuristic.SubMetaHeuristic = new DefaultMetaHeuristic().WithScope(EvolutionStage.Selection | EvolutionStage.Reinsertion).WithName("Not Mutation Heuristic");
            }

            //Enforcing pure reinsertion
            var subHeuristic = woaHeuristic.SubMetaHeuristic;
            woaHeuristic.SubMetaHeuristic = new ReinsertionHeuristic()
                { StaticOperator = new PureReinsertion(), SubMetaHeuristic = subHeuristic }.WithName("Forced Pure Reinsertion Heuristic");

            return woaHeuristic;
        }

        /// <summary>
        ///Alternate version with distinct parameter definitions, kept for reference
        /// </summary>
        public IContainerMetaHeuristic CreateWithParams()
        {
            var rnd = RandomizationProvider.Current;

            var updateTrackingCrossoverHeuristic = new CrossoverHeuristic()
                .WithCrossover(ParamScope.None, (h, ctx) => new GeometricCrossover<object>(GeometricConverter.IsOrdered, 2, false) //geneValues[1] is from best or random chromosome, geneValues[0] is from current parent
                    .WithLinearGeometricOperator((geneIndex, geneValues) => GeometricConverter.DoubleToGene(geneIndex, GeometricConverter.GeneToDouble(geneIndex, geneValues[1]) - ctx.GetParam<double>(h, nameof(WoaParam.A)) * Math.Abs(ctx.GetParam<double>(h, nameof(WoaParam.C)) * GeometricConverter.GeneToDouble(geneIndex, geneValues[1]) - GeometricConverter.GeneToDouble(geneIndex, geneValues[0])))));

            return new IfElseMetaHeuristic()
                .WithScope(EvolutionStage.Crossover)
                .WithParameter(nameof(WoaParam.a), "a decreases linearly from 2 to 0 in Eq. (2.3)", ParamScope.Generation, (h, ctx) => 2.0 - ctx.Population.GenerationsNumber * (2.0 / MaxGenerations))
                .WithParameter(nameof(WoaParam.a2), "a2 linearly decreases from -1 to -2 to calculate t in Eq. (3.12)", ParamScope.Generation, (h, ctx) => -1.0 + ctx.Population.GenerationsNumber * (-1.0 / MaxGenerations))
                .WithParameter(nameof(WoaParam.A), "Eq. (2.3) in the paper", ParamScope.Generation | ParamScope.Individual, (h, ctx) => 2.0 * ctx.GetParam<double>(h, nameof(WoaParam.a)) * rnd.GetDouble() - ctx.GetParam<double>(h, nameof(WoaParam.a)))
                .WithParameter(nameof(WoaParam.C), "Eq. (2.4) in the paper", ParamScope.Generation | ParamScope.Individual, (h, ctx) => 2.0 * rnd.GetDouble())
                .WithParameter(nameof(WoaParam.l), "parameters in Eq. (2.5)", ParamScope.Generation | ParamScope.Individual, (h, ctx) => (ctx.GetParam<double>(h, nameof(WoaParam.a2)) - 1.0) * rnd.GetDouble() + 1.0)
                .WithCaseGenerator(ParamScope.None, (h, ctx) => rnd.GetDouble() < 0.5)
                .WithTrue(new IfElseMetaHeuristic()
                    .WithCaseGenerator(ParamScope.Generation, (h, ctx) => Math.Abs(ctx.GetParam<double>(h, nameof(WoaParam.a))) > 1)
                    .WithTrue(new MatchMetaHeuristic()
                        .WithMatches(MatchingKind.Current, MatchingKind.Random)
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic))
                    .WithFalse(new MatchMetaHeuristic()
                        .WithMatches(MatchingKind.Current, MatchingKind.Best)
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic)))
                .WithFalse(new MatchMetaHeuristic()
                    .WithMatches(MatchingKind.Current, MatchingKind.Best)
                    .WithSubMetaHeuristic(new CrossoverHeuristic()
                        .WithCrossover(ParamScope.None, (h, ctx) => new GeometricCrossover<object>(GeometricConverter.IsOrdered, 2, false)
                            .WithLinearGeometricOperator((geneIndex, geneValues) => GeometricConverter.DoubleToGene(geneIndex, Math.Abs(GeometricConverter.GeneToDouble(geneIndex, geneValues[1]) - GeometricConverter.GeneToDouble(geneIndex, geneValues[0]))
                                                                                   * Math.Exp(ctx.GetParam<double>(h, nameof(WoaParam.l))) *
                                                                                   Math.Cos(ctx.GetParam<double>(h, nameof(WoaParam.l)) * 2 * Math.PI)
                                                                                   + GeometricConverter.GeneToDouble(geneIndex, geneValues[1]))))));
        }


       


        public static BubbleNetOperator GetSimpleBubbleNetOperator(double mixCoef = 0.5)
        {
            return (geneIndex, geneValues, geometricConverter, l, b) =>
            {
                var metricValues = geneValues.Select(value => geometricConverter.GeneToDouble(geneIndex, value)).ToList();
                var geometricValue = mixCoef * metricValues[0] + (1 - mixCoef) * metricValues[1];
                var toReturn = geometricConverter.DoubleToGene(geneIndex, geometricValue);
                return toReturn;
            };
        }


    }
}