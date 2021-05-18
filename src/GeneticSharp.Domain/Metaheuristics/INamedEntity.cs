using System;

namespace GeneticSharp.Domain.Metaheuristics
{
    public interface INamedEntity
    {
        /// <summary>
        /// An ID to identify the current metaheuristic, useful for caching
        /// </summary>
        Guid Guid { get; set; }

        /// <summary>
        /// An user-friendly name for the entity (Metaheuristic/Parameter)
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A user-friendly description of what the Metaheuristic/parameter does 
        /// </summary>
        string Description { get; set; }
    }
}