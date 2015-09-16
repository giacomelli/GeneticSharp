using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gdk;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Populations;
using Gtk;
using GeneticSharp.Domain.Crossovers;

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

        /// <summary>
        /// Gets the default crossover to sample.
        /// </summary>
        public abstract ICrossover DefaultCrossover { get; }
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
