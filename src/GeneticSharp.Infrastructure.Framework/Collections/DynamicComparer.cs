using System;
using System.Collections.Generic;

namespace GeneticSharp.Infrastructure.Framework.Collections
{
    public class DynamicComparer<TValue> : IComparer<TValue>
    {
        private Func<TValue, TValue, double> mComparer;

        public DynamicComparer(Func<TValue, TValue, double> comparerMethod)
        {
            mComparer = comparerMethod;
        }

        public int Compare(TValue x, TValue y)
        {
            return Convert.ToInt32(mComparer(x, y));
        }
    }
}