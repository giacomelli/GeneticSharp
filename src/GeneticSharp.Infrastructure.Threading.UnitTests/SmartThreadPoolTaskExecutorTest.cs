using System;
using NUnit.Framework;
using System.Threading;
using TestSharp;
using System.Threading.Tasks;

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

			var actual = target.Start ();
			Assert.IsTrue (actual);
			Assert.AreEqual ("132", pipeline);
		}

		[Test()]
		public void Start_Timeout_False ()
		{
			var pipeline = "1";
			var target = new SmartThreadPoolTaskExecutor ();
			target.Timeout = TimeSpan.FromMilliseconds (2);

			target.Add (() => {
				Thread.Sleep(10);
				pipeline += "2";
			});

			var actual = target.Start ();
			Assert.IsFalse (actual);

			Assert.AreEqual ("1", pipeline);
		}

		[Test()]
		public void Start_AnyTaskWithException_Exception ()
		{
			var pipeline = "";
			var target = new SmartThreadPoolTaskExecutor ();

			target.Add (() => {
				throw new Exception("1");
			});
			target.Add (() => {
				Thread.Sleep(5);
				pipeline += "2";
			});
			target.Add (() => {
				Thread.Sleep(5);
				pipeline += "3";
			});

			ExceptionAssert.IsThrowing (new Exception ("1"), () => {
				target.Start ();
			});
		}

		[Test()]
		public void Stop_ManyTasks_StopAll ()
		{
			var pipeline = "";
			var target = new SmartThreadPoolTaskExecutor ();
			target.Timeout = TimeSpan.FromMilliseconds(1);

			target.Add (() => {
				Thread.Sleep(5);
				pipeline += "1";
			});
			target.Add (() => {
				Thread.Sleep(5);
				pipeline += "2";
			});
			target.Add (() => {
				Thread.Sleep(5);
				pipeline += "3";
			});

			Parallel.Invoke (
				() => target.Start (),
				() => {
					Thread.Sleep(1);
					target.Stop();
				});

			Assert.AreEqual ("", pipeline);
		}
	}
}

