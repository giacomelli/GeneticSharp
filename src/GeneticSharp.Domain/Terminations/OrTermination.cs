using System.ComponentModel;
using System.Linq;

namespace GeneticSharp.Domain.Terminations
{
    /// <summary>
    /// An termination where you can combine others ITerminations with a OR logical operator.
    /// </summary>
    [DisplayName("Or")]
    public class OrTermination : LogicalOperatorTerminationBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OrTermination"/> class.
        /// </summary>
        /// <param name="terminations">The terminations.</param>
        public OrTermination(params ITermination[] terminations)
            : base(terminations)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <param name="geneticAlgorithm">The genetic algorithm.</param>
        /// <returns>
        /// True if termination has been reached, otherwise false.
        /// </returns>
        protected override bool PerformHasReached(IGeneticAlgorithm geneticAlgorithm)
        {
            return Terminations.Any(t => t.HasReached(geneticAlgorithm));
        }
        #endregion
    }
}
