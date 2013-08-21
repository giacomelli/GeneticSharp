using System;
using System.Collections.Generic;
using System.Linq;
using Amib.Threading;
using HelperSharp;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

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
        /// Evolves the genetic algorithm using population, fitness, selection, crossover, mutation and termination configured.
        /// </summary>
        /// <returns>True if termination condition has been reached.</returns>
        public bool Evolve()
        {
            return Evolve(0);
        }

        /// <summary>
        /// Evolves the genetic algorithm using population, fitness, selection, crossover, mutation and termination configured.
        /// </summary>
        /// <param name="timeoutPerGeneration">The timeout per generation.</param>
        /// <returns>True if termination condition has been reached.</returns>
		public bool Evolve(int timeoutPerGeneration)
		{
			var startDateTime = DateTime.Now;
			bool terminationConditionReached = false;
            
            Population.CreateInitialGeneration();
			if (EndCurrentGeneration (timeoutPerGeneration)) {
				return true;
			}

			do {
				terminationConditionReached = EvolveOneGeneration (timeoutPerGeneration);
				TimeEvolving = DateTime.Now - startDateTime;

			} while(!terminationConditionReached);

			return terminationConditionReached;
		}

        /// <summary>
        /// Aborts the evolution.
        /// <remarks>
        /// The default timeout is 60000 milliseconds.
        /// </remarks>
        /// </summary>
        public void AbortEvolution()
        {
            AbortEvolution(60000);
        }

		/// <summary>
		/// Aborts the evolution.
		/// </summary>
		/// <param name="timeout">Timeout to wait to abort.</param>
		public void AbortEvolution (int timeout)
		{
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
            var children = Cross(parents);
            Mutate(children);            
            Population.CreateNewGeneration(children);
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
			var children = new List<IChromosome>();

			for ( int i = 0; i < Population.MinSize; i += Crossover.ParentsNumber )
			{
				var selectedParents = parents.Skip (i).Take (Crossover.ParentsNumber).ToList ();

				//  If match the probabilith cross is made, else not the offspring is an exact copy of the parents.
				if (RandomizationProvider.Current.GetDouble () <= CrossoverProbability) {
					children.AddRange ( Crossover.Cross (selectedParents));
				} else {
					children.AddRange (selectedParents.Select(c => c.Clone()));
				}
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

