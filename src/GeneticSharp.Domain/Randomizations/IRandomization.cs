using System.Diagnostics.CodeAnalysis;

namespace GeneticSharp.Domain.Randomizations
{
    /// <summary>
    /// Defines an interface for randomization.
    /// </summary>
    public interface IRandomization
    {
        #region Methods
        /// <summary>
        /// Gets an integer value between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The integer.</returns>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        int GetInt(int min, int max);

        /// <summary>
        /// Gets an integer array with values between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The integer array.</returns>
        /// <param name="length">The array length</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        int[] GetInts(int length, int min, int max);

        /// <summary>
        /// Gets an integer array with unique values between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The integer array.</returns>
        /// <param name="length">The array length</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        int[] GetUniqueInts(int length, int min, int max);

        /// <summary>
        /// Gets a float value between 0.0 and 1.0.
        /// </summary>
        /// <returns>The float value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        float GetFloat();

        /// <summary>
        /// Gets a float value between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The float value.</returns>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Max value.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        float GetFloat(float min, float max);

        /// <summary>
        /// Gets a double value between 0.0 and 1.0.
        /// </summary>
        /// <returns>The double value.</returns>
        double GetDouble();

        /// <summary>
        /// Gets a double value between minimum value (inclusive) and maximum value (exclusive).
        /// </summary>
        /// <returns>The double value.</returns>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Max value.</param>
        double GetDouble(double min, double max);
        #endregion
    }
}