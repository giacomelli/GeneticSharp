using System.Collections.Generic;

namespace GeneticSharp
{
	/// <summary>
	/// Defines an interface for a vector fitness function.  <see cref="GeneticSharp.Domain.Fitnesses.IFitness"/>, for discussion of a scalar fitness function.
	/// <remarks>
	/// A vector fitness function extends the notion of a scalar fitness function to express the vector result of a fitness function run in parallel on a vector-processor
	/// device such as a GPU.  Given a set of chromosomes, the vector fitness function evaluates the chromosomes in the set on a logical vector device, and returns a vector of fitness values.
	/// This approach differs from and offers convenience relative to the scalar fitness function executed in parallel with <see cref="GeneticSharp.Infrastructure.Framework.Threading.ParallelTaskExecutor"/>,
	/// as VectorFitness delegates the parallel scoring of the chromosomes to the vector device and its substantial parallelism, rather than using a CPU thread per chromosome.
	/// Applications <em>must</em> use instances of this class with <see cref="GeneticSharp.Infrastructure.Framework.Threading.LinearTaskExecutor"/>.
	/// <see href="http://en.wikipedia.org/wiki/Fitness_function">Wikipedia</see>
	/// </remarks>
	/// </summary>
	public abstract class VectorFitness : IFitness
	{
		public double Evaluate(IChromosome chromosome)
		{
			// This method is not used in VectorFitness, but is required by the IFitness interface.
			// It is provided here to satisfy the interface contract, but should not be called.
			throw new System.NotSupportedException("IVectorFitness does not support single chromosome evaluation. Use Evaluate(IList<IChromosome>) or IFitness.Evaluate(IChromosome) instead.");
		}

		/// <summary>
		/// Evaluates the specified chromosomes and returns their fitness values as a vector.
		/// </summary>
		/// <param name="chromosomes">The chromosomes to be evaluated.</param>
		/// <returns>A vector of fitness values corresponding to the input chromosomes.  The indexes of the fitness values match the indexes of the chromosomes in the list.</returns>
		public abstract double[] Evaluate(IList<IChromosome> chromosomes);
	}
}
