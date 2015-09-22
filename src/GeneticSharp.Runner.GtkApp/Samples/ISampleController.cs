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
    /// Defines an interface for sample controller.
    /// </summary>
    public interface ISampleController
    {
        #region Events
        /// <summary>
        /// Occurs when the sample is reconfigured in the config widget.
        /// </summary>
        event EventHandler Reconfigured;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        SampleContext Context { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the config widget.
        /// </summary>
        /// <returns>The config widget.</returns>
        Widget CreateConfigWidget();

        /// <summary>
        /// Creates the fitness.
        /// </summary>
        /// <returns>The fitness.</returns>
        IFitness CreateFitness();

        /// <summary>
        /// Creates the chromosome.
        /// </summary>
        /// <returns>The chromosome.</returns>
        IChromosome CreateChromosome();

        /// <summary>
        /// Creates the crossover.
        /// </summary>
        /// <returns>The crossover.</returns>
        ICrossover CreateCrossover();

        /// <summary>
        /// Creates the mutation.
        /// </summary>
        /// <returns>The mutation.</returns>
        IMutation CreateMutation();

        /// <summary>
        /// Creates the selection.
        /// </summary>
        /// <returns>The selection.</returns>
        ISelection CreateSelection();

        /// <summary>
        /// Creates the termination.
        /// </summary>
        /// <returns>The termination.</returns>
        ITermination CreateTermination();

        /// <summary>
        /// Resets the sample.
        /// </summary>
        void Reset();

        /// <summary>
        /// Updates the sample.
        /// </summary>
        void Update();

        /// <summary>
        /// Draws the sample.
        /// </summary>
        void Draw();
        #endregion
    }
}
