using System;
using System.Collections.Generic;

namespace GeneticSharp.Extensions.Mathematic.Functions
{
    /// <summary>
    /// See <see href="https://en.wikipedia.org/wiki/Test_functions_for_optimization">Wikipedia Article</see> for further reference
    /// </summary>
    public static class KnownFunctions
    {
        
        private static readonly Dictionary<string, IKnownFunction> _knownFunctions;

        static KnownFunctions()
        {
            _knownFunctions = new Dictionary<string, IKnownFunction>();

            var range = 5.0;
            _knownFunctions.Add(nameof(Ackley), new KnownFunction(range)
            {
                Name = nameof(Ackley),
                Description = "",
                Function = Ackley,
                Fitness = d => Math.Pow(0.99, d)// 1 / (1 + Math.Log(Math.Pow(d,0.1)+1))
            });
            range = 1.0;
            _knownFunctions.Add(nameof(Hyperellipsoid), new KnownFunction(range)
            {
                Name = nameof(Hyperellipsoid),
                Description = "",
                Function = Hyperellipsoid,
                Fitness = d => d
            });
            range = 512.0;
            _knownFunctions.Add(nameof(Eggholder), new KnownFunction(range)
            {
                Name = nameof(Eggholder),
                Description = "",
                Function = Eggholder,
                Fitness = d => -d
            });
            range = 1000;
            _knownFunctions.Add(nameof(Katsuuras), new KnownFunction(range)
            {
                Name = nameof(Katsuuras),
                Description = "",
                Function = Katsuuras,
                Fitness = d => -d
            });
            range = 10;
            _knownFunctions.Add(nameof(Levy), new KnownFunction(range)
            {
                Name = nameof(Levy),
                Description = "",
                Function = Levy,
                Fitness = d => Math.Pow(0.99, d)
            });
            range = 10;
            _knownFunctions.Add(nameof(Maxmod), new KnownFunction(range)
            {
                Name = nameof(Maxmod),
                Description = "",
                Function = Maxmod,
                Fitness = d => -d
            });
            range = Math.PI;
            _knownFunctions.Add(nameof(Michalewitz), new KnownFunction(range)
            {
                Name = nameof(Michalewitz),
                Description = "",
                Function = Michalewitz,
                Fitness = d => -d
            });
            range = 10;
            _knownFunctions.Add(nameof(Rastrigin), new KnownFunction(range)
            {
                Name = nameof(Rastrigin),
                Description = "",
                Function = Rastrigin,
                Fitness = d => -d//Math.Pow(0.99, d)
            });
            range = 2;
            _knownFunctions.Add(nameof(Rosenbrock), new KnownFunction(range)
            {
                Name = nameof(Rosenbrock),
                Description = "",
                Function = Rosenbrock,
                Reverse = true,
                Fitness = d =>  Math.Pow(0.99999, d) //-d // 1 / (1 + d)
            });
        }


        public static IDictionary<string, IKnownFunction> GetKnownFunctions()
        {
            return _knownFunctions;
        }



        public static double Ackley(double[] coordinates)
        {
            var sum1 = 0.0d;
            var sum2 = 0.0d;

            for (var i = 0; i < coordinates.Length; i++)
            {
                sum1 += Math.Pow(coordinates[i], 2);
                sum2 += Math.Cos(2 * Math.PI * coordinates[i]);
            }

            return (-20.0 * Math.Exp(-0.2 * Math.Sqrt(sum1 / coordinates.Length)) + 20 - Math.Exp(sum2 / coordinates.Length)
                     + Math.Exp(1.0));
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
        private static double Eggholder(double[] x)
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
                sum += -(x[i + 1] + 47.0) * Math.Sin(Math.Sqrt(Math.Abs(x[i + 1] + x[i] * 0.5 + 47.0))) + Math.Sin(Math.Sqrt(Math.Abs(x[i] - (x[i + 1] + 47.0)))) * -x[i];
            }
            return sum;
        }





        /// <summary>
        /// The Katsuura function is a fractal with an appearance similar to a lightning bolt and is defined through an iterative process. <see href="https://andrescaicedo.files.wordpress.com/2012/01/katsuura.pdf"/>
        /// </summary>
        /// <param name="n">the dimension</param>
        /// <param name="x">the input double vector</param>
        /// <returns></returns>
        public static double Katsuuras(double[] x)
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
            return prod;
        }


        public static double Levy(double[] x)
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
            return Math.Pow(Math.Sin(3 * Math.PI * x[0]), 2.0) + sum + (x[n - 1] - 1) * (1 + Math.Pow(Math.Sin(2 * Math.PI * x[n - 1]), 2.0));
        }

        public static double Maxmod(double[] x)
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
                return u;
            }

        public static double Michalewitz(double[] x)
        {
            /*Domain: |x[i] <= PI
           Global minimum: 0 at x[i] = 0
           */
            int n = x.Length;
            double u;
            int i;
            u = 0;
            for (i = 0; i < n; i++)
            {
                u += Math.Sin(x[i]) * Math.Pow(Math.Sin(i * x[i] * x[i] / Math.PI), 2.0 * 10.0);
            }
            return u;
        }

        public static double Rastrigin(double[] coordinates)
        {
            double result = 0.0;

            foreach (double x in coordinates)
            {
                result += x * x - 10.0 * Math.Cos(2.0 * Math.PI * x);
            }

            return result;
        }



        /// <summary>
        /// From <see href="https://en.wikipedia.org/wiki/Rosenbrock_function#Multidimensional_generalisations">Wikipedia Article</see>
        /// </summary>
        public static double Rosenbrock(double[] coordinates)
        {
            var sum1 = 0.0d;

            for (var i = 0; i < coordinates.Length - 1; i++)
            {
                sum1 += 100 * Math.Pow(coordinates[i + 1] - Math.Pow(coordinates[i], 2), 2)
                    + Math.Pow(1 - coordinates[i], 2);
            }

            return sum1;
        }




       

       


      



       


    }
}