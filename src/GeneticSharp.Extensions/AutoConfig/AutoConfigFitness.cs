﻿using System;

namespace GeneticSharp.Extensions
{
    /// <summary>
    /// A fitness function to auto config another genetic algorithm.
    /// </summary>
    public sealed class AutoConfigFitness : IFitness
    {
        #region Fields
        private readonly IFitness m_targetFitness;
        private readonly IChromosome m_targetChromosome;
        #endregion

        #region Constructor               
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoConfigFitness"/> class.
        /// </summary>
        /// <param name="targetFitness">The target fitness.</param>
        /// <param name="targetChromosome">The target chromosome.</param>
        public AutoConfigFitness(IFitness targetFitness, IChromosome targetChromosome)
        {
            m_targetFitness = targetFitness;
            m_targetChromosome = targetChromosome;
            PopulationMinSize = 100;
            PopulationMaxSize = 100;
            Termination = new TimeEvolvingTermination(TimeSpan.FromSeconds(30));
            TaskExecutor = new LinearTaskExecutor();
        }
        #endregion

        #region Properties        
        /// <summary>
        /// Gets or sets the minimum size of the population.
        /// </summary>
        /// <value>
        /// The minimum size of the population.
        /// </value>
        public int PopulationMinSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the population.
        /// </summary>
        /// <value>
        /// The maximum size of the population.
        /// </value>
        public int PopulationMaxSize { get; set; }

        /// <summary>
        /// Gets or sets the termination.
        /// </summary>
        /// <value>
        /// The termination.
        /// </value>
        public ITermination Termination { get; set; }

        /// <summary>
        /// Gets or sets the task executor which will be used to execute fitness evaluation.
        /// </summary>
        public ITaskExecutor TaskExecutor { get; set; }
        #endregion

        #region Methods        
        /// <summary>
        /// Evaluates the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns>The chromosome fitness.</returns>
        public double Evaluate(IChromosome chromosome)
        {
            var autoConfigChromosome = (chromosome as AutoConfigChromosome)!;
            var selection = autoConfigChromosome.Selection;
            var crossover = autoConfigChromosome.Crossover;
            var mutation = autoConfigChromosome.Mutation;
            var population = new Population(PopulationMinSize, PopulationMaxSize, m_targetChromosome);

            var ga = new GeneticAlgorithm(population, m_targetFitness, selection, crossover, mutation);
            ga.Termination = Termination;
            ga.TaskExecutor = TaskExecutor;

            try
            {
                ga.Start();
            }
            catch (Exception)
            {
                // The selection, crossover and mutation combination is not valid,
                // so this chromossome should have a bad fitness.
                return 0;
            }

            return ga.BestChromosome!.Fitness!.Value;
        }
        #endregion
    }
}
