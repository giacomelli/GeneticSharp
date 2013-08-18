using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using HelperSharp;
using Amib.Threading;
using System.Linq;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain
{
	public class GeneticAlgorithm
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
		/// <param name="minSize">The minimum size (chromosomes).</param>
		/// <param name="maxSize">The maximum size (chromosomes).</param>
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
		#endregion

		#region Methods
		public bool Evolve(int timeoutPerGeneration = 0)
		{
			bool terminationConditionReached = false;

			do {
				terminationConditionReached = EvolveOneGeneration (timeoutPerGeneration);
			} while(!terminationConditionReached);

			return terminationConditionReached;
		}

		/// <summary>
		/// Aborts the evolution.
		/// </summary>
		/// <param name="timeout">Timeout to wait to abort.</param>
		public void AbortEvolution (int timeout = 60000)
		{
			if (m_threadPool != null) {
				m_threadPool.Shutdown (true, timeout);
			}
		}

		private bool EvolveOneGeneration (int timeout = 0)
		{
			if (Population.Generations.Count == 0) {
				Population.CreateInitialGeneration ();
				EvaluateFitness (timeout);
				Population.CurrentGeneration.Chromosomes = SelectParents ();
			} else {
				EvaluateFitness (timeout);
				Population.CreateNewGeneration(SelectParents ());
			}

			Mutate (Cross ());
			EvaluateFitness(timeout);
			Population.ElectBestChromosome();
			Population.FinalizeGeneration(Selection);

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
				EvaluateFitnessLinear(timeout);
			}
		}

		/// <summary>
		/// Evaluates the fitness linear.
		/// </summary>
		/// <param name="timeout">Timeout.</param>
		private void EvaluateFitnessLinear(int timeout)
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
		private IList<IChromosome> Cross ()
		{
			var children = new List<IChromosome>();

			for ( int i = 0; i < Population.MinSize; i += Crossover.ParentsNumber )
			{
				if (RandomizationProvider.Current.GetDouble() <= CrossoverProbability)
				{
					var child = Crossover.Cross (Population.CurrentGeneration.Chromosomes.Skip(i).Take(Crossover.ParentsNumber).ToList());
					children.AddRange (child);
				}
			}

			foreach (var c in children) {
				Population.CurrentGeneration.Chromosomes.Add (c);
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

