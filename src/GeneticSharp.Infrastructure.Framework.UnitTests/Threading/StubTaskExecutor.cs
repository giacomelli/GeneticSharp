using System;
using GeneticSharp.Infrastructure.Framework.Threading;
using System.Collections.Generic;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Threading
{
	public class StubTaskExecutor : TaskExecutorBase
	{
		public List<Action> GetTasks() 
		{
			return Tasks;
		}

		public bool GetStopRequested()
		{
			return StopRequested;
		}
	}
}