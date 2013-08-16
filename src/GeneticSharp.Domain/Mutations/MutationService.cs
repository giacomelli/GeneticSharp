using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HelperSharp;
using GeneticSharp.Domain.Helpers;

namespace GeneticSharp.Domain.Mutations
{
	/// <summary>
	/// Mutation service.
	/// </summary>
	public static class MutationService
	{
		#region Methods
		/// <summary>
		/// Gets available mutation types.
		/// </summary>
		/// <returns>All available mutation types.</returns>
		public static IList<Type> GetMutationTypes()
		{
			return TypeHelper.GetTypesByInterface<IMutation> ();
		}

		/// <summary>
		/// Gets the available mutation names.
		/// </summary>
		/// <returns>The mutation names.</returns>
		public static IList<string> GetMutationNames ()
		{
			return TypeHelper.GetDisplayNamesByInterface<IMutation> ();
		}	

		/// <summary>
		/// Creates the IMutation's implementation with the specified name.
		/// </summary>
		/// <returns>The IMutation's implementation instance.</returns>
		/// <param name="name">The IMutation name.</param>
		/// <param name="constructorArgs">Constructor arguments.</param>
		public static IMutation CreateMutationByName(string name, params object[] constructorArgs)
		{
			return TypeHelper.CreateInstanceByName<IMutation> (name, constructorArgs);
		}

		/// <summary>
		/// Gets the mutation type by the name.
		/// </summary>
		/// <returns>The mutation type.</returns>
		/// <param name="name">The name of mutation.</param>
		public static Type GetMutationTypeByName (string name)
		{
			return TypeHelper.GetTypeByName<IMutation> (name);
		}
		#endregion
	}
}