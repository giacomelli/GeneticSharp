using System;
using System.Linq;
using GeneticSharp.Infrastructure.Framework.Texts;

namespace GeneticSharp.Infrastructure.Framework.Commons
{
	/// <summary>
	/// Binary string representation.
	/// </summary>
	public static class BinaryStringRepresentation
	{
		/// <summary>
		/// Converts the long value to the binary string representation.
		/// </summary>
		/// <returns>The representation.</returns>
		/// <param name="value">Value.</param>
		/// <param name="totalBits">Total bits.</param>
		public static string ToRepresentation(long value, int totalBits = 0)
		{
			return ToRepresentation(value, totalBits, true);
		}

		/// <summary>
		/// Converts the long value to the binary string representation.
		/// </summary>
		/// <returns>The representation.</returns>
		/// <param name="value">Value.</param>
		/// <param name="totalBits">Total bits.</param>
		/// <param name="throwsException">If should throws an exception when the total bits is lower than needed by the value.</param>
		public static string ToRepresentation(long value, int totalBits, bool throwsException)
		{
			var result = Convert.ToString(value, 2).PadLeft(totalBits, '0');

			if (throwsException && totalBits > 0 && result.Length > totalBits)
			{
				throw new ArgumentException("The value {0} needs {1} total bits to be represented.".With(value, result.Length), nameof(value));
			}

			return result;
		}

		/// <summary>
		/// Converts the long values to the binary string representations.
		/// </summary>
		/// <returns>The representation.</returns>
		/// <param name="values">Values.</param>
		/// <param name="totalBits">Total bits.</param>
		public static string[] ToRepresentation(long[] values, int[] totalBits)
		{
			return ToRepresentation<long>(values, totalBits, totalBits, (v, t, f) => ToRepresentation(v, t));
		}

		/// <summary>
		/// Converts from string representation to Int64.
		/// </summary>
		/// <returns>The int64.</returns>
		/// <param name="representation">Representation.</param>
		public static long ToInt64(string representation)
		{
			return Convert.ToInt64(representation, 2);
		}

		/// <summary>
		/// Converts from string representation to Int64[].
		/// </summary>
		/// <returns>The int64.</returns>
		/// <param name="representation">Representation.</param>
		/// <param name="totalBits">Total bits.</param>
		public static long[] ToInt64(string representation, int[] totalBits)
		{
			ExceptionHelper.ThrowIfNullOrEmpty("representation", representation);

			if (representation.Length != totalBits.Sum(b => b))
			{
				throw new ArgumentException("The representation length should be the same of the sum of the totalBits.");
			}

			var int64s = new long[totalBits.Length];
			int startIndex = 0;

			for (int i = 0; i < totalBits.Length; i++)
			{
				var currentTotalBits = totalBits[i];
				int64s[i] = ToInt64(representation.Substring(startIndex, currentTotalBits));
				startIndex += currentTotalBits;
			}

			return int64s;
		}

		/// <summary>
		/// Converts from double to string representation .
		/// </summary>
		/// <returns>The representation.</returns>
		/// <param name="value">Value.</param>
		/// <param name="totalBits">Total bits.</param>
		/// <param name="fractionBits">Fraction (scale) bits.</param>
		public static string ToRepresentation(double value, int totalBits = 0, int fractionBits = 2)
		{
			var longValue = Convert.ToInt64(value * Math.Pow(10, fractionBits));

			var result = ToRepresentation(longValue, totalBits, false);

			if (totalBits > 0 && result.Length > totalBits)
			{
				throw new ArgumentException("The value {0} needs {1} total bits to be represented.".With(value, result.Length), nameof(value));
			}

			return result;
		}

		/// <summary>
		/// Converts from doubles to strings representation .
		/// </summary>
		/// <returns>The representations.</returns>
		/// <param name="values">The values..</param>
		/// <param name="totalBits">The total bits.</param>
		/// <param name="fractionBits">The fraction (scale) bits.</param>
		public static string[] ToRepresentation(double[] values, int[] totalBits, int[] fractionBits)
		{
			return ToRepresentation<double>(values, totalBits, fractionBits, (v, t, f) => ToRepresentation(v, t, f));
		}

		/// <summary>
		/// Converts from string representation to double.
		/// </summary>
		/// <returns>The double.</returns>
		/// <param name="representation">Representation.</param>
		/// <param name="fractionBits">Fraction (scale) bits.</param>
		public static double ToDouble(string representation, int fractionBits = 2)
		{
			double longValue = ToInt64(representation);

			return longValue / Math.Pow(10, fractionBits);
		}

		/// <summary>
		/// Converts from string representation to doubles.
		/// </summary>
		/// <returns>The double.</returns>
		/// <param name="representation">Representation.</param>
		/// <param name="totalBits">Total bits.</param>
		/// <param name="fractionBits">Fraction (scale) bits.</param>
		public static double[] ToDouble(string representation, int[] totalBits, int[] fractionBits)
		{
			return ToValue<double>(
				representation,
				totalBits,
				fractionBits,
				new double[totalBits.Length],
				(r, f) => ToDouble(r, f));
		}

		private static TValue[] ToValue<TValue>(string representation, int[] totalBits, int[] fractionBits, TValue[] values, Func<string, int, TValue> toValue)
		{
			ExceptionHelper.ThrowIfNullOrEmpty("representation", representation);

			if (totalBits.Length != fractionBits.Length)
			{
				throw new ArgumentException("The length of totalBits should be the same of the length of fractionBits.");
			}

			if (representation.Length != totalBits.Sum(b => b))
			{
				throw new ArgumentException("The representation length should be the same of the sum of the totalBits.");
			}

			int startIndex = 0;

			for (int i = 0; i < totalBits.Length; i++)
			{
				var currentTotalBits = totalBits[i];
				values[i] = toValue(representation.Substring(startIndex, currentTotalBits), fractionBits[i]);
				startIndex += currentTotalBits;
			}

			return values;
		}

		private static string[] ToRepresentation<TValue>(TValue[] values, int[] totalBits, int[] fractionBits, Func<TValue, int, int, string> toRepresentation)
		{
			if (values.Length != totalBits.Length || values.Length != fractionBits.Length)
			{
				throw new ArgumentException("The length of values should be the same of the length of totalBits and fractionBits.");
			}

			var representations = new string[values.Length];

			for (int i = 0; i < values.Length; i++)
			{
				representations[i] = toRepresentation(values[i], totalBits[i], fractionBits[i]);
			}

			return representations;
		}
	}
}

