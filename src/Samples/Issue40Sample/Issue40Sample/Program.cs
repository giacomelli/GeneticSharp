using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Issue40Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var progressTimer = new System.Timers.Timer(1000);
            progressTimer.AutoReset = true;
            progressTimer.Elapsed += (sender, arg) =>
            {
                //do something
                Console.WriteLine("Hello from progress timer");

            };

            //start timer
            progressTimer.Start();

            Task.Run(() =>
            {
                RunGeneticOptimizer();

            }).Wait();

            Console.WriteLine("All tasks inside actionblock completed");

            Console.WriteLine($"Press Key to quit");
            Console.ReadLine();

        }

        private static void GenerationRanCallback(string obj)
        {
            Console.WriteLine(obj);
        }

        private static void RunGeneticOptimizer()
        {
            //optimizer variables
            var variables = new List<GeneticOptimizerVariable>()
        {
            new GeneticOptimizerVariable("x", 4, -10, 10)
        };

            //optimizer configuration
            var configuration = new GeneticOptimizerConfiguration("y", variables, 1);

            //objective function
            var objectiveFunction = new Func<double[], double>(inputs =>
            {
                Console.WriteLine("objectiveFunction");
                Thread.Sleep(1000);

                var objectiveFunctionResult = Math.Pow(inputs[0], 3) / Math.Exp(Math.Pow(inputs[0], 0.8));
                return objectiveFunctionResult;
            });

            //optimizer
            var optimizer = new GeneticOptimizer(configuration, objectiveFunction, GenerationRanCallback);

            var watch = new Stopwatch();
            watch.Start();

            optimizer.Start();

            watch.Stop();

            Console.WriteLine($"Number milliseconds: {watch.ElapsedMilliseconds}");


            Console.WriteLine($"Press Key to quit");
            Console.ReadLine();
        }
    }
}
