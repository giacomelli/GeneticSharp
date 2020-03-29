using System.ComponentModel;

namespace GeneticSharp.Domain.Terminations {
    /// <summary>
    /// Mutation count termination.
    /// <remarks>
    /// The genetic algorithm will terminate when it exceeds the expected mutation count.
    /// </remarks>
    /// </summary>
    [DisplayName("Mutation Count")]
    public class MutationCountTermination : TerminationBase {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MutationCountTermination"/> class.
        /// </summary>
        /// <remarks>
        /// The default mutation count threshold is 1000.
        /// </remarks>
        public MutationCountTermination() : this(1000) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutationCountTermination"/> class.
        /// </summary>
        /// <param name="mutationCountThreshold">The mutation count threshold to consider whether the termination has been reached.</param>
        public MutationCountTermination(int mutationCountThreshold) {
            MutationCountThreshold = mutationCountThreshold;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the mutation count threshold to consider that termination has been reached.
        /// </summary>
        /// <value>The generation number.</value>
        public int MutationCountThreshold { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="geneticAlgorithm">The genetic algorithm.</param>
        protected override bool PerformHasReached(IGeneticAlgorithm geneticAlgorithm) {
            return geneticAlgorithm.MutationCount >= MutationCountThreshold;
        }
        #endregion
    }
}
