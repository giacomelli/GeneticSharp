using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Commons;

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
                Fitness = (genes, d) => Math.Pow(0.9, d) 
            });
            range = 1.0;
            _knownFunctions.Add(nameof(Hyperellipsoid), new KnownFunction(range)
            {
                Name = nameof(Hyperellipsoid),
                Description = "",
                Function = Hyperellipsoid,
                Fitness = (genes, d) => d
            });
            range = 512.0;
            _knownFunctions.Add(nameof(Eggholder), new KnownFunction(range)
            {
                Name = nameof(Eggholder),
                Description = "",
                Function = Eggholder,
                Fitness = (genes, d) => -d
            });
            range = 1000;
            _knownFunctions.Add(nameof(Katsuuras), new KnownFunction(range)
            {
                Name = nameof(Katsuuras),
                Description = "",
                Function = Katsuuras,
                Fitness = (genes, d) => -d
            });
            range = 10;
            _knownFunctions.Add(nameof(Levy), new KnownFunction(range)
            {
                Name = nameof(Levy),
                Description = "",
                Function = Levy,
                //Fitness = (genes, d) => -d
                Fitness = (genes, d) => Math.Pow(0.9, d)
            });
            range = 10;
            _knownFunctions.Add(nameof(Maxmod), new KnownFunction(range)
            {
                Name = nameof(Maxmod),
                Description = "",
                Function = Maxmod,
                Fitness = (genes, d) => -d
            });
            range = Math.PI;
            _knownFunctions.Add(nameof(Michalewitz), new KnownFunction(range)
            {
                Name = nameof(Michalewitz),
                Description = "",
                Function = Michalewitz,
                Fitness = (genes, d) => -d
            });
            range = 50;
            _knownFunctions.Add(nameof(Neumaier), new KnownFunction(range)
            {
                Name = nameof(Neumaier),
                Description = "",
                Function = Neumaier,
                Fitness = (genes, d) => -d
            });
            range = 10;
            _knownFunctions.Add(nameof(Rastrigin), new KnownFunction(range)
            {
                Name = nameof(Rastrigin),
                Description = "",
                Function = Rastrigin,
                Fitness = (genes, d) => Math.Pow(0.999, d) 
            });
            range = 2;
            _knownFunctions.Add(nameof(Rosenbrock), new KnownFunction(range)
            {
                Name = nameof(Rosenbrock),
                Description = "",
                Function = Rosenbrock,
                Fitness = (genes, d) => Math.Pow(0.99,Math.Sqrt(d)) 
            });
        }

        //private static Dictionary<int, double> _customFunctionMaxes = new Dictionary<int, double>();
        //private static double CustomFunctionMax(int size)
        //{
        //    if (!_customFunctionMaxes.TryGetValue(size, out var toReturn))
        //    {
        //        toReturn = _knownFunctions[nameof(KnownFunctions.CustomFunction)]
        //            .Function(Enumerable.Repeat(0.0, size).ToArray());
        //        _customFunctionMaxes[size] = toReturn;
        //    }

        //    return toReturn;
        //}

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
                sum1 += coordinates[i] * coordinates[i];
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
                    pow2 = 2.IntPow(k);
                    s += Math.Round(pow2 * x[i]) / pow2;
                }
                prod *= 1.0 + (i + 1) * s;
            }
            return prod;
        }

        /// <summary>
        /// From <see cref="https://www.sfu.ca/~ssurjano/levy.html"/> 
        /// </summary>
        public static double Levy(double[] x)
        {
            //Minimum 0 at (1,1,1,1,1,1...)
            int n = x.Length;
            int i;
            
            var w = x.Select(xi => (1 + (xi - 1) / 4)).ToArray();
            var s0 = Math.Sin(Math.PI * w[0]);
            double sum =  s0*s0;
            for (i = 0; i <= n - 2; i++)
            {
                var s1 = (w[i] - 1);
                var s2 = Math.Sin(Math.PI * w[i] + 1);
                sum += s1 * s1 * (1 + 10 * s2 * s2);
            }

            var sn1 = w[n - 1] - 1;
            var sn2 = Math.Sin(2 * Math.PI * w[n - 1]);
            sum += sn1 * sn1 * (1 + sn2 * sn2);
            return sum;
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
                u += Math.Sin(x[i]) * (Math.Sin(i * x[i] * x[i] / Math.PI).IntPow(20));
            }
            return u;
        }

        public static double Neumaier( double[] x)
            /*
            Suitable bounds for a bound constrained version would be [-n^2,n^2] for each component.
            The solution is x_i=i(n+1-i) with f(x)=-n(n+4)(n-1)/6.
            */
        {
            int n = x.Length;
            int i;
            double s1 = 0.0;
            double s2 = 0.0;
            for (i = 0; i < n; i++)
            {
                s1 += (x[i] - 1) * (x[i] - 1);
                if (i != 0)
                {
                    s2 += x[i] * x[i - 1];
                }
            }
            return s1 - s2;
        }


        public static double Rastrigin(double[] coordinates)
        {
            double result = 10 * coordinates.Length;

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
                var s0 = coordinates[i + 1] - (coordinates[i] * coordinates[i]);
                var s1 = coordinates[i] - 1;
                sum1 += (100 * s0 * s0) + (s1 * s1);
            }

            return sum1;
        }




       

       


      



       


    }
}