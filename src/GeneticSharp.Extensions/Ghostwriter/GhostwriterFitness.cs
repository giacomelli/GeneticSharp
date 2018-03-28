using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Extensions.Ghostwriter
{
	/// <summary>
	/// Ghostwriter fitness.
	/// </summary>
	public class GhostwriterFitness : IFitness
	{
		private readonly Func<string, double> m_evaluateFunc;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GeneticSharp.Extensions.Ghostwriter.GhostwriterFitness"/> class.
		/// </summary>
		/// <param name="evaluateFunc">Evaluate func.</param>
		public GhostwriterFitness(Func<string, double> evaluateFunc)
		{
			ExceptionHelper.ThrowIfNull("evaluateFunc", evaluateFunc);
			m_evaluateFunc = evaluateFunc;
		}

		/// <summary>
		/// Gets or sets the evaluate function.
		/// </summary>
		/// <value>The evaluate function.</value>

		/// <summary>
		/// Performs the evaluation against the specified chromosome.
		/// </summary>
		/// <param name="chromosome">The chromosome to be evaluated.</param>
		/// <returns>The fitness of the chromosome.</returns>
		public double Evaluate(IChromosome chromosome)
		{
			var c = chromosome as GhostwriterChromosome;
			var text = c.BuildText();

			return m_evaluateFunc(text);
		}
	}
}
