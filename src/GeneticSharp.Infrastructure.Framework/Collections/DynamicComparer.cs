using System;
using System.Collections.Generic;

namespace GeneticSharp.Infrastructure.Framework.Collections
{
    public class DynamicComparer<TValue> : IComparer<TValue>
    {
        private readonly Func<TValue, TValue, int> mComparer;

        public DynamicComparer(Func<TValue, TValue, int> comparerMethod)
        {
            mComparer = comparerMethod;
        }

        public int Compare(TValue x, TValue y)
        {
            return mComparer(x, y);
        }
    }
}