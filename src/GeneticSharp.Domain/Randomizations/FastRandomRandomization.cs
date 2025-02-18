﻿using System;
using System.Threading;
using SharpNeatLib.Maths;

namespace GeneticSharp
{
    /// <summary>
    /// An IRandomization using FastRandom has pseudo-number generator.
    /// <see href="http://www.codeproject.com/Articles/9187/A-fast-equivalent-for-System-Random"/>
    /// </summary>
    public class FastRandomRandomization : RandomizationBase
    {
        private static readonly FastRandom _globalRandom = new FastRandom(DateTime.Now.Millisecond);
        private static readonly object _globalLock = new object();
        private static int? _seed;

        /// <summary> 
        /// Random number generator 
        /// </summary> 
        private static ThreadLocal<FastRandom> _threadRandom = new ThreadLocal<FastRandom>(NewRandom);

        /// <summary> 
        /// Creates a new instance of FastRandom. The seed is derived 
        /// from a global (static) instance of Random, rather 
        /// than time. 
        /// </summary> 
        private static FastRandom NewRandom()
        {
            lock (_globalLock)
            {
                return new FastRandom(_seed ?? _globalRandom.Next(0, int.MaxValue));
            }
        }

        /// <summary> 
        /// Returns an instance of Random which can be used freely 
        /// within the current thread. 
        /// </summary> 
        private static FastRandom Instance { get { return _threadRandom.Value!; } }

        /// <summary>
        /// Resets the pseudorandom number generator (SharpNeatLib.Maths.FastRandom) initial seed.
        /// </summary>
        /// <param name="seed">The seed. Use null to reset to default one.</param>
        /// <remarks>
        /// Can be useful on situation where you need to generate the same result with the same parameter.
        /// </remarks>
        public static void ResetSeed(int? seed)
        {
            _seed = seed;            
            _threadRandom = new ThreadLocal<FastRandom>(NewRandom);
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
