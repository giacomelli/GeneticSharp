namespace GeneticSharp.Domain
{
    /// <summary>
    /// The possible states for a genetic algorithm.
    /// </summary>
    public enum GeneticAlgorithmState
    {
        /// <summary>
        /// The Genetic Algorithm has not been started yet.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The Genetic Algorithm has been started and is running.
        /// </summary>
        Started,

        /// <summary>
        /// The Genetic Algorithm has been stopped and is not running.
        /// </summary>
        Stopped,

        /// <summary>
        /// The Genetic Algorithm has been resumed after a stop or termination reach and is running.
        /// </summary>
        Resumed,

        /// <summary>
        /// The Genetic Algorithm has reach the termination condition and is not running.
        /// </summary>
        TerminationReached
    }
}