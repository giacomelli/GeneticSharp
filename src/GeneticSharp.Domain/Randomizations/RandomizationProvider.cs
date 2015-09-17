namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
    /// The randomization provider use for all elements of current genetic algorithm execution.
    /// </summary>
    public static class RandomizationProvider
    {
        #region Constructors               
        /// <summary>
        /// Initializes static members of the <see cref="RandomizationProvider"/> class.
        /// </summary>
        static RandomizationProvider()
        {
            Current = new BasicRandomization();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current IRandomization implementation.
        /// </summary>
        /// <value>The current.</value>
        public static IRandomization Current { get; set; }
        #endregion
    }
}