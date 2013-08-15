using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using HelperSharp;

namespace GeneticSharp.Domain.Helpers
{
	/// <summary>
	/// Type helper.
	/// </summary>
	internal static class TypeHelper
	{
		#region Methods
		/// <summary>
		/// Gets types by interface name
		/// </summary>
		/// <returns>All types that implements the interface specified.</returns>
		public static IList<Type> GetTypesByInterface<TInterface>()
		{
			var interfaceName = typeof(TInterface).Name;
			var assembly = typeof(TypeHelper).Assembly;
			var types = assembly.GetTypes ()
				.Where (t => t.GetInterface (interfaceName) != null && !t.IsAbstract)
					.OrderBy (t => t.Name)
					.ToList ();

			return types;
		}

		/// <summary>
		/// Gets the available crossover names.
		/// </summary>
		/// <returns>The crossover names.</returns>
		public static IList<string> GetDisplayNamesByInterface<TInterface> ()
		{
			return GetTypesByInterface<TInterface> ()
				.Select (t => ((DisplayNameAttribute)t.GetCustomAttributes (false).First ()).DisplayName)
					.ToList ();
		}

		/// <summary>
		/// Creates the TInterface's implementation with the specified name.
		/// </summary>
		/// <returns>The TInterface's implementation instance.</returns>
		/// <param name="name">The TInterface's implementation name.</param>
		/// <param name="constructorArgs">Constructor arguments.</param>
		public static TInterface CreateInstanceByName<TInterface>(string name, params object[] constructorArgs)
		{
			var interfaceName = typeof(TInterface).Name;
			var crossoverType =  GetTypesByInterface<TInterface> ()
				.Where (t => ((DisplayNameAttribute)t.GetCustomAttributes (false).First ()).DisplayName.Equals (name, StringComparison.OrdinalIgnoreCase))
					.FirstOrDefault ();

			if (crossoverType == null) {
				throw new ArgumentException ("There is no {0} implementation with name '{1}'.".With(interfaceName, name), "name");
			}

			try {
				return (TInterface) Activator.CreateInstance (crossoverType, constructorArgs);
			}
			catch(MissingMethodException ex) {
				throw new ArgumentException ("A {0}'s implementation with name '{1}' was found, but seems the constructor args was invalid.".With (interfaceName, name), "constructorArgs", ex);
			}
		}
		#endregion
	}
}