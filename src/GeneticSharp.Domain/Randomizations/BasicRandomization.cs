using System;
using System.Threading;

namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
    /// An IRandomization implementation using System.Random has pseudo-number generator.
    /// </summary>
    /// <remarks>
    /// https://codeblog.jonskeet.uk/2009/11/04/revisiting-randomness/
    /// </remarks>
    public class BasicRandomization : RandomizationBase
    {
        private static readonly Random _globalRandom = new Random();
        private static readonly object _globalLock = new object();
       
        /// <summary> 
        /// Random number generator 
        /// </summary> 
        private static readonly ThreadLocal<Random> s_threadRandom = new ThreadLocal<Random>(NewRandom);

        /// <summary> 
        /// Creates a new instance of Random. The seed is derived 
        /// from a global (static) instance of Random, rather 
        /// than time. 
        /// </summary> 
        private static Random NewRandom()
        {
            lock (_globalLock)
            {
                return new Random(_globalRandom.Next());
            }
        }

        /// <summary> 
        /// Returns an instance of Random which can be used freely 
        /// within the current thread. 
        /// </summary> 
        private static Random Instance { get { return s_threadRandom.Value; } }

        /// <summary>
        /// Gets an integer value between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The integer.</returns>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public override int GetInt(int min, int max)
        {
            return Instance.Next(min, max);            
        }

        /// <summary>
        /// Gets a float value between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        /// The float value.
        /// </returns>
        public override float GetFloat()
        {
            return (float)Instance.NextDouble();
        }

        /// <summary>
        /// Gets a double value between 0.0 and 1.0.
        /// </summary>
        /// <returns>The double value.</returns>
        public override double GetDouble()
        {
            return Instance.NextDouble();
        }       
    }
}