using System;

namespace GeneticSharp.Infrastructure.Framework.Images
{
    public static class MathExtensions
    {
        public static int PositiveMod(this int k, int n) { return (k %= n) < 0 ? k + n : k; }

        public static int PositiveMod<TValue>(this TValue k, int n) => PositiveMod(Convert.ToInt32(k), n);

        /// <summary>
        /// https://en.wikipedia.org/wiki/Exponentiation_by_squaring
        /// </summary>
        /// <param name="num">the number to exponentiate</param>
        /// <param name="exp">the integer exponent</param>
        /// <returns></returns>
        public static double IntPow(this double num, int exp)
        {
            double result = 1.0;
            while (exp > 0)
            {
                if (exp % 2 == 1)
                    result *= num;
                exp >>= 1;
                num *= num;
            }

            return result;
        }

        public static double IntPow<TValue>(this TValue num, int exp) => Convert.ToDouble(num).IntPow(exp);


    }
}