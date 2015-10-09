using System;
using System.Collections.Generic;
using GeneticSharp.Infrastructure.Framework.Threading;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Threading
{
    public class StubTaskExecutor : TaskExecutorBase
    {
        public IList<Action> GetTasks()
        {
            return Tasks;
        }

        public bool GetStopRequested()
        {
            return StopRequested;
        }
    }
}