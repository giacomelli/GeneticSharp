using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Infrastructure.Framework.Threading;
using HelperSharp;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

namespace GeneticSharp.Domain
{
    #region Enums
    /// <summary>
    /// The possible states for a genetic algorithm.
    /// </summary>
    public enum GeneticAlgorithmState
    {
        /// <summary>
        /// The GA has not been started yet.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The GA has been started and is running.
        /// </summary>
        Started,

        /// <summary>
        /// The GA has been stopped ans is not running.
        /// </summary>
        Stopped,

        /// <summary>
        /// The GA has been resumed after a stop or terminantion reach and is running.
        /// </summary>
        Resumed,

        /// <summary>
        /// The GA has reach the termination condition and is not running.
        /// </summary>
        TerminationReached
    }
    #endregion

    /// <summary>
    /// A genetic algorithm (GA) is a search heuristic that mimics the process of natural selection. 
    /// This heuristic (also sometimes called a metaheuristic) is routinely used to generate useful solutions 
    /// to optimization and search problems. Genetic algorithms belong to the larger class of evolutionary 
    /// algorithms (EA), which generate solutions to optimization problems using techniques inspired by natural evolution, 
    /// such as inheritance, mutation, selection, and crossover.
    /// 
    /// Genetic algorithms find application in bioinformatics, phylogenetics, computational science, engineering, 
    /// economics, chemistry, manufacturing, mathematics, physics, pharmacometrics, game development and other fields.
    /// 
    /// <see href="http://http://en.wikipedia.org/wiki/Genetic_algorithm">Wikipedia</see>
    /// </summary>
    public sealed class GeneticAlgorithm : IGeneticAlgorithm
    {
        #region Fields
        private bool m_stopRequested;
        private object m_lock = new object();
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
            Reinsertion = new ElitistReinsertion();
            Termination = new GenerationNumberTermination(1);

            CrossoverProbability = DefaultCrossoverProbability;
            MutationProbability = DefaultMutationProbability;
            TimeEvolving = TimeSpan.Zero;
            State = GeneticAlgorithmState.NotStarted;
            TaskExecutor = new LinearTaskExecutor();
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
        public ISelection Selection { get; set; }

        /// <summary>
        /// Gets the crossover operator.
        /// </summary>
        /// <value>The crossover.</value>
        public ICrossover Crossover { get; set; }

        /// <summary>
        /// Gets or sets the crossover probability.
        /// </summary>
        public float CrossoverProbability { get; set; }

        /// <summary>
        /// Gets the mutation operator.
        /// </summary>
        public IMutation Mutation { get; set; }

        /// <summary>
        /// Gets or sets the mutation probability.
        /// </summary>
        public float MutationProbability { get; set; }

        /// <summary>
        /// Gets or sets the reinsertion operator.
        /// </summary>
        public IReinsertion Reinsertion { get; set; }

        /// <summary>
        /// Gets or sets the termination condition.
        /// </summary>
        public ITermination Termination { get; set; }

        /// <summary>
        /// Gets the generations number.
        /// </summary>
        /// <value>The generations number.</value>
        public int GenerationsNumber { get { return Population.GenerationsNumber; } }

        /// <summary>
        /// Gets the best chromosome.
        /// </summary>
        /// <value>The best chromosome.</value>
        public IChromosome BestChromosome { get { return Population.BestChromosome; } }

        /// <summary>
        /// Gets the time evolving.
        /// </summary>
        public TimeSpan TimeEvolving { get; private set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public GeneticAlgorithmState State { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning
        {
            get
            {
                return State == GeneticAlgorithmState.Started || State == GeneticAlgorithmState.Resumed;
            }
        }

        /// <summary>
        /// Gets or sets the task executor which will be used to execute fitness evaluation.
        /// </summary>
        public ITaskExecutor TaskExecutor { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Starts the genetic algorithm using population, fitness, selection, crossover, mutation and termination configured.
        /// </summary>
        public void Start()
        {
            lock (m_lock)
            {
                State = GeneticAlgorithmState.Started;
                var startDateTime = DateTime.Now;
                Population.CreateInitialGeneration();
                TimeEvolving = DateTime.Now - startDateTime;
            }
            
            Resume();            
        }

        /// <summary>
        /// Resumes the last evolution of the genetic algorithm.
        /// <remarks>
        /// If genetic algorithm was not explicit Stop (calling Stop method), you will need provide a new extended Termination.
        /// </remarks>
        /// </summary>
        public void Resume()
        {            
            try
            {
                lock (m_lock)
                {
                    m_stopRequested = false;
                }

                if (Population.GenerationsNumber == 0)
                {
                    throw new InvalidOperationException("Attempt to resume a genetic algorithm which was not yet started.");
                }
                else if (Population.GenerationsNumber > 1)
                {
                    if (Termination.HasReached(this))
                    {
						throw new InvalidOperationException("Attempt to resume a genetic algorithm with a termination ({0}) already reached. Please, specify a new termination or extend the current one.".With(Termination));
                    }

                    State = GeneticAlgorithmState.Resumed;
                }

                if (EndCurrentGeneration())
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
                    terminationConditionReached = EvolveOneGeneration();
                    TimeEvolving += DateTime.Now - startDateTime;

                } while (!terminationConditionReached);
            }
            catch
            {
                State = GeneticAlgorithmState.Stopped;
                throw;
            }            
        }

        /// <summary>
        /// Stops the genetic algorithm..
        /// </summary>
        public void Stop()
        {            
            if (Population.GenerationsNumber == 0)
            {
                throw new InvalidOperationException("Attempt to stop a genetic algorithm which was not yet started.");
            }

            lock (m_lock)
            {
                m_stopRequested = true;                
            }            
        }

        /// <summary>
        /// Evolve one generation.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        private bool EvolveOneGeneration()
        {
            var parents = SelectParents();
            var offspring = Cross(parents);
            Mutate(offspring);
            var newGenerationChromosomes = Reinsert(offspring, parents);
            Population.CreateNewGeneration(newGenerationChromosomes);
            return EndCurrentGeneration();
        }

        /// <summary>
        /// Ends the current generation.
        /// </summary>
        /// <returns><c>true</c>, if current generation was ended, <c>false</c> otherwise.</returns>
        private bool EndCurrentGeneration()
        {
            EvaluateFitness();
            Population.EndCurrentGeneration();

            if (GenerationRan != null)
            {
                GenerationRan(this, EventArgs.Empty);
            }

            if (Termination.HasReached(this))
            {
                State = GeneticAlgorithmState.TerminationReached;

                if (TerminationReached != null)
                {
                    TerminationReached(this, EventArgs.Empty);
                }

                return true;
            }

            if (m_stopRequested)
            {
                TaskExecutor.Stop();
                State = GeneticAlgorithmState.Stopped;
            }

            return false;
        }

        /// <summary>
        /// Evaluates the fitness.
        /// </summary>
        private void EvaluateFitness()
        {
            try
            {
                var chromosomesWithoutFitness = Population.CurrentGeneration.Chromosomes.Where(c => !c.Fitness.HasValue).ToList();

                for (int i = 0; i < chromosomesWithoutFitness.Count; i++)
                {
                    var c = chromosomesWithoutFitness[i];

                    TaskExecutor.Add(() =>
                    {
                        RunEvaluateFitness(c);
                    });
                }

                if (!TaskExecutor.Start())
                {
                    throw new TimeoutException("The fitness evaluation rech the {0} timeout.".With(TaskExecutor.Timeout));
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
            finally
            {
                TaskExecutor.Stop();
                TaskExecutor.Clear();
            }

            Population.CurrentGeneration.Chromosomes = Population.CurrentGeneration.Chromosomes.OrderByDescending(c => c.Fitness.Value).ToList();
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
        private IList<IChromosome> SelectParents()
        {
            return Selection.SelectChromosomes(Population.MinSize, Population.CurrentGeneration);
        }

        /// <summary>
        /// Cross this instance.
        /// </summary>
        private IList<IChromosome> Cross(IList<IChromosome> parents)
        {
            var offspring = new List<IChromosome>();

            for (int i = 0; i < Population.MinSize; i += Crossover.ParentsNumber)
            {
                var selectedParents = parents.Skip(i).Take(Crossover.ParentsNumber).ToList();

                // If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
                // Checks if the number of selected parents is equal which the crossover expect, because the in the end of the list we can
                // have some rest chromosomes.
                if (selectedParents.Count == Crossover.ParentsNumber && RandomizationProvider.Current.GetDouble() <= CrossoverProbability)
                {
                    offspring.AddRange(Crossover.Cross(selectedParents));
                }
            }

            return offspring;
        }

        /// <summary>
        /// Mutate the specified chromosomes.
        /// </summary>
        /// <param name="chromosomes">Chromosomes.</param>
        private void Mutate(IList<IChromosome> chromosomes)
        {
            foreach (var c in chromosomes)
            {
                Mutation.Mutate(c, MutationProbability);
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
        #endregion
    }
}

