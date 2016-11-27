using System;
using HelperSharp;

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
		public static string ToRepresentation (long value, int totalBits = 0)
		{
			return Convert.ToString (value, 2).PadLeft(totalBits, '0');
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
		/// Converts from double to string representation .
		/// </summary>
		/// <returns>The representation.</returns>
		/// <param name="value">Value.</param>
		/// <param name="totalBits">Total bits.</param>
		/// <param name="fractionBits">Fraction (scale) bits.</param>
		public static string ToRepresentation (double value, int totalBits = 0, int fractionBits = 2)
		{
			var longValue = Convert.ToInt64 (value * Math.Pow (10, fractionBits));
			return ToRepresentation (longValue, totalBits);
		}

		/// <summary>
		/// Converts from string representation to dooubl.
		/// </summary>
		/// <returns>The double.</returns>
		/// <param name="representation">Representation.</param>
		/// <param name="fractionBits">Fraction (scale) bits.</param>
		public static double ToDouble(string representation, int fractionBits = 2)
		{
			double longValue = ToInt64 (representation);

			return longValue / Math.Pow (10, fractionBits);
		}
	}
}

