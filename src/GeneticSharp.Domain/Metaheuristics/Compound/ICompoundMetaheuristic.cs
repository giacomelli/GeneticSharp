using System.Collections.Generic;
using System.Text;
using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace GeneticSharp.Domain.Metaheuristics.Compound
{
    /// <summary>
    /// Interface for metaheuristics built from compounding available metaheuristic primitives
    /// </summary>
   public interface ICompoundMetaheuristic
    {

        /// <summary>
        /// Creates a compound metaheuristic from local state
        /// </summary>
        /// <returns>a compound metaheuristic</returns>
        IContainerMetaHeuristic Build();


    }
}
