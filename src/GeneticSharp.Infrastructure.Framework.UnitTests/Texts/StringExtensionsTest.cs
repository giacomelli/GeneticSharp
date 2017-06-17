using System;
using GeneticSharp.Infrastructure.Framework.Texts;
using NUnit.Framework;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Texts
{
	[TestFixture]
	public class StringExtensionsTest
	{
		[Test()]
		public void RemovePunctuations_Punctuations_CleanString()
		{
			Assert.AreEqual("`1234567890-=qwertyuiop\\asdfghjklzxcvbnm/", "`1234567890-=q!wer?tyuiop,[]\\asdfghjkl;\'zxcvbnm,./".RemovePunctuations());
		}

		[Test()]
		public void With_SourceAndArgs_Formatted()
		{
			Assert.AreEqual("A1b2", "A{0}b{1}".With(1, 2));
		}
	}
}
