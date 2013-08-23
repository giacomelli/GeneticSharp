using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Reinsertions
{
	/// <summary>
	/// Base class for IReinsertion's implementations.
	/// </summary>
	public abstract class ReinsertionBase : IReinsertion
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.ReinsertionBase"/> class.
		/// </summary>
		/// <param name="canCollpase">If set to <c>true</c> can collapse the number of selected chromosomes for reinsertion.</param>
		/// <param name="canExpand">If set to <c>true</c> can expand the number of selected chromosomes for reinsertion.</param>
		protected ReinsertionBase(bool canCollpase, bool canExpand)
		{
			CanCollapse = canCollpase;
			CanExpand = canExpand;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets if can collapse the number of selected chromosomes for reinsertion.
		/// </summary>
		public bool CanCollapse { get; private set; }

		/// <summary>
		/// Gets if can expand the number of selected chromosomes for reinsertion.
		/// </summary>
		public bool CanExpand { get; private set; }
		#endregion

		#region Methods
		/// <summary>
		/// Selects the chromosomes which will be reinserted.
		/// </summary>
		/// <returns>The chromosomes to be reinserted in next generation..</returns>
		/// <param name="population">The population.</param>
		/// <param name="offsprings">The offsprings.</param>
		/// <param name="parents">The parents.</param>
		public IList<IChromosome> SelectChromosomes (Population population, IList<IChromosome> offsprings, IList<IChromosome> parents)
		{
			if (!CanExpand && offsprings.Count < population.MinSize) {
				throw new ReinsertionException (this, "Cannot expand the number of chromosome in population. Try another reinsertion!");
			}

			if (!CanCollapse && offsprings.Count > population.MaxSize) {
				throw new ReinsertionException (this, "Cannot collapse the number of chromosome in population. Try another reinsertion!");
			}

			return PerformSelectChromosomes (population, offsprings, parents);
		}

		/// <summary>
		/// Selects the chromosomes which will be reinserted.
		/// </summary>
		/// <returns>The chromosomes to be reinserted in next generation..</returns>
		/// <param name="population">The population.</param>
		/// <param name="offsprings">The offsprings.</param>
		/// <param name="parents">The parents.</param>
		protected abstract IList<IChromosome> PerformSelectChromosomes (Population population, IList<IChromosome> offsprings, IList<IChromosome> parents);
		#endregion
	}
}

