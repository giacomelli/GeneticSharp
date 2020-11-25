using System;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// See <see href="https://en.wikipedia.org/wiki/Test_functions_for_optimization">Wikipedia Article</see> for further reference
    /// </summary>
    public static class KnownFunctionsFactory
    {

        public static double Rastrigin(double[] coordinates)
        {
            double result = 0.0;

            foreach (double x in coordinates)
            {
                result += x * x - 10.0 * Math.Cos(2.0 * Math.PI * x);
            }

            return result;
        }

        public static double ReverseAckley(double[] coordinates)
        {
            var sum1 = 0.0d;
            var sum2 = 0.0d;

            for (var i = 0; i < coordinates.Length; i++)
            {
                sum1 += Math.Pow(coordinates[i], 2);
                sum2 += (Math.Cos(2 * Math.PI * coordinates[i]));
            }

            return -(-20.0 * Math.Exp(-0.2 * Math.Sqrt(sum1 / coordinates.Length)) + 20 - Math.Exp(sum2 / coordinates.Length)
                     + Math.Exp(1.0));
        }

        /// <summary>
        /// From <see href="https://en.wikipedia.org/wiki/Rosenbrock_function#Multidimensional_generalisations">Wikipedia Article</see>
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static double ReverseRosenbrock(double[] coordinates)
        {
            var sum1 = 0.0d;

            for (var i = 0; i < coordinates.Length-1; i++)
            {
                sum1 += 100 * Math.Pow(coordinates[i+1] - Math.Pow(coordinates[i], 2), 2)
                    + Math.Pow(1-coordinates[i], 2);
            }

            return -sum1;
        }



    }
}