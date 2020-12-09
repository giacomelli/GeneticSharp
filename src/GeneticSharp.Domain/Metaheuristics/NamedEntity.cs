using System;

namespace GeneticSharp.Domain.Metaheuristics
{

    public class NamedEntity
    {

        /// <summary>
        /// An ID to identify the current metaheuristic, useful for caching
        /// </summary>
        public Guid Guid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// An user-friendly name for the entity (Metaheuristic/Parameter)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A user-friendly description of what the Metaheuristic/parameter does 
        /// </summary>
        public string Description { get; set; }

    }
}