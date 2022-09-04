using System;
using System.Collections.Generic;

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