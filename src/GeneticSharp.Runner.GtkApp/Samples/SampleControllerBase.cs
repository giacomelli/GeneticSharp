using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using Gtk;

namespace GeneticSharp.Runner.GtkApp.Samples
{
    /// <summary>
    /// Base class for sample controllers.
    /// </summary>
    public abstract class SampleControllerBase : ISampleController
    {
        #region Events
        /// <summary>
        /// Occurs when the sample is reconfigured in the config widget.
        /// </summary>
        public event EventHandler Reconfigured;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public SampleContext Context { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the config widget.
        /// </summary>
        /// <returns>The config widget.</returns>
        public abstract Widget CreateConfigWidget();

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
        public abstract void Draw();

        /// <summary>
        /// Raises the reconfigured event.
        /// </summary>
        protected void OnReconfigured()
        {
            if (Reconfigured != null)
            {
                Reconfigured(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
