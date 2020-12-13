using System;

namespace GeneticSharp.Infrastructure.Framework.Commons
{
    public static class MathExtensions
    {
        public static int PositiveMod(this int k, int n) { return (k %= n) < 0 ? k + n : k; }

        public static int PositiveMod<TValue>(this TValue k, int n) => PositiveMod(Convert.ToInt32(k), n);
    }
}