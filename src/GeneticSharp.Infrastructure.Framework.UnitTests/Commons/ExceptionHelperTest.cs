using System;
using GeneticSharp.Infrastructure.Framework.Commons;
using NUnit.Framework;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Commons
{

	[TestFixture()]
	public class ExceptionHelperTest
	{
		[Test()]
		public void ThrowIfNull_NotNull_NoException()
		{
			ExceptionHelper.ThrowIfNull("one", 1);
		}

		[Test()]
		public void ThrowIfNull_Null_Exception()
		{
			Assert.Throws<ArgumentNullException>(() => ExceptionHelper.ThrowIfNull("one", null));
		}

		[Test()]
		public void ThrowIfNullOrEmpty_NotNull_NoException()
		{
			ExceptionHelper.ThrowIfNullOrEmpty("one", "1");
		}

		[Test()]
		public void ThrowIfNullOrEmpty_Null_Exception()
		{
			Assert.Throws<ArgumentNullException>(() => ExceptionHelper.ThrowIfNullOrEmpty("one", null));
		}

		[Test()]
		public void ThrowIfNullOrEmpty_Empty_Exception()
		{
			Assert.Throws<ArgumentException>(() => ExceptionHelper.ThrowIfNullOrEmpty("one", ""));
		}
	}
}
