using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{
    public static class MetaHeuristicsExtensions
    {
        /// <summary>
        /// This fluent helper allows to add names and description to building blocks for more clarity/maintainability
        /// </summary>
        public static T WithName<T>(this T metaHeuristic, string paramName, string paramDescription = "") where T : NamedEntity
        {
            metaHeuristic.Name = paramName;
            metaHeuristic.Description = paramDescription;
            return metaHeuristic;
        }

        /// <summary>
        /// Allows defining dynamic parameters to a Metaheuristic, to be leveraged in children operators and sub-parameters
        /// </summary>
        public static T WithParameter<T, TParamType>(this T metaHeuristic, string paramName, string paramDescription, ParamScope scope, ParameterGenerator<TParamType> generator) where T : MetaHeuristicBase
        {
            metaHeuristic.Parameters.Add(paramName, new MetaHeuristicParameter<TParamType> {Name = paramName, Description = paramDescription, Generator = generator, Scope = scope});
            return metaHeuristic;
        }

        /// <summary>
        /// Definined with a LambdaExpression, a parameter can be injected directly as a sub-epxression for global compilation if unscoped, otherwise it is checked for local scoped cached object, and the compiled expression serves as a factory
        /// </summary>
        public static T WithParam<T, TParamType>(this T metaHeuristic, string paramName, string paramDescription, ParamScope scope, Expression<ParameterGenerator<TParamType>> generator) where T : MetaHeuristicBase
        {
            metaHeuristic.Parameters.Add(paramName, new ExpressionMetaHeuristicParameter<TParamType> { Name = paramName, Description = paramDescription, DynamicGenerator = generator, Scope = scope });
            return metaHeuristic;
        }

        /// <summary>
        /// Additional arguments can be added to the parameter expression. They must be named exactly as a previously defined parameter and will be processed into a nested expression accounting for the different parameter scopes.
        /// </summary>
        public static T WithParam<T, TParamType, TArg1>(this T metaHeuristic, string paramName, string paramDescription, ParamScope scope, Expression<ParameterGenerator<TParamType, TArg1>> generator) where T : MetaHeuristicBase
        {
            metaHeuristic.Parameters.Add(paramName, new ExpressionMetaHeuristicParameter<TParamType, TArg1> { Name = paramName, Description = paramDescription, DynamicGeneratorWithArg = generator, Scope = scope });
            return metaHeuristic;
        }


        /// <summary>
        /// You can define the evolution stages to apply that Metaheuristic
        /// </summary>
        public static T WithScope<T>(this T metaHeuristic, EvolutionStage stage) where T : ScopedMetaHeuristic
        {
            metaHeuristic.Scope = stage;
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

        /// <summary>
        /// Define how input crossover probability is processed and/or passed to the suboperators
        /// </summary>
        public static T WithCrossoverProbabilityStrategy<T>(this T metaHeuristic, ProbabilityStrategy strategy) where T : ContainerMetaHeuristic
        {
            metaHeuristic.CrossoverProbabilityStrategy = strategy;
            return metaHeuristic;
        }

        /// <summary>
        /// Define how input mutation probability is processed and/or passed to the suboperators
        /// </summary>
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

        /// <summary>
        /// Defines the true case of a IfElse compound Metaheuristic
        /// </summary>
        public static T WithTrue<T>(this T metaHeuristic, IMetaHeuristic subMetaHeuristic) where T : IfElseMetaHeuristic
        {
            return WithCase(metaHeuristic, true, subMetaHeuristic);
        }

        /// <summary>
        /// Defines the false case of a IfElse compound Metaheuristic
        /// </summary>
        public static T WithFalse<T>(this T metaHeuristic, IMetaHeuristic subMetaHeuristic) where T : IfElseMetaHeuristic
        {
            return WithCase(metaHeuristic, false, subMetaHeuristic);
        }


        /// <summary>
        /// This fluent extension allows to define phase generator, that is, the function that will define which phase heuristic to run according to the context
        /// </summary>
        /// <param name="metaHeuristic">the MetaHeuristic to which to apply the fluent operator</param>
        /// <param name="scope">the scope for the case generator caching</param>
        /// <param name="phaseGenerator">the phase generator for the heuristic</param>
        /// <returns>the current phase based MetaHeuristic</returns>
        public static T WithCaseGenerator<T, TIndex>(this T metaHeuristic, ParamScope scope, ParameterGenerator<TIndex> phaseGenerator) where T : SwitchMetaHeuristic<TIndex>
        {
            metaHeuristic.DynamicParameter = new MetaHeuristicParameter<TIndex>
            {
                Name = $"{metaHeuristic.Guid}_CaseGenerator",
                Generator = phaseGenerator,
                Scope = scope
            };
          
            return metaHeuristic;
        }


        /// <summary>
        /// This fluent extension allows to define phase generator, that is, the function that will define which phase heuristic to run according to the context.
        /// This overload takes a typed Lambda expression with an extra parameter. That parameter must be registered to the context before the dynamic generator is invoked.
        /// The expression will be upgraded into a parameter-less expression leveraging the context cache or replacing the parameter by the upstream expression definition if available.
        /// </summary>
        /// <param name="metaHeuristic">the MetaHeuristic to which to apply the fluent operator</param>
        /// <param name="scope">The scope for the phase index evaluation. Accordingly, the result will be stored in context for further invocation with the same scope.</param>
        /// <param name="dynamicPhaseGenerator">An expression with an additional parameter, the name of which must corresponding to an existing context parameter defined
        /// earlier in the computational tree</param>
        /// <returns>the current phase based MetaHeuristic with the phased generator defined</returns>
        public static T WithCaseGenerator<T, TIndex, TArg1>(this T metaHeuristic, ParamScope scope, Expression<ParameterGenerator<TIndex, TArg1>> dynamicPhaseGenerator) where T : SwitchMetaHeuristic<TIndex>
        {
            metaHeuristic.DynamicParameter = new ExpressionMetaHeuristicParameter<TIndex, TArg1>
            {
                DynamicGeneratorWithArg = dynamicPhaseGenerator,
                Scope = scope
            };
            return metaHeuristic;
        }


        /// <summary>
        /// Fluent helper to defined a static selection to apply within a SelectionHeuristic
        /// </summary>
        /// <typeparam name="T">The type of the heuristic, that is inferred implicitly, without a need to type it</typeparam>
        /// <param name="metaHeuristic">the MetaHeuristic to which to apply the fluent operator</param>
        /// <param name="selection">The static selection to define</param>
        /// <returns>the currrent selection metaheuristic with a static selection defined</returns>
        public static T WithSelection<T>(this T metaHeuristic, ISelection selection) where T : SelectionHeuristic
        {
            metaHeuristic.StaticOperator = selection;
            return metaHeuristic;
        }


        public static T WithSelection<T>(this T metaHeuristic, ParameterGenerator<ISelection> dynamicOperator, ParamScope scope) where T : SelectionHeuristic
        {
            metaHeuristic.DynamicParameter = new MetaHeuristicParameter<ISelection>
            {
                    Generator = dynamicOperator,
                    Scope = scope
                }; 
            return metaHeuristic;
        }


        public static T WithCrossover<T>(this T metaHeuristic, ICrossover crossover) where T : CrossoverHeuristic
        {
            metaHeuristic.StaticOperator = crossover;
            return metaHeuristic;
        }

        public static T WithCrossover<T>(this T metaHeuristic, ParamScope scope, ParameterGenerator<ICrossover> dynamicOperator) where T : CrossoverHeuristic
        {
            metaHeuristic.DynamicParameter = new MetaHeuristicParameter<ICrossover>
            {
                Generator = dynamicOperator,
                Scope = scope
            };
            return metaHeuristic;
        }

        public static T WithCrossover<T, TArg1>(this T metaHeuristic, ParamScope scope, Expression<ParameterGenerator<ICrossover, TArg1>> dynamicOperator) where T : CrossoverHeuristic
        {
            metaHeuristic.DynamicParameter = new ExpressionMetaHeuristicParameter<ICrossover,TArg1>
            {
                DynamicGeneratorWithArg= dynamicOperator,
                Scope = scope
            };
            return metaHeuristic;
        }

        public static T WithCrossover<T, TArg1, TArg2>(this T metaHeuristic, ParamScope scope, Expression<ParameterGenerator<ICrossover, TArg1, TArg2>> dynamicOperator) where T : CrossoverHeuristic
        {
            metaHeuristic.DynamicParameter = new ExpressionMetaHeuristicParameter<ICrossover, TArg1, TArg2>
            {
                DynamicGeneratorWithArgs = dynamicOperator,
                Scope = scope
            };
            return metaHeuristic;
        }
        

        public static T WithMutation<T>(this T metaHeuristic, IMutation mutation) where T : MutationHeuristic
        {
            metaHeuristic.StaticOperator = mutation;
            return metaHeuristic;
        }

        public static T WithMutation<T>(this T metaHeuristic, ParamScope scope, ParameterGenerator<IMutation> dynamicOperator) where T : MutationHeuristic
        {
            metaHeuristic.DynamicParameter = new MetaHeuristicParameter<IMutation>
            {
                Generator = dynamicOperator,
                Scope = scope
            };
            return metaHeuristic;
        }



        public static T WithReinsertion<T>(this T metaHeuristic, IReinsertion reinsertion) where T : ReinsertionHeuristic
        {
            metaHeuristic.StaticOperator = reinsertion;
            return metaHeuristic;
        }

        public static T WithReinsertion<T>(this T metaHeuristic, ParamScope scope, ParameterGenerator<IReinsertion> dynamicOperator) where T : ReinsertionHeuristic
        {
            metaHeuristic.DynamicParameter = new MetaHeuristicParameter<IReinsertion>
            {
                Generator = dynamicOperator,
                Scope = scope
            };
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

        public static GeometricCrossover<TValue> WithGeometricOperator<TValue>(this GeometricCrossover<TValue> geometricCrossover, Func<int, IList<TValue>, TValue> geometricOperator) 
        {
            geometricCrossover.LinearGeometricOperator = geometricOperator;
            return geometricCrossover;
        }

        public static T WithGeometryEmbedding<T, TValue>(this T geometricCrossover, IGeometryEmbedding<TValue> geometryEmbedding) where T : GeometricCrossover<TValue>
        {
            geometricCrossover.GeometryEmbedding = geometryEmbedding;
            return geometricCrossover;
        }


    }
}