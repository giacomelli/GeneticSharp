using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Crossovers
{

    /// <summary>
    /// A general interface to define geometry embeddings. They are responsible for mapping gene-space into a target metric-space, in order to use a geometric operator
    /// </summary>
    /// <typeparam name="TValue">The base type of the metric space (typically a .Net value type)</typeparam>
    public interface IGeometryEmbedding<TValue>
    {
        ///// <summary>
        ///// An embedding can work differently if working with ordered chromosomes/crossover.
        ///// This property allows switching between those two modes.
        ///// </summary>
        //bool IsOrdered { get; set; }

        /// <summary>
        /// Converts offspring values in metric space into an offspring individual in gene space
        /// </summary>
        /// <param name="parents">The original offspring's parent individuals </param>
        /// <param name="offSpringValues">the metric-space values for the offspring to create</param>
        /// <returns>the converted offspring chromosome individual</returns>
        IChromosome MapFromGeometry(IList<IChromosome> parents, IList<TValue> offSpringValues);

        /// <summary>
        /// Imports a parent individual from gene space into metric-space
        /// </summary>
        /// <param name="parent">the parent to convert</param>
        /// <returns>the converted metric-space vector representing the parent</returns>
        IList<TValue> MapToGeometry(IChromosome parent);

    }
}