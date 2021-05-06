using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Metaheuristics.Compound;
using GeneticSharp.Domain.Metaheuristics.Matching;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Infrastructure.Framework.Reflection;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Population service.
    /// </summary>
    public static class MetaHeuristicsService 
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
            //var typedNames =  TypeHelper.GetDisplayNamesByInterface<IMetaHeuristic>();
            //return compoundNames.Union(typedNames).Distinct().ToList();
            return compoundNames;
        }

        private static string[] compoundNames = Enum.GetNames(typeof(KnownCompoundMetaheuristics));

        /// <summary>
        /// Creates the IGenerationStrategy's implementation with the specified name.
        /// </summary>
        /// <returns>The generation strategy implementation instance.</returns>
        /// <param name="name">The generation strategy name.</param>
        /// <param name="geometricConverter">a parameter that defines how geometric operators should be applied,
        /// that is how to do transformations between gene space and metric space</param>
        public static IMetaHeuristic CreateMetaHeuristicByName(string name, int maxGenerations = 1000, int populationSize = 100, IGeometricConverter geometricConverter = null, bool noMutation = true)
        {
            
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
                        toReturn.MatchMetaHeuristic.Picker.MatchPicks[1] = new MatchingSettings(){MatchingKind = MatchingKind.Random};
                        toReturn.MatchMetaHeuristic.EnableHyperSpeed = true;
                        return toReturn;
                    case KnownCompoundMetaheuristics.WhaleOptimisation:
                    case KnownCompoundMetaheuristics.WhaleOptimisationNaive:
                        if (geometricConverter == null)
                        {
                            geometricConverter = new DefaultGeometricConverter();
                        }
                        var woa = new WhaleOptimisationAlgorithm()
                        {
                            MaxGenerations = maxGenerations, GeometricConverter = geometricConverter,
                            NoMutation = noMutation
                        };
                        if (enumName == KnownCompoundMetaheuristics.WhaleOptimisationNaive)
                        {
                            woa.BubbleOperator = WhaleOptimisationAlgorithm.GetSimpleBubbleNetOperator();
                        }
                        return woa.Build();
                    case KnownCompoundMetaheuristics.EquilibriumOptimizer:
                        if (geometricConverter == null)
                        {
                            geometricConverter = new DefaultGeometricConverter();
                        }
                        var eo = new EquilibriumOptimizer()
                        {
                            MaxGenerations = maxGenerations,
                            GeometricConverter = geometricConverter,
                            NoMutation = noMutation
                        };
                        return eo.Build();
                    case KnownCompoundMetaheuristics.ForensicBasedInvestigation:
                        if (geometricConverter == null)
                        {
                            geometricConverter = new DefaultGeometricConverter();
                        }
                        var fbi = new ForensicBasedInvestigation()
                        {
                            MaxGenerations = maxGenerations,
                            GeometricConverter = geometricConverter,
                            NoMutation = noMutation
                        };
                        return fbi.Build();
                    case KnownCompoundMetaheuristics.Islands5Default:
                    case KnownCompoundMetaheuristics.Islands5DefaultNoMigration:
                    case KnownCompoundMetaheuristics.Islands5BestMixture:
                    case KnownCompoundMetaheuristics.Islands5BestMixtureNoMigration:
                        var islandNb = 5;
                        if (geometricConverter == null)
                        {
                            geometricConverter = new DefaultGeometricConverter();
                        }
                        IslandCompoundMetaheuristic islandCompound;

                        switch (enumName)
                        {
                            case KnownCompoundMetaheuristics.Islands5Default:
                            case KnownCompoundMetaheuristics.Islands5DefaultNoMigration:
                                ICompoundMetaheuristic targetCompoundHeuristic;
                                var defaultGA = new DefaultMetaHeuristic();
                                targetCompoundHeuristic = new SimpleCompoundMetaheuristic(defaultGA);

                                islandCompound = new IslandCompoundMetaheuristic(populationSize / islandNb, islandNb,
                                    targetCompoundHeuristic);
                               
                                break;
                            case KnownCompoundMetaheuristics.Islands5BestMixture:
                            case KnownCompoundMetaheuristics.Islands5BestMixtureNoMigration:
                                var woaIsland = new WhaleOptimisationAlgorithm()
                                {
                                    MaxGenerations = maxGenerations,
                                    GeometricConverter = geometricConverter,
                                    NoMutation = noMutation
                                };
                                var eoIsland = new EquilibriumOptimizer()
                                {
                                    MaxGenerations = maxGenerations,
                                    GeometricConverter = geometricConverter,
                                    NoMutation = noMutation
                                };
                                var fbiIsland = new ForensicBasedInvestigation()
                                {
                                    MaxGenerations = maxGenerations,
                                    GeometricConverter = geometricConverter,
                                    NoMutation = noMutation
                                };
                                var defaultGABest = new DefaultMetaHeuristic();
                                var defaultIsland = new SimpleCompoundMetaheuristic(defaultGABest);
                                islandCompound = new IslandCompoundMetaheuristic(populationSize,
                                    (1,defaultIsland),
                                    (1, woaIsland),
                                    (1, fbiIsland),
                                    (2, eoIsland));
                                break;
                            default:
                                throw new InvalidOperationException("Unsuported Island configuration");
                        }
                        if (enumName == KnownCompoundMetaheuristics.Islands5DefaultNoMigration||
                            enumName == KnownCompoundMetaheuristics.Islands5BestMixtureNoMigration)
                        {
                            islandCompound.MigrationMode = MigrationMode.None;
                        }
                        return islandCompound.Build();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(name));
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
                    case KnownCompoundMetaheuristics.DefaultRandomHyperspeed:
                        return typeof(DefaultMetaHeuristic);
                    case KnownCompoundMetaheuristics.WhaleOptimisation:
                    case KnownCompoundMetaheuristics.WhaleOptimisationNaive:
                        return typeof(IfElseMetaHeuristic);
                    case KnownCompoundMetaheuristics.ForensicBasedInvestigation:
                        return typeof(GenerationMetaHeuristic);
                    case KnownCompoundMetaheuristics.EquilibriumOptimizer:
                        return typeof(MatchMetaHeuristic);
                    case KnownCompoundMetaheuristics.Islands5Default:
                    case KnownCompoundMetaheuristics.Islands5DefaultNoMigration:
                    case KnownCompoundMetaheuristics.Islands5BestMixture:
                    case KnownCompoundMetaheuristics.Islands5BestMixtureNoMigration:
                        return typeof(IslandMetaHeuristic);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(name));
                }
            }
            return TypeHelper.GetTypeByName<IMetaHeuristic>(name);
        }
        #endregion
    }
}