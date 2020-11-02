using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

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
    }
}
