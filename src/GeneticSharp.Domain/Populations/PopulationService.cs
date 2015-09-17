using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GeneticSharp.Infrastructure.Framework.Reflection;

namespace GeneticSharp.Domain.Populations
{
    /// <summary>
    /// Population service.
    /// </summary>
    public static class PopulationService
    {
        #region Methods
        /// <summary>
        /// Gets available generation strategy types.
        /// </summary>
        /// <returns>All available generation strategy types.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<Type> GetGenerationStrategyTypes()
        {
            return TypeHelper.GetTypesByInterface<IGenerationStrategy>();
        }

        /// <summary>
        /// Gets the available generation strategy names.
        /// </summary>
        /// <returns>The generation strategy names.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<string> GetGenerationStrategyNames()
        {
            return TypeHelper.GetDisplayNamesByInterface<IGenerationStrategy>();
        }

        /// <summary>
        /// Creates the IGenerationStrategy's implementation with the specified name.
        /// </summary>
        /// <returns>The generation strategy implementation instance.</returns>
        /// <param name="name">The generation strategy name.</param>
        /// <param name="constructorArgs">Constructor arguments.</param>
        public static IGenerationStrategy CreateGenerationStrategyByName(string name, params object[] constructorArgs)
        {
            return TypeHelper.CreateInstanceByName<IGenerationStrategy>(name, constructorArgs);
        }

        /// <summary>
        /// Gets the generation strategy type by the name.
        /// </summary>
        /// <returns>The generation strategy type.</returns>
        /// <param name="name">The name of generation strategy.</param>
        public static Type GetGenerationStrategyTypeByName(string name)
        {
            return TypeHelper.GetTypeByName<IGenerationStrategy>(name);
        }
        #endregion
    }
}