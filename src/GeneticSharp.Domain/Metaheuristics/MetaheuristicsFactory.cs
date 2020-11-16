using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Metaheuristics
{


   public static class MetaHeuristicsExtensions
    {
        public static T WithParameter<T, TParamType>(this T metaheuristic, string paramName, ParameterScope scope, ParameterGenerator<TParamType> generator) where T : MetaHeuristicBase
        {
            metaheuristic.Parameters.Add(paramName, new MetaHeuristicParameter<TParamType>(){Generator = generator, Scope = scope});
            return metaheuristic;
        }

        public static T WithScope<T>(this T metaheuristic, MetaHeuristicsScope scope) where T : ScopedMetaHeuristic
        {
            metaheuristic.Scope = scope;
            return metaheuristic;
        }


        /// <summary>
        /// This fluent helper allows to define the sub metaheuristic after the container definition
        /// </summary>
        /// <param name="metaheuristic">the MetaHeuristic to which to apply the fluent operator</param>
        /// <param name="subMetaHeuristic">the sub metaheuristic for the current container</param>
        /// <returns>the fluent MetaHeuristic </returns>
        public static T WithSubMetaHeuristic<T>(this T metaheuristic, IMetaHeuristic subMetaHeuristic) where T: ContainerMetaHeuristic
        {
            metaheuristic.SubMetaHeuristic = subMetaHeuristic;
            return metaheuristic;
        }


        /// <summary>
        /// A fluent extension allows to define phase heuristics in sequence
        /// </summary>
        /// <param name="metaheuristic">the MetaHeuristic to which to apply the fluent operator</param>
        /// <param name="phaseSize">the phase size for the next phase heuristic</param>
        /// <param name="subMetaHeuristic">the phase heuristic to add</param>
        /// <returns>the current phase based Metaheuristic</returns>
        public static  T WithPhaseMetaHeuristic<T, TIndex>(this T metaheuristic, TIndex phaseIndex, IMetaHeuristic subMetaHeuristic) where  T: PhaseMetaHeuristicBase<TIndex>
        {
            metaheuristic.PhaseHeuristics.Add(phaseIndex,subMetaHeuristic);
            return metaheuristic;
        }

        /// <summary>
        /// A fluent extension allows to define phase generator
        /// </summary>
        /// <param name="metaheuristic">the MetaHeuristic to which to apply the fluent operator</param>
        /// <param name="phaseSize">the phase size for the next phase heuristic</param>
        /// <param name="subMetaHeuristic">the phase heuristic to add</param>
        /// <returns>the current phase based Metaheuristic</returns>
        public static T WithPhaseGenerator<T, TIndex>(this T metaheuristic, ParameterGenerator<TIndex> phaseGenerator) where T : PhaseMetaHeuristic<TIndex>
        {
            metaheuristic.PhaseGenerator = phaseGenerator;
            return metaheuristic;
        }


        public static T WithCrossover<T>(this T metaheuristic, ICrossover crossover) where T : CrossoverHeuristic
        {
            metaheuristic.Crossover= crossover;
            return metaheuristic;
        }

        public static T WithDynamicCrossover<T>(this T metaheuristic, Func<IMetaHeuristicContext, ICrossover> crossoverGenerator) where T : CrossoverHeuristic
        {
            metaheuristic.DynamicCrossover = crossoverGenerator;
            return metaheuristic;
        }

        public static T WithMatches<T>(this T metaheuristic, params MatchingTechnique[] matchingTechnique) where T : MatchingMetaHeuristic
        {
            metaheuristic.MatchingTechniques = matchingTechnique.ToList();
            return metaheuristic;
        }

        public static T WithSizeMetaHeuristic<T>(this T metaheuristic, int phaseSize, IMetaHeuristic subMetaHeuristic) where T : SizeBasedMetaHeuristic
        {
            metaheuristic.PhaseSizes.Add(phaseSize);
            metaheuristic.PhaseHeuristics[metaheuristic.PhaseSizes.Count - 1] = subMetaHeuristic;
            return metaheuristic;
        }

        public static T WithSizeMetaHeuristics<T>(this T metaheuristic, int phaseSize, int repeatNb, IMetaHeuristic subMetaHeuristic) where T : SizeBasedMetaHeuristic
        {
            for (int i = 0; i < repeatNb; i++)
            {
                metaheuristic.PhaseSizes.Add(phaseSize);
                metaheuristic.PhaseHeuristics[metaheuristic.PhaseSizes.Count - 1] = subMetaHeuristic;
            }
            return metaheuristic;
        }

        public static T WithGeometricOperator<T, TValue>(this T geometricCrossover, Func<IList<TValue>, TValue> geometricOperator) where T : GeometricCrossover<TValue>
        {
            geometricCrossover.GeometricOperator = geometricOperator;
            return geometricCrossover;
        }




    }

    /// <summary>
    /// Provides a factory to generate common well known Metaheuristics
    /// </summary>
    public static class MetaheuristicsFactory
    {

        /// <summary>
        /// Implemented directly from <see href="https://fr.mathworks.com/matlabcentral/fileexchange/55667-the-whale-optimization-algorithm?s_tid=srchtitle">The Whale Optimization Algorithm</see>
        /// </summary>
        /// <param name="maxGenerations"></param>
        /// <returns></returns>
        public static IMetaHeuristic WhaleOptimisationAlgorithm(int maxGenerations)
        {
            //Declare the parameters used for better name access (note that the local variables won't be used directly because they will be thread-safe / context-dependent parameters
            double a, a2, A, C, l;
            
            var rnd = RandomizationProvider.Current;

            var updateTrackingCrossoverHeuristic = new CrossoverHeuristic()
                .WithDynamicCrossover(ctx => new GeometricCrossover<double>(2)
                    .WithGeometricOperator((IList<double> geneValues) => //geneValues[1] is from best or random chromosome, geneValues[0] is from current parent
                        geneValues[1] -
                        ctx.GetParam<double>(
                            nameof(A)) * 
                        Math.Abs(ctx.GetParam<double>(nameof(C)) * geneValues[1] - geneValues[0])));

            return new PhaseMetaHeuristic<bool>()
                .WithScope(MetaHeuristicsScope.Crossover)
                .WithParameter(nameof(a), ParameterScope.Generation,
                    ctx => 2 - ctx.Population.GenerationsNumber * (2 / maxGenerations))
                .WithParameter(nameof(a2), ParameterScope.Generation,
                    ctx => 1 + ctx.Population.GenerationsNumber * (-1 / maxGenerations))
                .WithParameter(nameof(A), ParameterScope.Individual,
                    ctx => 2 * ctx.GetParam<double>(nameof(a)) * rnd.GetDouble() - ctx.GetParam<double>(nameof(a)))
                .WithParameter(nameof(C), ParameterScope.Individual,
                    ctx => 2 * rnd.GetDouble())
                .WithParameter(nameof(l), ParameterScope.Individual,
                    ctx => (ctx.GetParam<double>(nameof(a2)) - 1) * rnd.GetDouble() + 1)
                .WithPhaseGenerator(ctx => rnd.GetDouble() < 0.5)
                .WithPhaseMetaHeuristic(true, new PhaseMetaHeuristic<bool>()
                    .WithPhaseGenerator(ctx => Math.Abs(ctx.GetParam<double>(nameof(a))) > 1)
                    .WithPhaseMetaHeuristic(true, new MatchingMetaHeuristic()
                        .WithMatches(MatchingTechnique.Randomize)    
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic))
                    .WithPhaseMetaHeuristic(false, new MatchingMetaHeuristic()
                        .WithMatches(MatchingTechnique.Best)
                        .WithSubMetaHeuristic(updateTrackingCrossoverHeuristic)))
                .WithPhaseMetaHeuristic(false, new MatchingMetaHeuristic()
                    .WithMatches(MatchingTechnique.Best)
                    .WithSubMetaHeuristic(new CrossoverHeuristic()
                        .WithDynamicCrossover(ctx => new GeometricCrossover<double>(2)
                            .WithGeometricOperator((IList<double> geneValues) =>
                                Math.Abs(geneValues[1] - geneValues[0]) *
                                Math.Exp(ctx.GetParam<double>(nameof(l))) *
                                Math.Cos(ctx.GetParam<double>(nameof(l)) * 2 * Math.PI)
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