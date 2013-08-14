using System;
using HelperSharp;

namespace GeneticSharp.Domain.Mutations
{
	public sealed class MutationException : Exception
	{
		#region Constructors
		public MutationException (IMutation mutation, string message) : base("{0}: {1}".With(mutation.GetType().Name, message))
		{
			Mutation = mutation;
		}
		#endregion

		#region Properties
		public IMutation Mutation { get; private set; }
		#endregion
	}
}