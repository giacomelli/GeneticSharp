namespace GeneticSharp
{
    /// <summary>
    /// The randomization provider use for all elements of current genetic algorithm execution.
    /// </summary>
    public static class RandomizationProvider
    {
        /// <summary>
        /// Initializes static members of the <see cref="RandomizationProvider"/> class.
        /// </summary>
        static RandomizationProvider()
        {
            Current = new FastRandomRandomization();
        }

        /// <summary>
        /// Gets or sets the current IRandomization implementation.
        /// </summary>
        /// <value>The current.</value>
        public static IRandomization Current { get; set; }
    }
}