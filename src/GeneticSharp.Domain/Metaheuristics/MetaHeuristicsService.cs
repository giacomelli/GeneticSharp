using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Infrastructure.Framework.Reflection;

namespace GeneticSharp.Domain.Metaheuristics
{


    public enum KnownCompoundMetaheuristics
    {
        None = 0,
        Default,
        WhaleOptimisation,
        WhaleOptimisationNaive,
    }




    /// <summary>
    /// Population service.
    /// </summary>
    public static class MetaHeuristicsService<TGeneValue> where TGeneValue: IConvertible
    {
        private static readonly TypeConverter _converter = TypeDescriptor.GetConverter(typeof(TGeneValue));

        #region Methods
        /// <summary>
        /// Gets available generation strategy types.
        /// </summary>
        /// <returns>All available generation strategy types.</returns>
        public static IList<Type> GetMetaHeuristicTypes()
        {
            return TypeHelper.GetTypesByInterface<IMetaHeuristic>();
        }

        /// <summary>
        /// Gets the available generation strategy names.
        /// </summary>
        /// <returns>The generation strategy names.</returns>
        public static IList<string> GetMetaHeuristicNames()
        {
            var compoundNames = Enum.GetNames(typeof(KnownCompoundMetaheuristics));
            var typedNames =  TypeHelper.GetDisplayNamesByInterface<IMetaHeuristic>();
            return compoundNames.Union(typedNames).Distinct().ToList();
        }

        /// <summary>
        /// Creates the IGenerationStrategy's implementation with the specified name.
        /// </summary>
        /// <returns>The generation strategy implementation instance.</returns>
        /// <param name="name">The generation strategy name.</param>
        /// <param name="constructorArgs">Constructor arguments.</param>
        public static IMetaHeuristic CreateMetaHeuristicByName(string name, params object[] constructorArgs)
        {
            var compoundNames = Enum.GetNames(typeof(KnownCompoundMetaheuristics));
            if (compoundNames.Contains(name))
            {
                Enum.TryParse<KnownCompoundMetaheuristics>(name, out var enumName);
                switch (enumName)
                {
                    case KnownCompoundMetaheuristics.None:
                        return null;
                    case KnownCompoundMetaheuristics.Default:
                        return new DefaultMetaHeuristic();
                    case KnownCompoundMetaheuristics.WhaleOptimisation:
                        return MetaHeuristicsFactory.WhaleOptimisationAlgorithm<TGeneValue>(false, 500,
                            (geneIndex, geneValue) => Convert.ToDouble(geneValue),
                            (geneIndex, metricValue) =>
                            {
                                return (TGeneValue)_converter.ConvertFrom(metricValue);
                            });
                    case KnownCompoundMetaheuristics.WhaleOptimisationNaive:
                        return MetaHeuristicsFactory.WhaleOptimisationAlgorithm<object>(false, 500,
                            (geneIndex, geneValue) => Convert.ToDouble(geneValue),
                            (geneIndex, metricValue) =>
                            {
                                return (TGeneValue)_converter.ConvertFrom(metricValue);
                            },
                            bubbleNetOperator: MetaHeuristicsFactory.GetSimpleBubbleNetOperator<object>());
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return TypeHelper.CreateInstanceByName<IMetaHeuristic>(name, constructorArgs);
        }

        /// <summary>
        /// Gets the generation strategy type by the name.
        /// </summary>
        /// <returns>The generation strategy type.</returns>
        /// <param name="name">The name of generation strategy.</param>
        public static Type GetMetaHeuristicTypeByName(string name)
        {
            var compoundNames = Enum.GetNames(typeof(KnownCompoundMetaheuristics));
            if (compoundNames.Contains(name))
            {
                Enum.TryParse<KnownCompoundMetaheuristics>(name, out var enumName);
                switch (enumName)
                {
                    case KnownCompoundMetaheuristics.None:
                        return null;
                    case KnownCompoundMetaheuristics.Default:
                        return typeof(DefaultMetaHeuristic);
                    case KnownCompoundMetaheuristics.WhaleOptimisation:
                        return typeof(IfElseMetaHeuristic);
                    case KnownCompoundMetaheuristics.WhaleOptimisationNaive:
                        return typeof(IfElseMetaHeuristic);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return TypeHelper.GetTypeByName<IMetaHeuristic>(name);
        }
        #endregion
    }
}