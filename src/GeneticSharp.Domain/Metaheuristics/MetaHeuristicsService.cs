﻿using System;
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
    public static class MetaHeuristicsService //where TGeneValue: IConvertible
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
            //var typedNames =  TypeHelper.GetDisplayNamesByInterface<IMetaHeuristic>();
            //return compoundNames.Union(typedNames).Distinct().ToList();
            return compoundNames;
        }

        /// <summary>
        /// Creates the IGenerationStrategy's implementation with the specified name.
        /// </summary>
        /// <returns>The generation strategy implementation instance.</returns>
        /// <param name="name">The generation strategy name.</param>
        /// <param name="constructorArgs">Constructor arguments.</param>
        public static IMetaHeuristic CreateMetaHeuristicByName(string name, IGeometricConverter geometricConverter = null)
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
                        if (geometricConverter == null)
                        {
                            geometricConverter = new DefaultGeometricConverter();
                        }
                        return MetaHeuristicsFactory.WhaleOptimisationAlgorithm(geometricConverter.IsOrdered, 1000, geometricConverter);
                    case KnownCompoundMetaheuristics.WhaleOptimisationNaive:
                        if (geometricConverter == null)
                        {
                            geometricConverter = new DefaultGeometricConverter();
                        }
                        return MetaHeuristicsFactory.WhaleOptimisationAlgorithmExtended(geometricConverter.IsOrdered, 1000, geometricConverter, bubbleNetOperator: MetaHeuristicsFactory.GetSimpleBubbleNetOperator());
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