using System;
using HelperSharp;

namespace GeneticSharp.Domain.Mutations
{
	/// <summary>
	/// Exception throw when an error occurs during the execution of mutate.
	/// </summary>
	public sealed class MutationException : Exception
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Mutations.MutationException"/> class.
		/// </summary>
		/// <param name="mutation">The mutation where ocurred the error.</param>
		/// <param name="message">The error message.</param>
		public MutationException (IMutation mutation, string message) : base("{0}: {1}".With(mutation.GetType().Name, message))
		{
			Mutation = mutation;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the mutation.
		/// </summary>
		/// <value>The mutation.</value>
		public IMutation Mutation { get; private set; }
		#endregion
	}
}