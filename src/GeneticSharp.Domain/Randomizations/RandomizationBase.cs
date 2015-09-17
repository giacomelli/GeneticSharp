using System;
using System.Linq;
using HelperSharp;

namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
    /// Base class for randomization.
    /// </summary>
    public abstract class RandomizationBase : IRandomization
    {
        #region Methods
        /// <summary>
        /// Gets an integer value between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The integer.</returns>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public abstract int GetInt(int min, int max);

        /// <summary>
        /// Gets a float value between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        /// The float value.
        /// </returns>
        public abstract float GetFloat();

        /// <summary>
        /// Gets a double value between 0.0 and 1.0.
        /// </summary>
        /// <returns>The double value.</returns>
        public abstract double GetDouble();

        /// <summary>
        /// Gets an integer array with values between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The integer array.</returns>
        /// <param name="length">The array length</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public virtual int[] GetInts(int length, int min, int max)
        {
            var ints = new int[length];

            for (int i = 0; i < length; i++)
            {
                ints[i] = GetInt(min, max);
            }

            return ints;
        }

        /// <summary>
        /// Gets an integer array with unique values between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The integer array.</returns>
        /// <param name="length">The array length</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public virtual int[] GetUniqueInts(int length, int min, int max)
        {
            var diff = max - min;

            if (diff < length)
            {
                throw new ArgumentOutOfRangeException(
                    "length",
                    "The length is {0}, but the possible unique values between {1} (inclusive) and {2} (exclusive) are {3}.".With(length, min, max, diff));
            }

            var orderedValues = Enumerable.Range(min, diff).ToList();
            var ints = new int[length];

            for (int i = 0; i < length; i++)
            {
                var removeIndex = RandomizationProvider.Current.GetInt(0, orderedValues.Count);
                ints[i] = orderedValues[removeIndex];
                orderedValues.RemoveAt(removeIndex);
            }

            return ints;
        }

        /// <summary>
        /// Gets a float value between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Max value.</param>
        /// <returns>
        /// The float value.
        /// </returns>
        public float GetFloat(float min, float max)
        {
            return min + ((max - min) * GetFloat());
        }

        /// <summary>
        /// Gets a double value between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The double value.</returns>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Max value.</param>
        public virtual double GetDouble(double min, double max)
        {
            return min + ((max - min) * GetDouble());
        }
        #endregion
    }
}
