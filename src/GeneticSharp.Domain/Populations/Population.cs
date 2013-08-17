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
		#region Constants
		/// <summary>
		/// The default crossover probability.
		/// </summary>
		public const float DefaultCrossoverProbability = 0.75f;

		/// <summary>
		/// The default mutation probability.
		/// </summary>
		public const float DefaultMutationProbability = 0.1f;
		#endregion

        #region Events
		/// <summary>
		/// Occurs when generation ran.
		/// </summary>
        public event EventHandler GenerationRan;

		/// <summary>
		/// Occurs when best chromosome changed.
		/// </summary>
		public event EventHandler BestChromosomeChanged;

		/// <summary>
		/// Occurs when termination reached.
		/// </summary>
		public event EventHandler TerminationReached;
        #endregion

        #region Fields
        private IChromosome m_adamChromosome;
		private SmartThreadPool m_threadPool;
        #endregion

        #region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Populations.Population"/> class.
		/// </summary>
		/// <param name="minSize">The minimum size (chromosomes).</param>
		/// <param name="maxSize">The maximum size (chromosomes).</param>
		/// <param name="adamChromosome">The original chromosome of all population ;).</param>
		/// <param name="fitness">The fitness evaluation function.</param>
		/// <param name="selection">The selection operator.</param>
		/// <param name="crossover">The crossover operator.</param>
		/// <param name="mutation">The mutation operator.</param>
        public Population(int minSize, 
                          int maxSize,
                          IChromosome adamChromosome,
		                  IFitness fitness, 
		                  ISelection selection, 
		                  ICrossover crossover,
		                  IMutation mutation)
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
            ExceptionHelper.ThrowIfNull("fitness", fitness);
            ExceptionHelper.ThrowIfNull("selection", selection);
            ExceptionHelper.ThrowIfNull("crossover", crossover);
            ExceptionHelper.ThrowIfNull("mutation", mutation);

			MinSize = minSize;
            MaxSize = maxSize;
            m_adamChromosome = adamChromosome;
			Fitness = fitness;
			Selection = selection;
			Crossover = crossover;
			Mutation = mutation;
			Termination = new GenerationNumberTermination (1);

			Generations = new List<Generation> ();
			CrossoverProbability = DefaultCrossoverProbability;
			MutationProbability = DefaultMutationProbability;

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
		/// Gets the fitness function.
		/// </summary>
		public IFitness Fitness { get; private set; }

		/// <summary>
		/// Gets the selection operator.
		/// </summary>
		public ISelection Selection { get; private set; }

		/// <summary>
		/// Gets the crossover operator.
		/// </summary>
		/// <value>The crossover.</value>
		public ICrossover Crossover { get; private set; }

		/// <summary>
		/// Gets or sets the crossover probability.
		/// </summary>
		public float CrossoverProbability  { get; set; }

		/// <summary>
		/// Gets the mutation operator.
		/// </summary>
		public IMutation Mutation { get; private set; }

		/// <summary>
		/// Gets or sets the mutation probability.
		/// </summary>
		public float MutationProbability  { get; set; }

		/// <summary>
		/// Gets or sets the termination condition.
		/// </summary>
		public ITermination Termination { get; set; }

		/// <summary>
		/// Gets the best chromosome.
		/// </summary>
		/// <value>The best chromosome.</value>
        public IChromosome BestChromosome { get; private set; }
		#endregion

		#region Public methods
		/// <summary>
		/// Runs a generation.
		/// </summary>
		/// <param name="timeout">The timeout to run the generation.</param>
		public bool RunGeneration(int timeout = 0)
		{
			if (Generations.Count == 0) {
				CurrentGeneration = CreateNewGeneration (CreateInitialChromosomes ());
				EvaluateFitness (timeout);
				CurrentGeneration.Chromosomes = SelectParents ();
			} else {
				EvaluateFitness (timeout);
				CurrentGeneration = CreateNewGeneration(SelectParents ());
			}
			
			Mutate (Cross ());
            EvaluateFitness(timeout);
            ElectBestChromosome();
            FinalizeGeneration();

            if (GenerationRan != null)
            {
                GenerationRan(this, EventArgs.Empty);
            }

			if (Termination.HasReached (CurrentGeneration)) {
				if (TerminationReached != null) {
					TerminationReached (this, EventArgs.Empty);
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Runs the generations.
		/// </summary>
		/// <param name="timeoutPerGeneration">Timeout per generation.</param>
		public bool RunGenerations(int timeoutPerGeneration = 0)
		{
			bool terminationConditionReached = false;

			do {
				terminationConditionReached = RunGeneration (timeoutPerGeneration);
			} while(!terminationConditionReached);

			return terminationConditionReached;
		}

		/// <summary>
		/// Aborts the generation.
		/// </summary>
		/// <param name="timeout">Timeout to wait to abort.</param>
		public void AbortGeneration (int timeout = 60000)
		{
			if (m_threadPool != null) {
				m_threadPool.Shutdown (true, timeout);
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Finalizes the generation.
		/// </summary>
		private void FinalizeGeneration ()
		{
			if(CurrentGeneration.Chromosomes.Count > MaxSize)
			{
                CurrentGeneration.Chromosomes = Selection.SelectChromosomes(MaxSize, CurrentGeneration);

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
		private Generation CreateNewGeneration(IList<IChromosome> chromosomes)
		{
			var g = new Generation (Generations.Count + 1, chromosomes);
			Generations.Add (g);

			return g;
		}

		/// <summary>
		/// Creates the initial chromosomes.
		/// </summary>
		/// <returns>The initial chromosomes.</returns>
		private IList<IChromosome> CreateInitialChromosomes ()
		{
			var chromosomes = new List<IChromosome> ();

			for(int i = 0; i < MinSize; i++)
			{
				var c = m_adamChromosome.CreateNew ();
				chromosomes.Add (c);
			}

			return chromosomes;
		}

		/// <summary>
		/// Evaluates the fitness.
		/// </summary>
		/// <param name="timeout">Timeout.</param>
        private void EvaluateFitness(int timeout)
        {
            if (Fitness.SupportsParallel)
            {
                EvaluateFitnessParallel(timeout);
            }
            else
            {
                EvaluateFitnessLinear(timeout);
            }
        }

		/// <summary>
		/// Evaluates the fitness linear.
		/// </summary>
		/// <param name="timeout">Timeout.</param>
        private void EvaluateFitnessLinear(int timeout)
        {
            var chromosomesWithoutFitness = CurrentGeneration.Chromosomes.Where(c => !c.Fitness.HasValue);
             
			foreach(var c in chromosomesWithoutFitness)
            {
                c.Fitness = Fitness.Evaluate(c);

                if (c.Fitness < 0 || c.Fitness > 1)
                {
                    throw new FitnessException(Fitness, "The {0}.Evaluate returns a fitness with value {1}. The fitness value should be between 0.0 and 1.0."
                                                        .With(Fitness.GetType(), c.Fitness));
                }
            }          
        }

		/// <summary>
		/// Evaluates the fitness parallel.
		/// </summary>
		/// <param name="timeout">Timeout.</param>
		private void EvaluateFitnessParallel (int timeout)
		{
			m_threadPool = new SmartThreadPool();

			try {
	            m_threadPool.MinThreads = MinSize;
				m_threadPool.MaxThreads = MinSize;	            
	            var chromosomesWithoutFitness = CurrentGeneration.Chromosomes.Where(c => !c.Fitness.HasValue).ToList();
	            var workItemResults = new IWorkItemResult[chromosomesWithoutFitness.Count];

	            for (int i = 0; i < chromosomesWithoutFitness.Count; i++)
	            {
	                var c = chromosomesWithoutFitness[i];

	                try
	                {
						workItemResults[i] = m_threadPool.QueueWorkItem(new WorkItemCallback(RunEvaluateFitness), c);
	                }
	                catch (Exception ex)
	                {
	                    throw new InvalidOperationException("Error executing Fitness.Evaluate for chromosome {0}: {1}".With(c.Id, ex.Message), ex);
	                }
	            }                
				
                m_threadPool.Start ();                

				if(!m_threadPool.WaitForIdle (timeout == 0 ? int.MaxValue : timeout))
				{
					throw new TimeoutException("The RunGeneration reach the {0} milliseconds timeout.".With(timeout));
				}

                foreach (var wi in workItemResults)
                {
                    Exception ex;
                    wi.GetResult(out ex);

                    if (ex != null)
                    {
                        throw ex;
                    }
                }

				foreach (var c in chromosomesWithoutFitness)
				{
					if (c.Fitness < 0 || c.Fitness > 1)
					{
						throw new FitnessException(Fitness, "The {0}.Evaluate returns a fitness with value {1}. The fitness value should be between 0.0 and 1.0."
						                                    .With(Fitness.GetType(), c.Fitness));
					}
				}
			}
			finally {
				m_threadPool.Shutdown(true, 1000);
			}
		}

		/// <summary>
		/// Runs the evaluate fitness.
		/// </summary>
		/// <returns>The evaluate fitness.</returns>
		/// <param name="state">State.</param>
		private object RunEvaluateFitness(object state)
		{
			var c = state as IChromosome;

			try
			{
				c.Fitness = Fitness.Evaluate(c);
			}
			catch (Exception ex)
			{
				throw new FitnessException(Fitness, "Error executing Fitness.Evaluate for chromosome {0}: {1}".With(c.Id, ex.Message), ex);
			}

			return c.Fitness;
		}

		/// <summary>
		/// Elects the best chromosome.
		/// </summary>
		private void ElectBestChromosome()
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

		/// <summary>
		/// Selects the parents.
		/// </summary>
		/// <returns>The parents.</returns>
		private IList<IChromosome> SelectParents ()
		{
			return Selection.SelectChromosomes (MinSize, CurrentGeneration);
		}

		/// <summary>
		/// Cross this instance.
		/// </summary>
		private IList<IChromosome> Cross ()
		{
			var children = new List<IChromosome>();

			for ( int i = 0; i < MinSize; i += Crossover.ParentsNumber )
			{
				if (RandomizationProvider.Current.GetDouble() <= CrossoverProbability)
				{
					var child = Crossover.Cross (CurrentGeneration.Chromosomes.Skip(i).Take(Crossover.ParentsNumber).ToList());
					children.AddRange (child);
				}
			}

			foreach (var c in children) {
				CurrentGeneration.Chromosomes.Add (c);
			}
                        
			return children;
		}        

		/// <summary>
		/// Mutate the specified chromosomes.
		/// </summary>
		/// <param name="chromosomes">Chromosomes.</param>
		private void Mutate (IList<IChromosome> chromosomes)
		{
			foreach(var c in chromosomes)
			{
				Mutation.Mutate (c, MutationProbability);
			}
		}
		#endregion
	}
}