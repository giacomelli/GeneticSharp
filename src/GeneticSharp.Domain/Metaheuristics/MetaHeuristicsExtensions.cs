using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
    public static class MetaHeuristicsExtensions
    {
        public static T WithParameter<T, TParamType>(this T metaHeuristic, string paramName, ParameterScope scope, ParameterGenerator<TParamType> generator) where T : MetaHeuristicBase
        {
            metaHeuristic.Parameters.Add(paramName, new MetaHeuristicParameter<TParamType>(){Generator = generator, Scope = scope});
            return metaHeuristic;
        }

        public static T WithScope<T>(this T metaHeuristic, MetaHeuristicsStage stage) where T : ScopedMetaHeuristic
        {
            metaHeuristic.Stage = stage;
            return metaHeuristic;
        }


        /// <summary>
        /// This fluent helper allows to define the sub metaHeuristic after the container definition
        /// </summary>
        /// <param name="metaHeuristic">the MetaHeuristic to which to apply the fluent operator</param>
        /// <param name="subMetaHeuristic">the sub metaHeuristic for the current container</param>
        /// <returns>the fluent MetaHeuristic </returns>
        public static T WithSubMetaHeuristic<T>(this T metaHeuristic, IMetaHeuristic subMetaHeuristic) where T: ContainerMetaHeuristic
        {
            metaHeuristic.SubMetaHeuristic = subMetaHeuristic;
            return metaHeuristic;
        }

        public static T WithCrossoverProbabilityStrategy<T>(this T metaHeuristic, ProbabilityStrategy strategy) where T : ContainerMetaHeuristic
        {
            metaHeuristic.CrossoverProbabilityStrategy = strategy;
            return metaHeuristic;
        }

        public static T WithMutationProbabilityStrategy<T>(this T metaHeuristic, ProbabilityStrategy strategy) where T : ContainerMetaHeuristic
        {
            metaHeuristic.MutationProbabilityStrategy = strategy;
            return metaHeuristic;
        }




        /// <summary>
        /// A fluent extension allows to define phase heuristics in sequence
        /// </summary>
        /// <param name="metaHeuristic">the MetaHeuristic to which to apply the fluent operator</param>
        /// <param name="phaseIndex">the phase index for the next phase heuristic</param>
        /// <param name="subMetaHeuristic">the phase heuristic to add</param>
        /// <returns>the current phase based Metaheuristic</returns>
        public static  T WithCase<T, TIndex>(this T metaHeuristic, TIndex phaseIndex, IMetaHeuristic subMetaHeuristic) where  T: PhaseMetaHeuristicBase<TIndex>
        {
            metaHeuristic.PhaseHeuristics.Add(phaseIndex,subMetaHeuristic);
            return metaHeuristic;
        }

        public static T WithTrue<T>(this T metaHeuristic, IMetaHeuristic subMetaHeuristic) where T : IfElseMetaHeuristic
        {
            return WithCase(metaHeuristic, true, subMetaHeuristic);
        }

        public static T WithFalse<T>(this T metaHeuristic, IMetaHeuristic subMetaHeuristic) where T : IfElseMetaHeuristic
        {
            return WithCase(metaHeuristic, false, subMetaHeuristic);
        }



        /// <summary>
        /// A fluent extension allows to define phase generator
        /// </summary>
        /// <param name="metaHeuristic">the MetaHeuristic to which to apply the fluent operator</param>
        /// <param name="phaseGenerator">the phase generator for the heuristic</param>
        /// <returns>the current phase based MetaHeuristic</returns>
        public static T WithCaseGenerator<T, TIndex>(this T metaHeuristic, ParameterGenerator<TIndex> phaseGenerator) where T : SwitchMetaHeuristic<TIndex>
        {
            metaHeuristic.IndexGenerator = phaseGenerator;
            return metaHeuristic;
        }


        public static T WithSelection<T>(this T metaHeuristic, ISelection selection) where T : SelectionHeuristic
        {
            metaHeuristic.StaticOperator = selection;
            return metaHeuristic;
        }

        public static T WithSelection<T>(this T metaHeuristic, ParameterGenerator<ISelection> dynamicOperator) where T : SelectionHeuristic
        {
            metaHeuristic.DynamicOperator = dynamicOperator;
            return metaHeuristic;
        }


        public static T WithCrossover<T>(this T metaHeuristic, ICrossover crossover) where T : CrossoverHeuristic
        {
            metaHeuristic.StaticOperator = crossover;
            return metaHeuristic;
        }

        public static T WithCrossover<T>(this T metaHeuristic, ParameterGenerator<ICrossover> dynamicOperator) where T : CrossoverHeuristic
        {
            metaHeuristic.DynamicOperator = dynamicOperator;
            return metaHeuristic;
        }

        public static T WithMutation<T>(this T metaHeuristic, IMutation mutation) where T : MutationHeuristic
        {
            metaHeuristic.StaticOperator = mutation;
            return metaHeuristic;
        }

        public static T WithMutation<T>(this T metaHeuristic, ParameterGenerator<IMutation> dynamicOperator) where T : MutationHeuristic
        {
            metaHeuristic.DynamicOperator = dynamicOperator;
            return metaHeuristic;
        }



        public static T WithReinsertion<T>(this T metaHeuristic, IReinsertion reinsertion) where T : ReinsertionHeuristic
        {
            metaHeuristic.StaticOperator = reinsertion;
            return metaHeuristic;
        }

        public static T WithReinsertion<T>(this T metaHeuristic, ParameterGenerator<IReinsertion> dynamicOperator) where T : ReinsertionHeuristic
        {
            metaHeuristic.DynamicOperator = dynamicOperator;
            return metaHeuristic;
        }

        public static T WithMatches<T>(this T metaHeuristic, params MatchingTechnique[] matchingTechnique) where T : MatchMetaHeuristic
        {
            metaHeuristic.MatchingTechniques = matchingTechnique.ToList();
            return metaHeuristic;
        }

        public static T WithSizeMetaHeuristic<T>(this T metaHeuristic, int phaseSize, IMetaHeuristic subMetaHeuristic) where T : SizeBasedMetaHeuristic
        {
            metaHeuristic.PhaseSizes.Add(phaseSize);
            metaHeuristic.PhaseHeuristics[metaHeuristic.PhaseSizes.Count - 1] = subMetaHeuristic;
            return metaHeuristic;
        }

        public static T WithSizeMetaHeuristics<T>(this T metaHeuristic, int phaseSize, int repeatNb, IMetaHeuristic subMetaHeuristic) where T : SizeBasedMetaHeuristic
        {
            for (int i = 0; i < repeatNb; i++)
            {
                metaHeuristic.PhaseSizes.Add(phaseSize);
                metaHeuristic.PhaseHeuristics[metaHeuristic.PhaseSizes.Count - 1] = subMetaHeuristic;
            }
            return metaHeuristic;
        }

        public static T WithGeometricOperator<T, TValue>(this T geometricCrossover, Func<IList<TValue>, TValue> geometricOperator) where T : GeometricCrossover<TValue>
        {
            geometricCrossover.GeometricOperator = geometricOperator;
            return geometricCrossover;
        }




    }
}