using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp.Domain.Reinsertions
{
	/// <summary>
	/// Elitist reinsertion.
	/// <remarks>
	/// When there are less offspring than parents, select the best parents to be reinserted together with the offsprings. 
	/// 
    /// <see href="http://usb-bg.org/Bg/Annual_Informatics/2011/SUB-Informatics-2011-4-29-35.pdf">Generalized Nets Model of Offspring Reinsertion in Genetic Algorithm</see>
	/// </remarks>
	/// </summary>
	public class ElitistReinsertion : ReinsertionBase
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.ElitistReinsertion"/> class.
		/// </summary>
		public ElitistReinsertion() : base(false, true)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Selects the chromosomes which will be reinserted.
		/// </summary>
		/// <returns>The chromosomes to be reinserted in next generation..</returns>
		/// <param name="population">The population.</param>
		/// <param name="offsprings">The offsprings.</param>
		/// <param name="parents">The parents.</param>
		protected override IList<IChromosome> PerformSelectChromosomes (Population population, IList<IChromosome> offsprings, IList<IChromosome> parents)
		{
			var diff = population.MinSize - offsprings.Count;

			if (diff > 0) {
				var bestParents = parents.OrderByDescending (p => p.Fitness).Take (diff);

				foreach (var p in bestParents) {
					offsprings.Add (p);
				}
			}

			return offsprings;
		}
		#endregion
	}
}