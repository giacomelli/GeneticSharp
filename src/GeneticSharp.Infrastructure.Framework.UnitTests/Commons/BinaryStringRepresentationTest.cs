using System;
using NUnit.Framework;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Commons
{
	[TestFixture]
	public class BinaryStringRepresentationTest
	{
		[Test]
		public void ToRepresentation_Int64_Representation ()
		{
			var actual = BinaryStringRepresentation.ToRepresentation (1L);
			Assert.AreEqual ("1", actual);

			actual = BinaryStringRepresentation.ToRepresentation (1L, 16);
			Assert.AreEqual ("0000000000000001", actual);

			actual = BinaryStringRepresentation.ToRepresentation (10L);
			Assert.AreEqual ("1010", actual);
		}

		[Test]
		public void ToInt64_Representation_Int64 ()
		{
			var actual = BinaryStringRepresentation.ToInt64 ("1");
			Assert.AreEqual (1L, actual);

			actual = BinaryStringRepresentation.ToInt64 ("0000000000000001");
			Assert.AreEqual (1L, actual);

			actual = BinaryStringRepresentation.ToInt64 ("1010");
			Assert.AreEqual (10L, actual);
		}

		[Test]
		public void ToRepresentation_Double_Representation ()
		{
			var actual = BinaryStringRepresentation.ToRepresentation (1.1);
			Assert.AreEqual ("1101110", actual);

			actual = BinaryStringRepresentation.ToRepresentation (1.1, 16);
			Assert.AreEqual ("0000000001101110", actual);

			actual = BinaryStringRepresentation.ToRepresentation (10.123, 0, 3);
			Assert.AreEqual ("10011110001011", actual);
		}

		[Test]
		public void ToDouble_Representation_Double ()
		{
			var actual = BinaryStringRepresentation.ToDouble ("1101110");
			Assert.AreEqual (1.1, actual);

			actual = BinaryStringRepresentation.ToDouble ("0000000001101110");
			Assert.AreEqual (1.1, actual);

			actual = BinaryStringRepresentation.ToDouble ("10011110001011", 3);
			Assert.AreEqual (10.123, actual);
		}
	}
}

