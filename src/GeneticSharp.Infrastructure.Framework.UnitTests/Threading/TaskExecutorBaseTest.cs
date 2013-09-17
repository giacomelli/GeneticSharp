using System;
using NUnit.Framework;
using GeneticSharp.Infrastructure.Framework.Threading;
using Rhino.Mocks;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Threading
{
	[TestFixture()]
	public class TaskExecutorBaseTest
	{
		[Test()]
		public void Add_Tasks_TasksAdded ()
		{
			var target = new StubTaskExecutor ();
			target.Add (() => {});
			target.Add (() => {});
			target.Add (() => {});

			Assert.AreEqual (3, target.GetTasks ().Count);
		}

		[Test()]
		public void Clear_Tasks_TasksClean ()
		{
			var target = new StubTaskExecutor ();
			target.Add (() => {});
			target.Add (() => {});
			target.Add (() => {});

			target.Clear ();
			Assert.AreEqual (0, target.GetTasks ().Count);
		}

		[Test()]
		public void Start_NoArgs_StopRequestedFalse ()
		{
			var target = new StubTaskExecutor ();
			target.Start ();
			Assert.IsFalse (target.GetStopRequested ());
			target.Stop ();
			target.Start ();
			Assert.IsFalse (target.GetStopRequested ());
		}

		[Test()]
		public void Stop_NoArgs_StopRequestedTrue ()
		{
			var target = new StubTaskExecutor ();
			target.Start ();
			target.Stop ();
			Assert.IsTrue (target.GetStopRequested ());
		}
	}
}

