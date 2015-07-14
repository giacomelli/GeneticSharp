using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Infrastructure.Threading.UnitTests
{
    [TestFixture()]
    public class SmartThreadPoolTaskExecutorTest
    {
        [Test()]
        public void Start_ManyTasks_ParallelExecuted()
        {
            var pipeline = "";
            var target = new SmartThreadPoolTaskExecutor();
            target.Add(() =>
            {
                pipeline += "1";
            });
            target.Add(() =>
            {
                Thread.Sleep(10);
                pipeline += "2";
            });
            target.Add(() =>
            {
                pipeline += "3";
            });

            var actual = target.Start();
            Assert.IsTrue(actual);
            Assert.AreEqual("132", pipeline);
        }

        [Test()]
        public void Start_Timeout_False()
        {
            var pipeline = "1";
            var target = new SmartThreadPoolTaskExecutor();
            target.Timeout = TimeSpan.FromMilliseconds(2);

            target.Add(() =>
            {
                Thread.Sleep(10);
                pipeline += "2";
            });

            var actual = target.Start();
            Assert.IsFalse(actual);
        }

        [Test()]
        public void Start_AnyTaskWithException_Exception()
        {
            var pipeline = "";
            var target = new SmartThreadPoolTaskExecutor();

            target.Add(() =>
            {
                throw new Exception("1");
            });
            target.Add(() =>
            {
                Thread.Sleep(5);
                pipeline += "2";
            });
            target.Add(() =>
            {
                Thread.Sleep(5);
                pipeline += "3";
            });

            ExceptionAssert.IsThrowing(new Exception("1"), () =>
            {
                target.Start();
            });
        }

        [Test()]
        public void Stop_ManyTasks_StopAll()
        {
            var pipeline = "";
            var target = new SmartThreadPoolTaskExecutor();
            target.Timeout = TimeSpan.FromMilliseconds(1000);

            target.Add(() =>
            {
                Thread.Sleep(5);
                pipeline += "1";
            });
            target.Add(() =>
            {
                Thread.Sleep(5);
                pipeline += "2";
            });
            target.Add(() =>
            {
                Thread.Sleep(5);
                pipeline += "3";
            });

            Parallel.Invoke(
                () => Assert.IsTrue(target.Start()),
                () =>
                {
                    Thread.Sleep(100);
                    target.Stop();
                });
        }

        [Test()]
        public void Stop_Tasks_ShutdownCalled()
        {
            var pipeline = "";
            var target = new SmartThreadPoolTaskExecutor();
            target.Timeout = TimeSpan.FromMilliseconds(1000);

            target.Add(() =>
            {
                Thread.Sleep(500);
                pipeline += "1";
            });
            target.Add(() =>
            {
                Thread.Sleep(500);
                pipeline += "2";
            });
            target.Add(() =>
            {
                Thread.Sleep(500);
                pipeline += "3";
            });

            Parallel.Invoke(
                () => target.Start(),
                () =>
                {
                    Thread.Sleep(100);
                    target.Stop();
                });

            Assert.IsFalse(target.IsRunning);
        }
    }
}

