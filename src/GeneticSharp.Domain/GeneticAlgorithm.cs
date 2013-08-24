using System;
using System.Collections.Generic;
using System.Linq;
using Amib.Threading;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using HelperSharp;

namespace GeneticSharp.Domain
{
	/// <summary>
	/// A genetic algorithm (GA) is a search heuristic that mimics the process of natural selection. 
	/// This heuristic (also sometimes called a metaheuristic) is routinely used to generate useful solutions 
	/// to optimization and search problems.[1] Genetic algorithms belong to the larger class of evolutionary 
	/// algorithms (EA), which generate solutions to optimization problems using techniques inspired by natural evolution, 
	/// such as inheritance, mutation, selection, and crossover.
	/// 
	/// Genetic algorithms find application in bioinformatics, phylogenetics, computational science, engineering, 
	/// economics, chemistry, manufacturing, mathematics, physics, pharmacometrics, game development and other fields.
	/// 
	/// <see href="http://http://en.wikipedia.org/wiki/Genetic_algorithm">Wikipedia</see>
	/// </summary>
	public sealed class GeneticAlgorithm : IDisposable
	{
		#region Fields
		private SmartThreadPool m_threadPool;
        private bool m_stopRequested;
		#endregion

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
		/// Occurs when termination reached.
		/// </summary>
		public event EventHandler TerminationReached;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.GeneticAlgorithm"/> class.
		/// </summary>
        /// <param name="population">The chromosomes population.</param>
		/// <param name="fitness">The fitness evaluation function.</param>
		/// <param name="selection">The selection operator.</param>
		/// <param name="crossover">The crossover operator.</param>
		/// <param name="mutation">The mutation operator.</param>
		public GeneticAlgorithm(Population population,
		                  IFitness fitness, 
		                  ISelection selection, 
		                  ICrossover crossover,
		                  IMutation mutation)
		{
			ExceptionHelper.ThrowIfNull("Population", population);
			ExceptionHelper.ThrowIfNull("fitness", fitness);
			ExceptionHelper.ThrowIfNull("selection", selection);
			ExceptionHelper.ThrowIfNull("crossover", crossover);
			ExceptionHelper.ThrowIfNull("mutation", mutation);

			Population = population;
			Fitness = fitness;
			Selection = selection;
			Crossover = crossover;
			Mutation = mutation;
			Reinsertion = new ElitistReinsertion ();
			Termination = new GenerationNumberTermination (1);

			CrossoverProbability = DefaultCrossoverProbability;
			MutationProbability = DefaultMutationProbability;
			TimeEvolving = TimeSpan.Zero;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the population.
		/// </summary>
		/// <value>The population.</value>
		public Population Population { get; private set; }

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
        /// Gets or sets the reinsertion operator.
        /// </summary>
        public IReinsertion Reinsertion { get; set; }

		/// <summary>
		/// Gets or sets the termination condition.
		/// </summary>
		public ITermination Termination { get; set; }

		/// <summary>
		/// Gets the time evolving.
		/// </summary>
		public TimeSpan TimeEvolving { get; private set; }  
		#endregion

		#region Methods
        /// <summary>
        /// Starts the genetic algorithm using population, fitness, selection, crossover, mutation and termination configured.
        /// </summary>
        /// <returns>True if termination condition has been reached.</returns>
        public void Start()
        {
            Start(0);
        }

        /// <summary>
        /// Starts the genetic algorithm using population, fitness, selection, crossover, mutation and termination configured.
        /// </summary>
        /// <param name="timeoutPerGeneration">The timeout per generation.</param>
        public void Start(int timeoutPerGeneration)
		{            
			var startDateTime = DateTime.Now;			
            Population.CreateInitialGeneration();
            TimeEvolving = DateTime.Now - startDateTime;

            Resume(timeoutPerGeneration);
		}

        /// <summary>
        /// Resumes the last evolution of the genetic algorithm.
        /// <remarks>
        /// If genetic algorithm was not explicit Stop (calling Stop method), you will need provide a new extended Termination.
        /// </remarks>
        /// </summary>
        public void Resume()
        {
            Resume(0);
        }

        /// <summary>
        /// Resumes the last evolution of the genetic algorithm.
        /// <remarks>
        /// If genetic algorithm was not explicit Stop (calling Stop method), you will need provide a new extended Termination.
        /// </remarks>
        /// </summary>
        /// <param name="timeoutPerGeneration">The timeout per generation.</param>
        public void Resume(int timeoutPerGeneration)
        {
            m_stopRequested = false; 

            if (Population.GenerationsNumber == 0)
            {
                throw new InvalidOperationException("Attempt to resume a genetic algorithm which was not yet started.");
            }

            if (EndCurrentGeneration(timeoutPerGeneration))
            {
                return;
            }

            bool terminationConditionReached = false;
            DateTime startDateTime;

            do
            {
                if (m_stopRequested)
                {
                    break;
                }

                startDateTime = DateTime.Now;
                terminationConditionReached = EvolveOneGeneration(timeoutPerGeneration);
                TimeEvolving += DateTime.Now - startDateTime;

            } while (!terminationConditionReached);
        }

        /// <summary>
        /// Stops the genetic algorithm.
        /// <remarks>
        /// The default timeout is 60000 milliseconds.
        /// </remarks>
        /// </summary>
        public void Stop()
        {
            Stop(60000);
        }

		/// <summary>
        /// Stops the genetic algorithm..
		/// </summary>
		/// <param name="timeout">Timeout to wait to abort.</param>
		public void Stop (int timeout)
		{
            if (Population.GenerationsNumber == 0)
            {
                throw new InvalidOperationException("Attempt to stop a genetic algorithm which was not yet started.");
            }

            m_stopRequested = true;

			if (m_threadPool != null) {
				m_threadPool.Shutdown (true, timeout);
			}
		}

        /// <summary>
        /// Evolve one generation.
        /// </summary>
        /// <param name="timeout">The timeout to evolve.</param>
        /// <returns>True if termination has been reached, otherwise false.</returns>
		private bool EvolveOneGeneration (int timeout = 0)
		{
            var parents = SelectParents();
            var offspring = Cross(parents);
            Mutate(offspring);
            var newGenerationChromosomes = Reinsert(offspring, parents);
            Population.CreateNewGeneration(newGenerationChromosomes);
			return EndCurrentGeneration (timeout);
		}        

		/// <summary>
		/// Ends the current generation.
		/// </summary>
		/// <returns><c>true</c>, if current generation was ended, <c>false</c> otherwise.</returns>
		/// <param name="timeout">Timeout.</param>
		private bool EndCurrentGeneration(int timeout)
		{
			EvaluateFitness(timeout);
			Population.EndCurrentGeneration ();

			if (GenerationRan != null)
			{
				GenerationRan(this, EventArgs.Empty);
			}

			if (Termination.HasReached (Population.CurrentGeneration)) {
				if (TerminationReached != null) {
					TerminationReached (this, EventArgs.Empty);
				}

				return true;
			}

			return false;
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
				EvaluateFitnessLinear();
			}

            Population.CurrentGeneration.Chromosomes = Population.CurrentGeneration.Chromosomes.OrderByDescending(c => c.Fitness.Value).ToList();
		}

		/// <summary>
		/// Evaluates the fitness linear.
		/// </summary>		
		private void EvaluateFitnessLinear()
		{
			var chromosomesWithoutFitness = Population.CurrentGeneration.Chromosomes.Where(c => !c.Fitness.HasValue);

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
				m_threadPool.MinThreads = Population.MinSize;
				m_threadPool.MaxThreads = Population.MinSize;	            
				var chromosomesWithoutFitness = Population.CurrentGeneration.Chromosomes.Where(c => !c.Fitness.HasValue).ToList();
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
						throw new InvalidOperationException("Error executing Fitness.Evaluate for chromosome: {0}".With(ex.Message), ex);
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
				throw new FitnessException(Fitness, "Error executing Fitness.Evaluate for chromosome: {0}".With(ex.Message), ex);
			}

			return c.Fitness;
		}

		/// <summary>
		/// Selects the parents.
		/// </summary>
		/// <returns>The parents.</returns>
		private IList<IChromosome> SelectParents ()
		{
			return Selection.SelectChromosomes (Population.MinSize, Population.CurrentGeneration);
		}

		/// <summary>
		/// Cross this instance.
		/// </summary>
		private IList<IChromosome> Cross (IList<IChromosome> parents)
		{
			var offspring = new List<IChromosome>();

			for ( int i = 0; i < Population.MinSize; i += Crossover.ParentsNumber )
			{
				var selectedParents = parents.Skip (i).Take (Crossover.ParentsNumber).ToList ();

				// If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
                // Checks if the number of selected parents is equal which the crossover expect, because the in the of the list we can
                // have some rest chromosomes.
				if (selectedParents.Count == Crossover.ParentsNumber && RandomizationProvider.Current.GetDouble () <= CrossoverProbability) {
					offspring.AddRange ( Crossover.Cross (selectedParents));
				}
			}

			return offspring;
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

		/// <summary>
		/// Reinsert the specified offspring and parents.
		/// </summary>
		/// <param name="offspring">offspring.</param>
		/// <param name="parents">Parents.</param>
        private IList<IChromosome> Reinsert(IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return Reinsertion.SelectChromosomes(Population, offspring, parents); 
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Dispose()
        {
            if (m_threadPool != null)
            {
                m_threadPool.Dispose();
            }
        }
		#endregion       
    }
}

