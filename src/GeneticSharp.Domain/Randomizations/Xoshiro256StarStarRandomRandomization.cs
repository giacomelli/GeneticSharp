using System.Threading;
using Redzen.Random;

namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
    /// An IRandomization using Xoshiro256StarStarRandomRandomization as pseudo-number generator.
    /// <see href="https://github.com/colgreen/Redzen"/>
    /// </summary>
    public class Xoshiro256StarStarRandomRandomization : RandomizationBase
    {
        private static readonly Xoshiro256StarStarRandomFactory _globalRandomFactory = new Xoshiro256StarStarRandomFactory();

        /// <summary> 
        /// Random number generator 
        /// </summary> 
        private static readonly ThreadLocal<Xoshiro256StarStarRandom> _threadRandom = new ThreadLocal<Xoshiro256StarStarRandom>(NewRandom);

        /// <summary> 
        /// Creates a new instance of XorShiftRandom.
        /// </summary> 
        private static Xoshiro256StarStarRandom NewRandom()
        {
            // XorShiftRandomFactory by default has thread safe random seed source
            return _globalRandomFactory.Create() as Xoshiro256StarStarRandom;
        }

        /// <summary> 
        /// Returns an instance of Random which can be used freely 
        /// within the current thread. 
        /// </summary> 
        private static Xoshiro256StarStarRandom Instance { get { return _threadRandom.Value; } }

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
