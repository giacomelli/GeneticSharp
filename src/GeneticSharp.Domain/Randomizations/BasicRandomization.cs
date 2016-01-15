using System;
using System.Threading;

namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
    /// An IRandomization implementation using System.Random has pseudo-number generator.
    /// </summary>
    public class BasicRandomization : RandomizationBase
    {
        #region Fields   
        // TODO: change to ThreadLocal when we migrate GeneticSharp to .NET 4.0+.
        // http://codeblog.jonskeet.uk/2009/11/04/revisiting-randomness/
        private static int s_seed = Environment.TickCount;

        [ThreadStatic]
        private static Random s_random;
        #endregion

        #region Properties
        private static Random Random
        {
            get
            {
                if (s_random == null)
                {
                    s_random = new Random(Interlocked.Increment(ref s_seed));
                }

                return s_random;
            }
        }
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
            return Random.Next(min, max);            
        }

        /// <summary>
        /// Gets a float value between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        /// The float value.
        /// </returns>
        public override float GetFloat()
        {
            return (float)Random.NextDouble();
        }

        /// <summary>
        /// Gets a double value between 0.0 and 1.0.
        /// </summary>
        /// <returns>The double value.</returns>
        public override double GetDouble()
        {
            return Random.NextDouble();
        }       
        #endregion
    }
}
