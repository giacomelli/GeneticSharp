using System;

namespace GeneticSharp.Runner.MauiApp.Samples
{
    /// <summary>
    /// Base class for sample controllers.
    /// </summary>
    public abstract class SampleControllerBase : ISampleController
    {
        /// <summary>
        /// Occurs when the sample is reconfigured in the config widget.
        /// </summary>
        public event EventHandler Reconfigured;
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public SampleContext Context { get; set; }

        /// <summary>
        /// Creates the config widget.
        /// </summary>
        /// <returns>The config widget.</returns>
        public abstract IView CreateConfigView();

        /// <summary>
        /// Creates the fitness.
        /// </summary>
        /// <returns>The fitness.</returns>
        public abstract IFitness CreateFitness();

        /// <summary>
        /// Creates the chromosome.
        /// </summary>
        /// <returns>The chromosome.</returns>
        public abstract IChromosome CreateChromosome();

        /// <summary>
        /// Creates the crossover.
        /// </summary>
        /// <returns>
        /// The crossover.
        /// </returns>
        public abstract ICrossover CreateCrossover();

        /// <summary>
        /// Creates the mutation.
        /// </summary>
        /// <returns>
        /// The mutation.
        /// </returns>
        public abstract IMutation CreateMutation();

        /// <summary>
        /// Creates the selection.
        /// </summary>
        /// <returns>
        /// The selection.
        /// </returns>
        public abstract ISelection CreateSelection();

        /// <summary>
        /// Creates the termination.
        /// </summary>
        /// <returns>
        /// The termination.
        /// </returns>
        public virtual ITermination CreateTermination()
        {
            return new FitnessStagnationTermination(1000);
        }

        /// <summary>
        /// Configure the Genetic Algorithm.
        /// </summary>
        /// <param name="ga">The genetic algorithm.</param>
        public virtual void ConfigGA(GeneticAlgorithm ga)
        {
        }

        /// <summary>
        /// Resets the sample.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Updates the sample.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Draws the sample.
        /// </summary>
        public abstract void Draw(ICanvas canvas);

        /// <summary>
        /// Raises the reconfigured event.
        /// </summary>
        protected void OnReconfigured()
        {
            Reconfigured?.Invoke(this, EventArgs.Empty);
        }        
    }
}
