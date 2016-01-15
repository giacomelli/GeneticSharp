using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Randomizations;
using NTime.Framework;

namespace GeneticSharp.Domain.PerformanceTests.Randomizations
{
    [TimerFixture]
    public class BasicRandomizationTest
    {
        [TimerDurationTest(50, Unit = TimePeriod.Microsecond)]
        public void GetUniqueInts()
        {
            var target = new BasicRandomization();
            target.GetUniqueInts(50, 0, 100);
        }
    }
}
