using System;
using System.Threading;
using System.Threading.Tasks;
using GeneticSharp.Infrastructure.Framework.Threading;
using NUnit.Framework;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Threading
{
    [TestFixture()]
    [Category("Infrastructure")]
    public class LinearTaskExecutorTest
    {
        [Test()]
        public void Start_Task_TaskRan()
        {
            var pipeline = "";
            var target = new LinearTaskExecutor();
            target.Add(() => pipeline += "1");
            target.Add(() => pipeline += "2");
            target.Add(() => pipeline += "3");

            Assert.IsTrue(target.Start());
            Assert.AreEqual("123", pipeline);
        }

        [Test()]
        public void Start_TakeMoreThanTimeout_False()
        {
            var pipeline = "";
            var target = new LinearTaskExecutor();
            target.Add(() => pipeline += "1");
            target.Add(() =>
            {
                pipeline += "2";
                Thread.Sleep(100);
            });
            target.Add(() => pipeline += "3");

            target.Timeout = TimeSpan.FromMilliseconds(50);
            Assert.IsFalse(target.Start());
            Assert.AreEqual("12", pipeline);
        }

        [Test()]
        public void Stop_ManyTasks_True()
        {
            var pipeline = "";
            var target = new LinearTaskExecutor();
            target.Add(() => pipeline += "1");
            target.Add(() =>
            {
                pipeline += "2";
                Thread.Sleep(1000);
            });
            target.Add(() => pipeline += "3");

            Parallel.Invoke(
                () => Assert.IsTrue(target.Start()),
                () =>
                {
                    Thread.Sleep(5);
                    target.Stop();
                });
        }
    }
}

