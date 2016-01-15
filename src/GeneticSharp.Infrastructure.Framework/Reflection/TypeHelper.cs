using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HelperSharp;

namespace GeneticSharp.Infrastructure.Framework.Reflection
{
    /// <summary>
    /// Type helper.
    /// </summary>
    public static class TypeHelper
    {
        #region Methods
        /// <summary>
        /// Gets types by interface name
        /// </summary>
        /// <typeparam name="TInterface">The interface.</typeparam>
        /// <returns>All types that implements the interface specified.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "is a good use for this case")]
        public static IList<Type> GetTypesByInterface<TInterface>()
        {
            var interfaceType = typeof(TInterface);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var selectedAssemblies = assemblies.Where(
                a => a.FullName.StartsWith("GeneticSharp.", StringComparison.OrdinalIgnoreCase));

            var types = selectedAssemblies.SelectMany(a => a.GetTypes())
                    .Where(t => t.GetInterfaces().Any(i => i == interfaceType) && !t.IsAbstract)
                    .OrderBy(t => t.Name)
                    .ToList();

            return types;
        }

        /// <summary>
        /// Gets the available crossover names.
        /// </summary>
        /// <typeparam name="TInterface">The interface.</typeparam>
        /// <returns>The crossover names.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "is a good use for this case")]
        public static IList<string> GetDisplayNamesByInterface<TInterface>()
        {
            return GetTypesByInterface<TInterface>()
                .Select(t => GetDisplayNameAttribute(t).DisplayName)
                .ToList();
        }

        /// <summary>
        /// Creates the TInterface's implementation with the specified name.
        /// </summary>
        /// <returns>The TInterface's implementation instance.</returns>
        /// <param name="name">The TInterface's implementation name.</param>
        /// <param name="constructorArgs">Constructor arguments.</param>
        /// <typeparam name="TInterface">The interface.</typeparam>
        public static TInterface CreateInstanceByName<TInterface>(string name, params object[] constructorArgs)
        {
            var crossoverType = GetTypeByName<TInterface>(name);

            try
            {
                return (TInterface)Activator.CreateInstance(crossoverType, constructorArgs);
            }
            catch (MissingMethodException ex)
            {
                throw new ArgumentException("A {0}'s implementation with name '{1}' was found, but seems the constructor args were invalid.".With(typeof(TInterface).Name, name), "constructorArgs", ex);
            }
        }

        /// <summary>
        /// Gets the TInterface's implementation with the specified name.
        /// </summary>
        /// <returns>The TInterface's implementation type.</returns>
        /// <param name="name">The TInterface's implementation name.</param>
        /// <typeparam name="TInterface">The interface.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "is a good use for this case")]
        public static Type GetTypeByName<TInterface>(string name)
        {
            var interfaceName = typeof(TInterface).Name;
            var crossoverType = GetTypesByInterface<TInterface>()
                .Where(t => GetDisplayNameAttribute(t).DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

            if (crossoverType == null)
            {
                throw new ArgumentException("There is no {0} implementation with name '{1}'.".With(interfaceName, name), "name");
            }

            return crossoverType;
        }

        private static DisplayNameAttribute GetDisplayNameAttribute(MemberInfo member)
        {
            var attribute = member.GetCustomAttributes(false).FirstOrDefault(a => a is DisplayNameAttribute);

            if (attribute == null)
            {
                throw new InvalidOperationException("The member '{0}' has no DisplayNameAttribute.".With(member.Name));
            }

            return attribute as DisplayNameAttribute;
        }
        #endregion
    }
}