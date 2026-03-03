using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Threading
{
    public class StubTaskExecutor : TaskExecutorBase
    {
        public IList<Func<CancellationToken, ValueTask>> GetTasks()
        {
            return Tasks;
        }

        public bool GetStopRequested()
        {
            return StopRequested;
        }
    }
}