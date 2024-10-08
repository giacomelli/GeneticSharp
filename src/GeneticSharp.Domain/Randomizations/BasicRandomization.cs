using System;
using System.Threading;

namespace GeneticSharp
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
        private static int? _seed;
       
        /// <summary> 
        /// Random number generator 
        /// </summary> 
        private static ThreadLocal<Random> _threadRandom = new ThreadLocal<Random>(NewRandom);
        
        /// <summary> 
        /// Creates a new instance of Random. The seed is derived 
        /// from a global (static) instance of Random, rather 
        /// than time. 
        /// </summary> 
        private static Random NewRandom()
        {
            lock (_globalLock)
            {
                return new Random(_seed ?? _globalRandom.Next());
            }
        }

        /// <summary> 
        /// Returns an instance of Random which can be used freely 
        /// within the current thread. 
        /// </summary> 
        private static Random Instance { get { return _threadRandom.Value!; } }

        /// <summary>
        /// Resets the pseudorandom number generator (System.Random) initial seed.
        /// </summary>
        /// <param name="seed">The seed. Use null to reset to default one.</param>
        /// <remarks>
        /// Can be useful on situation where you need to generate the same result with the same parameter.
        /// </remarks>
        public static void ResetSeed(int? seed)
        {
            _seed = seed;            
            _threadRandom = new ThreadLocal<Random>(NewRandom);
        }

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