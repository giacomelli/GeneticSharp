using System;
using System.Collections.Generic;
using System.Linq;
using Amib.Threading;
using HelperSharp;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

namespace GeneticSharp.Domain.Populations
{
	/// <summary>
	/// Represents a population of candidate solutions (chromosomes).
	/// </summary>
	public class Population
    {
		#region Events
		/// <summary>
		/// Occurs when best chromosome changed.
		/// </summary>
		public event EventHandler BestChromosomeChanged;
		#endregion

        #region Fields
        private IChromosome m_adamChromosome;
        #endregion

        #region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Populations.Population"/> class.
		/// </summary>
		/// <param name="minSize">The minimum size (chromosomes).</param>
		/// <param name="maxSize">The maximum size (chromosomes).</param>
		/// <param name="adamChromosome">The original chromosome of all population ;).</param>
	    public Population(int minSize, 
                          int maxSize,
                          IChromosome adamChromosome)
		{
            if (minSize < 2)
            {
                throw new ArgumentOutOfRangeException("minSize", "The minimum size for a population is 2 chromosomes.");
            }

            if (maxSize < minSize)
            {
                throw new ArgumentOutOfRangeException("maxSize", "The maximum size for a population should be equal or greater than minimum size.");
            }

            ExceptionHelper.ThrowIfNull("adamChromosome", adamChromosome);
        
			MinSize = minSize;
            MaxSize = maxSize;
            m_adamChromosome = adamChromosome;
			Generations = new List<Generation> ();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the generations.
		/// </summary>
		/// <value>The generations.</value>
		public IList<Generation> Generations { get; private set; }

		/// <summary>
		/// Gets the current generation.
		/// </summary>
		/// <value>The current generation.</value>
		public Generation CurrentGeneration { get; private set; }

		/// <summary>
		/// Gets the minimum size.
		/// </summary>
		/// <value>The minimum size.</value>
		public int MinSize { get; private set; }

		/// <summary>
		/// Gets the size of the max.
		/// </summary>
		/// <value>The size of the max.</value>
        public int MaxSize { get; private set; }

		/// <summary>
		/// Gets the best chromosome.
		/// </summary>
		/// <value>The best chromosome.</value>
		public IChromosome BestChromosome { get; private set; }
		#endregion

		#region Public methods
		/// <summary>
		/// Creates the initial generation.
		/// </summary>
		/// <returns>The initial generation.</returns>
		public void CreateInitialGeneration ()
		{
			var chromosomes = new List<IChromosome> ();

			for(int i = 0; i < MinSize; i++)
			{
				var c = m_adamChromosome.CreateNew ();
				chromosomes.Add (c);
			}

			CreateNewGeneration(chromosomes);
		}

		/// <summary>
		/// Finalizes the current generation.
		/// </summary>
		public void FinalizeGeneration (ISelection selection)
		{
			if(CurrentGeneration.Chromosomes.Count > MaxSize)
			{
				CurrentGeneration.Chromosomes = selection.SelectChromosomes(MaxSize, CurrentGeneration);

				if (!CurrentGeneration.Chromosomes.Any (c => c == CurrentGeneration.BestChromosome)) {
					CurrentGeneration.Chromosomes.RemoveAt (CurrentGeneration.Chromosomes.Count - 1);
					CurrentGeneration.Chromosomes.Add (CurrentGeneration.BestChromosome);
				}
			}
		}
	
		/// <summary>
		/// Creates a new generation.
		/// </summary>
		/// <returns>The new generation.</returns>
		/// <param name="chromosomes">Chromosomes.</param>
		public void CreateNewGeneration(IList<IChromosome> chromosomes)
		{
			CurrentGeneration = new Generation (Generations.Count + 1, chromosomes);
			Generations.Add (CurrentGeneration);
		}

		/// <summary>
		/// Elects the best chromosome.
		/// </summary>
		public void ElectBestChromosome()
		{
			var newBestChromosome = CurrentGeneration.Chromosomes.OrderByDescending(c => c.Fitness.Value).First();
			ValidateBestChromosome (newBestChromosome);
			CurrentGeneration.BestChromosome = newBestChromosome;

			if (newBestChromosome != BestChromosome) {
				BestChromosome = newBestChromosome;
				if (BestChromosomeChanged != null) {
					BestChromosomeChanged (this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Validates the best chromosome.
		/// </summary>
		/// <param name="chromosome">Chromosome.</param>
		private void ValidateBestChromosome(IChromosome chromosome)
		{
			if (!chromosome.Fitness.HasValue) {
				throw new InvalidOperationException (
					"There is unknown problem in current population, because BestChromosome should have a Fitness value. BestChromosome: Id:{0}, age: {1} and length: {2}"
					.With (chromosome.Id, chromosome.Age, chromosome.Length));
			}
		}
		#endregion
	}
}