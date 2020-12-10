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
        public static double ReverseRosenbrock(double[] coordinates)
        {
            var sum1 = 0.0d;

            for (var i = 0; i < coordinates.Length - 1; i++)
            {
                sum1 += 100 * Math.Pow(coordinates[i + 1] - Math.Pow(coordinates[i], 2), 2)
                    + Math.Pow(1 - coordinates[i], 2);
            }

            return -sum1;
        }




        public static double Hyperellipsoid(double[] x)
        /*
        -n = 30
        -Domain: |x| <= 1.0
        */
        {
            int n = x.Length;
            int i;
            double s = 0.0;
            for (i = 0; i < n; i++)
            {
                s += i * i + x[i] * x[i];
            }
            return s;
        }

        /// <summary>
        /// <see href="https://www.researchgate.net/publication/337947149_Hybridization_of_interval_methods_and_evolutionary_algorithms_for_solving_difficult_optimization_problems"/>
        /// </summary>
        public static double ReverseEggholder(double[] x)
        /*
        - Dimension: n
        - Domain: | x_i | < 512
        -
        */
        {
            int n = x.Length;
            int i;
            var sum = 0.0;
            for (i = 0; i < n - 1; i++)
            {
                sum += -(x[i + 1] + 47.0) * Math.Sin(Math.Sqrt(Math.Abs(x[i + 1] + x[i] * 0.5 + 47.0))) + Math.Sin(Math.Sqrt(Math.Abs(x[i] - (x[i + 1] + 47.0)))) * (-x[i]);
            }
            return -sum;
        }



        public static double ReverseLevy(double[] x)
        /*
        - Global minimum
        - for n=4, fmin = -21.502356 at (1,1,1,-9.752356 )
        - for n=5,6,7, fmin = -11.504403 at (1,\dots,1,-4.754402 )
        */
        {
            int n = x.Length;
            int i;
            double sum = 0.0;
            for (i = 0; i <= n - 2; i++)
            {
                sum += Math.Pow(x[i] - 1, 2.0) * (1 + Math.Pow(Math.Sin(3 * Math.PI * x[i + 1]), 2.0));
            }
            return -Math.Pow(Math.Sin(3 * Math.PI * x[0]), 2.0) + sum + (x[n - 1] - 1) * (1 + Math.Pow(Math.Sin(2 * Math.PI * x[n - 1]), 2.0));
        }

        public static double ReverseMaxmod(double[] x)
        /*Domain: |x[i] <= 10
        Global minimum: 0 at x[i] = 0
        */
        {
            int n = x.Length;
            int i;
            double t = x[0];
            double u = 0;
            for (i = 1; i < n; i++)
            {
                u = Math.Abs(x[i]);
                if (u < t)
                {
                    t = u;
                }
            }
            return -u;
        }



        /// <summary>
        /// The Katsuura function is a fractal with an appearance similar to a lightning bolt and is defined through an iterative process. <see href="https://andrescaicedo.files.wordpress.com/2012/01/katsuura.pdf"/>
        /// </summary>
        /// <param name="n">the dimension</param>
        /// <param name="x">the input double vector</param>
        /// <returns></returns>
        public static double ReverseKatsuuras(double[] x)
        /*
        = Dimension: n (10)
        - Domain: | x[i] | <= 1000
        - Global minimum 1.0 at 0 vector.
        */
        {
            int n = x.Length;
            int i;
            int k;
            int d = 32;
            double prod;
            double s;
            double pow2;
            prod = 1.0;
            for (i = 0; i < n; i++)
            {
                s = 0.0;
                for (k = 1; k <= d; k++)
                {
                    pow2 = Math.Pow(2, k);
                    s += Math.Round(pow2 * x[i]) / pow2;
                }
                prod *= 1.0 + (i + 1) * s;
            }
            return -prod;
        }



    }
}