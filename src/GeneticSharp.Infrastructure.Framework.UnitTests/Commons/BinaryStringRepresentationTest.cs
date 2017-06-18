using System;
using NUnit.Framework;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Commons
{
	[TestFixture]
	public class BinaryStringRepresentationTest
	{
		#region Int64
		[Test]
		public void ToRepresentation_Int64_Representation()
		{
			var actual = BinaryStringRepresentation.ToRepresentation(1L);
			Assert.AreEqual("1", actual);

			actual = BinaryStringRepresentation.ToRepresentation(1L, 16);
			Assert.AreEqual("0000000000000001", actual);

			actual = BinaryStringRepresentation.ToRepresentation(10L);
			Assert.AreEqual("1010", actual);

			actual = BinaryStringRepresentation.ToRepresentation(360L);
			Assert.AreEqual("101101000", actual);

			for (long i = 0; i <= 360; i++)
			{
				var stringRepresentation = BinaryStringRepresentation.ToRepresentation(i, 9);
				Assert.AreEqual(9, stringRepresentation.Length);
				Assert.AreEqual(i, BinaryStringRepresentation.ToInt64(stringRepresentation));
			}
		}

		[Test]
		public void ToRepresentation_ValuesLenghtDiffTotalBits_Exception()
		{
			Assert.Catch<ArgumentException>(delegate
			{
				BinaryStringRepresentation.ToRepresentation(new long[] { 1L, 2L, 3L }, new int[] { 4, 6 });
			});
		}

		[Test]
		public void ToRepresentation_Int64Array_Representations()
		{
			var actual = BinaryStringRepresentation.ToRepresentation(new long[] { 1L, 2L, 3L }, new int[] { 4, 6, 8 });
			Assert.AreEqual(3, actual.Length);
			Assert.AreEqual("0001", actual[0]);
			Assert.AreEqual("000010", actual[1]);
			Assert.AreEqual("00000011", actual[2]);
		}

		[Test]
		public void ToInt64_Representation_Int64()
		{
			var actual = BinaryStringRepresentation.ToInt64("1");
			Assert.AreEqual(1L, actual);

			actual = BinaryStringRepresentation.ToInt64("0000000000000001");
			Assert.AreEqual(1L, actual);

			actual = BinaryStringRepresentation.ToInt64("1010");
			Assert.AreEqual(10L, actual);
		}

		[Test]
		public void ToInt64_RepresentationLengthDiffThanTotalBitsSum_Exception()
		{
			Assert.Catch<ArgumentException>(delegate
			{
				BinaryStringRepresentation.ToInt64("000100001000000011", new int[] { 4, 6, 9 });
			});
		}

		[Test]
		public void ToInt64_RepresentationAndTotalBitsArray_Int64s()
		{
			var actual = BinaryStringRepresentation.ToInt64("000100001000000011", new int[] { 4, 6, 8 });
			Assert.AreEqual(3, actual.Length);
			Assert.AreEqual(1L, actual[0]);
			Assert.AreEqual(2L, actual[1]);
			Assert.AreEqual(3L, actual[2]);
		}
		#endregion

		#region Double
		[Test]
		public void ToRepresentation_DoubleAndTotalBits_LengthEqualsTotalBits()
		{
			var actual = BinaryStringRepresentation.ToRepresentation(1000, 10, 0);
			Assert.AreEqual(actual.Length, 10);
		}

		[Test]
		public void ToRepresentation_LongAndTotalBitsLowerThanBitsNeedToRepresentation_Exception()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				BinaryStringRepresentation.ToRepresentation(1000, 9);
			}, "The value 1000 needs 10 total bits to be represented.");

		}

		[Test]
		public void ToRepresentation_DoubleAndTotalBitsLowerThanBitsNeedToRepresentation_Exception()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				BinaryStringRepresentation.ToRepresentation(1000.00, 9, 0);
			}, "The value 1000.00 needs 10 total bits to be represented.");
		}

		[Test]
		public void ToRepresentation_Double_Representation()
		{
			var actual = BinaryStringRepresentation.ToRepresentation(1.1);
			Assert.AreEqual("1101110", actual);

			actual = BinaryStringRepresentation.ToRepresentation(1.1, 16);
			Assert.AreEqual("0000000001101110", actual);

			actual = BinaryStringRepresentation.ToRepresentation(10.123, 0, 3);
			Assert.AreEqual("10011110001011", actual);
		}

		[Test]
		public void ToRepresentation_DoubleArray_Representations()
		{
			var actual = BinaryStringRepresentation.ToRepresentation(
				new double[] { 1.1, 10.123 },
				new int[] { 16, 14 },
				new int[] { 2, 3 });

			Assert.AreEqual(2, actual.Length);
			Assert.AreEqual("0000000001101110", actual[0]);
			Assert.AreEqual("10011110001011", actual[1]);
		}

		[Test]
		public void ToDouble_Representation_Double()
		{
			var actual = BinaryStringRepresentation.ToDouble("1101110");
			Assert.AreEqual(1.1, actual);

			actual = BinaryStringRepresentation.ToDouble("0000000001101110");
			Assert.AreEqual(1.1, actual);

			actual = BinaryStringRepresentation.ToDouble("10011110001011", 3);
			Assert.AreEqual(10.123, actual);
		}

		[Test]
		public void ToRepresentation_NegativeDouble_Representation()
		{
			var actual = BinaryStringRepresentation.ToRepresentation(-1.1);

			// https://en.wikipedia.org/wiki/Signed_number_representations#Two.27s_complement
			Assert.AreEqual("1111111111111111111111111111111111111111111111111111111110010010", actual);

			actual = BinaryStringRepresentation.ToRepresentation(-1.1, 64);
			Assert.AreEqual("1111111111111111111111111111111111111111111111111111111110010010", actual);

			actual = BinaryStringRepresentation.ToRepresentation(-10.123, 0, 3);
			Assert.AreEqual("1111111111111111111111111111111111111111111111111101100001110101", actual);
		}


		[Test]
		public void ToDouble_NegativeRepresentation_Double()
		{
			var actual = BinaryStringRepresentation.ToDouble("1111111111111111111111111111111111111111111111111111111110010010");
			Assert.AreEqual(-1.1, actual);

			actual = BinaryStringRepresentation.ToDouble("1111111111111111111111111111111111111111111111111111111110010010");
			Assert.AreEqual(-1.1, actual);

			actual = BinaryStringRepresentation.ToDouble("1111111111111111111111111111111111111111111111111101100001110101", 3);
			Assert.AreEqual(-10.123, actual);
		}

		[Test]
		public void ToDouble_DiffArraysLengths_Exception()
		{
			var actual = Assert.Catch(() => BinaryStringRepresentation.ToDouble("000000000110111010011110001011", new int[] { 16, 14, 1 }, new int[] { 2, 3 }));
			Assert.AreEqual("The length of totalBits should be the same of the length of fractionBits.", actual.Message);

			actual = Assert.Catch(() => BinaryStringRepresentation.ToDouble("00000000011011101001111000101", new int[] { 16, 14, }, new int[] { 2, 3 }));
			Assert.AreEqual("The representation length should be the same of the sum of the totalBits.", actual.Message);
		}

		[Test]
		public void ToDouble_RepresentationAndTotalBitsArray_Doubles()
		{
			var actual = BinaryStringRepresentation.ToDouble("000000000110111010011110001011", new int[] { 16, 14 }, new int[] { 2, 3 });

			Assert.AreEqual(2, actual.Length);
			Assert.AreEqual(1.1, actual[0]);
			Assert.AreEqual(10.123, actual[1]);
		}

		[Test]
		public void ToDouble_RepresentationAndTotalBitsAndFractionZeroArray_Doubles()
		{
			var actual = BinaryStringRepresentation.ToDouble("1111101000111110100011111010001111101000", new int[] { 10, 10, 10, 10 }, new int[] { 0, 0, 0, 0 });

			Assert.AreEqual(4, actual.Length);
			Assert.AreEqual(1000, actual[0]);
			Assert.AreEqual(1000, actual[1]);
			Assert.AreEqual(1000, actual[2]);
			Assert.AreEqual(1000, actual[3]);
		}
		#endregion
	}
}