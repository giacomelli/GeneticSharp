using System;
using NUnit.Framework;
using System.Threading;

namespace GeneticSharp.Infrastructure.Threading.UnitTests
{
	[TestFixture()]
	public class SmartThreadPoolTaskExecutorTest
	{
		[Test()]
		public void Start_ManyTasks_ParallelExecuted ()
		{
			var pipeline = "";
			var target = new SmartThreadPoolTaskExecutor ();
			target.Add (() => {
				pipeline += "1";
			});
			target.Add (() => {
				Thread.Sleep(10);
				pipeline += "2";
			});
			target.Add (() => {
				pipeline += "3";
			});

			target.Start ();
			Assert.AreEqual ("132", pipeline);
		}
	}
}

