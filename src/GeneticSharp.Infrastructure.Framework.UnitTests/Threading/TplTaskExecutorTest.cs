using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Threading
{
    [TestFixture()]
    [Category("Infrastructure")]
    public class TplTaskExecutorTest
    {
        [Test]
        public void Start_ManyTasks_ParallelExecuted()
        {
            var pipeline = "";
            var target = new TplTaskExecutor();
            target.Add(() =>
            {
                pipeline += "1";
            });
            target.Add(() =>
            {
                Thread.Sleep(100);
                pipeline += "2";
            });
            target.Add(() =>
            {
                Thread.Sleep(10);
                pipeline += "3";
            });

            var actual = target.Start();
            Assert.IsTrue(actual);
            Assert.AreEqual("132", pipeline);
        }

        [Test]
        public void Start_ManyTasksWithGreaterNumberOfThreads_ParallelExecuted()
        {
            var pipeline = "";
            var target = new TplTaskExecutor();
            target.Add(() =>
            {
                pipeline += "1";
            });
            target.Add(() =>
            {
                Thread.Sleep(100);
                pipeline += "2";
            });
            target.Add(() =>
            {
                Thread.Sleep(10);
                pipeline += "3";
            });

            var actual = target.Start();
            Assert.IsTrue(actual);
            Assert.AreEqual("132", pipeline);
        }


        [Test]
        [TestCase(1, 5000, false)]
        [TestCase(1000, 1, true)]
        public void Start_Timeout_Completed(int timeout, int timeExecutingTasks, bool expectedCompleted)
        {
            var pipeline = "1";
            var target = new TplTaskExecutor();
            target.Timeout = TimeSpan.FromMilliseconds(timeout);

            target.Add(() =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(timeExecutingTasks));
                pipeline += "2";
            });

            var actual = target.Start();
            Assert.AreEqual(expectedCompleted, actual);
            Assert.AreEqual("12", pipeline);
        }

        [Test]
        public void Start_AnyTaskWithException_Exception()
        {
            var pipeline = "";
            var target = new TplTaskExecutor();

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

            Assert.Catch<Exception>(() =>
            {
                target.Start();
            }, "1");
        }

        [Test]
        public void Stop_ManyTasks_StopAll()
        {
            var pipeline = "";
            var target = new TplTaskExecutor();
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
                    //The tasks should be completed when stop is invoked.
                    Thread.Sleep(100);
                    target.Stop();
                });
        }

        [Test]
        public void Stop_Tasks_ShutdownCalled()
        {
            var pipeline = "";
            var target = new TplTaskExecutor
            {
                Timeout = TimeSpan.FromMilliseconds(1000)
            };

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

        [Test]
        public void Start_MaxThreads1_DoNotBlockOtherThreads()
        {
            var target = new TplTaskExecutor
            {
                MinThreads = 1,
                MaxThreads = 1
            };

            target.Add(() =>
            {
            });
            target.Add(() =>
            {
                Thread.Sleep(200);
            });
            target.Add(() =>
            {
                Thread.Sleep(10);
            });


            int otherThreadCount = 0;
            var otherThread = new System.Timers.Timer(50)
            {
                AutoReset = true
            };
            otherThread.Elapsed += (sender, arg) =>
            {
                otherThreadCount++;
            };
            otherThread.Start();

            Task.Run(() =>
            {
                target.Start();
            }).Wait();

            otherThread.Stop();
            Assert.GreaterOrEqual(otherThreadCount, 2);
        }

        [Test]
        public void Stop_Before_Start()
        {
            var pipeline = "";
            var target = new TplTaskExecutor();
            target.Add(() =>
            {
                pipeline += "1";
            });
            target.Add(() =>
            {
                Thread.Sleep(100);
                pipeline += "2";
            });
            target.Add(() =>
            {
                Thread.Sleep(10);
                pipeline += "3";
            });

            target.Stop();
            var actual = target.Start();
            Assert.IsTrue(actual);
            Assert.AreEqual("132", pipeline);
        }

        [Test]        
        public void Start_Stop_Incomplete()
        {
            long pipeline = 0;
            var target = new TplTaskExecutor();
            target.Timeout = TimeSpan.FromSeconds(2);

            for (int i = 0; i < 1000; i++)
            {
                target.Add(() =>
                {
                    Interlocked.Increment(ref pipeline);
                    target.Stop();
                    Thread.Sleep(200);                                       
                }); 
            }
       
            var actual = target.Start();
            Assert.IsFalse(actual);
            Assert.Less(pipeline, 32);
        }
    }
}
