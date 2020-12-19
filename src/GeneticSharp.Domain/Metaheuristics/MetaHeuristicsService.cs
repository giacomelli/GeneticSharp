using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Infrastructure.Framework.Reflection;

namespace GeneticSharp.Domain.Metaheuristics
{


    public enum KnownCompoundMetaheuristics
    {
        None = 0,
        Default,
        DefaultRandomHyperspeed,
        WhaleOptimisation,
        WhaleOptimisationNaive,
    }




    /// <summary>
    /// Population service.
    /// </summary>
    public static class MetaHeuristicsService<TGeneValue> //where TGeneValue: IConvertible
    {
        

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
        public static IMetaHeuristic CreateMetaHeuristicByName(string name, Func<int, TGeneValue, double> geneToDoubleConverter, Func<int, double, TGeneValue> doubleToGeneConverter, IGeometryEmbedding<TGeneValue> geometryEmbedding = null)
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
                    case KnownCompoundMetaheuristics.DefaultRandomHyperspeed:
                        var toReturn = new DefaultMetaHeuristic();
                        toReturn.MatchMetaHeuristic.MatchingTechniques[0] = MatchingTechnique.Randomize;
                        return toReturn;
                    case KnownCompoundMetaheuristics.WhaleOptimisation:
                        if (geneToDoubleConverter == null)
                        {
                            geneToDoubleConverter = MetaHeuristicsFactory.GetDefaultGeneConverter<TGeneValue>().GeneToDouble;
                        }
                        if (doubleToGeneConverter == null)
                        {
                            doubleToGeneConverter = MetaHeuristicsFactory.GetDefaultGeneConverter<TGeneValue>().DoubleToGene;
                        }
                        return MetaHeuristicsFactory.WhaleOptimisationAlgorithm<TGeneValue>(false, 500,
                            geneToDoubleConverter, doubleToGeneConverter, geometryEmbedding);
                    case KnownCompoundMetaheuristics.WhaleOptimisationNaive:
                        if (geneToDoubleConverter == null)
                        {
                            geneToDoubleConverter = MetaHeuristicsFactory.GetDefaultGeneConverter<TGeneValue>().GeneToDouble;
                        }
                        if (doubleToGeneConverter == null)
                        {
                            doubleToGeneConverter = MetaHeuristicsFactory.GetDefaultGeneConverter<TGeneValue>().DoubleToGene;
                        }
                        return MetaHeuristicsFactory.WhaleOptimisationAlgorithmExtended<TGeneValue>(false, 500, geneToDoubleConverter, doubleToGeneConverter, geometryEmbedding, bubbleNetOperator: MetaHeuristicsFactory.GetSimpleBubbleNetOperator<TGeneValue>());
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return TypeHelper.CreateInstanceByName<IMetaHeuristic>(name);
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
                    case KnownCompoundMetaheuristics.DefaultRandomHyperspeed:
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