using System;

namespace GeneticSharp.Extensions.Mathematic
{
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



    }
}