using System;

namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
    /// An IRandomization implementation using System.Random has pseudo-number generator.
    /// </summary>
    public class BasicRandomization : RandomizationBase
    {
        #region Fields
        private Random m_random = new Random(DateTime.Now.Millisecond);
        #endregion

        #region Methods
        /// <summary>
        /// Gets an integer value between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The integer.</returns>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public override int GetInt(int min, int max)
        {
            return m_random.Next(min, max);
        }

        /// <summary>
        /// Gets a float value between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        /// The float value.
        /// </returns>
        public override float GetFloat()
        {
            return (float)m_random.NextDouble();
        }

        /// <summary>
        /// Gets a double value between 0.0 and 1.0.
        /// </summary>
        /// <returns>The double value.</returns>
        public override double GetDouble()
        {
            return m_random.NextDouble();
        }
        #endregion
    }
}
