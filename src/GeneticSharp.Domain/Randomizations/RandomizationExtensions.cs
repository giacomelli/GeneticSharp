using System;

namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
    /// Extension methods for IRandomization.
    /// </summary>
    public static class RandomizationExtensions
    {
        /// <summary>
        /// Gets an even integer value between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The integer.</returns>
        /// <param name="randomization">The IRandomization implementation.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public static int GetEvenInt(this IRandomization randomization, int min, int max)
        {
            checked
            {
                var candidate = randomization.GetInt(min, max - 1);

                return candidate % 2 == 0 ? candidate : candidate + 1;
            }
        }

        /// <summary>
        /// Gets an odd integer value between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The integer.</returns>
        /// <param name="randomization">The IRandomization implementation.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public static int GetOddInt(this IRandomization randomization, int min, int max)
        {
            checked
            {
                var candidate = randomization.GetInt(min, max - 1);

                return candidate % 2 != 0 ? candidate : candidate + 1;
            }
        }

        /// <summary>
        /// Gets a normally distributed random double from a uniform distribution using the Box-Muller algorithm.
        /// </summary>
        /// <returns>a random double</returns>
        /// <param name="randomization">The IRandomization implementation.</param>
        /// <param name="mean">The mean of the normal distribution</param>
        /// <param name="std">The standard deviation of the normal distribution</param>
        public static double GetNormal(this IRandomization randomization, double mean, double std)
        {
            checked
            {
                var u1 = randomization.GetDouble();
                var u2 = randomization.GetDouble();
                var z1 = Math.Sqrt((-2) * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);

                return std*z1+mean;
            }
        }
    }
}
