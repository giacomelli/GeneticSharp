using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GeneticSharp.Infrastructure.Framework.Reflection;

namespace GeneticSharp.Domain.Terminations
{
    /// <summary>
    /// Termination service.
    /// </summary>
    public static class TerminationService
    {
        #region Methods
        /// <summary>
        /// Gets available termination types.
        /// </summary>
        /// <returns>All available termination types.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<Type> GetTerminationTypes()
        {
            return TypeHelper.GetTypesByInterface<ITermination>();
        }

        /// <summary>
        /// Gets the available termination names.
        /// </summary>
        /// <returns>The termination names.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<string> GetTerminationNames()
        {
            return TypeHelper.GetDisplayNamesByInterface<ITermination>();
        }

        /// <summary>
        /// Creates the ITermination's implementation with the specified name.
        /// </summary>
        /// <returns>The ITermination's implementation instance.</returns>
        /// <param name="name">The ITermination name.</param>
        /// <param name="constructorArgs">Constructor arguments.</param>
        public static ITermination CreateTerminationByName(string name, params object[] constructorArgs)
        {
            return TypeHelper.CreateInstanceByName<ITermination>(name, constructorArgs);
        }

        /// <summary>
        /// Gets the termination type by the name.
        /// </summary>
        /// <returns>The termination type.</returns>
        /// <param name="name">The name of termination.</param>
        public static Type GetTerminationTypeByName(string name)
        {
            return TypeHelper.GetTypeByName<ITermination>(name);
        }
        #endregion
    }
}