using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelperSharp;

namespace GeneticSharp.Domain.Crossovers
{
	/// <summary>
	/// Exception throw when an error occurs during the execution of cross.
	/// </summary>
    [Serializable]
    public class CrossoverException : Exception
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.CrossoverException"/> class.
		/// </summary>
		/// <param name="crossover">The crossover where ocurred the error.</param>
		/// <param name="message">The error message.</param>
        public CrossoverException(ICrossover crossover, string message)
            : base("{0}: {1}".With(crossover.GetType().Name, message))
		{            
            Crossover = crossover;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.CrossoverException"/> class.
        /// </summary>
        /// <param name="crossover">The crossover where ocurred the error.</param>
        /// <param name="message">The error message.</param>
		/// <param name="innerException">The inner exception.</param>
        public CrossoverException(ICrossover crossover, string message, Exception innerException)
            : base("{0}: {1}".With(crossover.GetType().Name, message), innerException)
        {
            Crossover = crossover;
        }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the crossover.
		/// </summary>
		/// <value>The crossover.</value>
        public ICrossover Crossover { get; private set; }
		#endregion
	}
}