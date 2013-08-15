using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HelperSharp;
using GeneticSharp.Domain.Helpers;

namespace GeneticSharp.Domain.Crossovers
{
	/// <summary>
	/// Crossover service.
	/// </summary>
	public static class CrossoverService
	{
		#region Methods
		/// <summary>
		/// Gets available crossover types.
		/// </summary>
		/// <returns>All available crossover types.</returns>
		public static IList<Type> GetCrossoverTypes()
		{
			return TypeHelper.GetTypesByInterface<ICrossover>();
		}

		/// <summary>
		/// Gets the available crossover names.
		/// </summary>
		/// <returns>The crossover names.</returns>
		public static IList<string> GetCrossoverNames ()
		{
			return TypeHelper.GetDisplayNamesByInterface<ICrossover>();
		}

		/// <summary>
		/// Creates the ICrossover's implementation with the specified name.
		/// </summary>
		/// <returns>The crossover implementation instance.</returns>
		/// <param name="name">The crossover name.</param>
		/// <param name="constructorArgs">Constructor arguments.</param>
		public static ICrossover CreateCrossoverByName(string name, params object[] constructorArgs)
		{
			return TypeHelper.CreateInstanceByName<ICrossover> (name, constructorArgs);
		}
		#endregion
	}
}