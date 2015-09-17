using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GeneticSharp.Infrastructure.Framework.Reflection;

namespace GeneticSharp.Domain.Selections
{
    /// <summary>
    /// Selection service.
    /// </summary>
    public static class SelectionService
    {
        #region Methods
        /// <summary>
        /// Gets available selection types.
        /// </summary>
        /// <returns>All available selection types.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<Type> GetSelectionTypes()
        {
            return TypeHelper.GetTypesByInterface<ISelection>();
        }

        /// <summary>
        /// Gets the available selection names.
        /// </summary>
        /// <returns>The selection names.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<string> GetSelectionNames()
        {
            return TypeHelper.GetDisplayNamesByInterface<ISelection>();
        }

        /// <summary>
        /// Creates the ISelection's implementation with the specified name.
        /// </summary>
        /// <returns>The selection implementation instance.</returns>
        /// <param name="name">The selection name.</param>
        /// <param name="constructorArgs">Constructor arguments.</param>
        public static ISelection CreateSelectionByName(string name, params object[] constructorArgs)
        {
            return TypeHelper.CreateInstanceByName<ISelection>(name, constructorArgs);
        }

        /// <summary>
        /// Gets the selection type by the name.
        /// </summary>
        /// <returns>The selection type.</returns>
        /// <param name="name">The name of selection.</param>
        public static Type GetSelectionTypeByName(string name)
        {
            return TypeHelper.GetTypeByName<ISelection>(name);
        }
        #endregion
    }
}