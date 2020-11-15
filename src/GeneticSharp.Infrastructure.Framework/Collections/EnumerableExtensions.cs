﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace GeneticSharp.Infrastructure.Framework.Collections
{
    /// <summary>
    /// Enumerable extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Iterates in the collection calling the action for each item.
        /// </summary>
        /// <param name="self">The enumerable it self.</param>
        /// <param name="action">The each action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Each<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
            {
                action(item);
            }
        }

        public static TSource MaxBy<TSource, TKey>(this IList<TSource> items, Func<TSource, TKey> keySelector)
        {
            return MaxBy(items, keySelector, Comparer<TKey>.Default);
        }


        /// <summary>
        /// Returns the item from a collection given an generics indexer in linear time (sorting first is log-linear)
        /// The gain can be very significant (*10) depending on the version of .Net framework, as latest .Net core seems to implement the "lazy" version below, which would yield a similar behavior
        /// </summary>
        /// <param name="items">the items, from which to extract the max</param>
        /// <param name="keySelector">The selector for values to compare</param>
        /// <param name="comparer">the comparer used ascending</param>
        /// <returns></returns>
        public static TSource MaxBy<TSource, TKey>(this IList<TSource> items, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (items.Count==0)
            {
                return default;
            }
            TSource current = items[0];
            TKey maxValue = keySelector(items[0]);
            for (var index = 1; index < items.Count; index++)
            {
                var currentKey = keySelector(items[index]);
                if (comparer.Compare(currentKey, maxValue) > 0)
                {
                    maxValue = currentKey;
                    current = items[index];
                }
            }

            return current;

        }



        public static IEnumerable<TSource> LazyOrderBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return LazyOrderBy(source, keySelector, Comparer<TKey>.Default);
        }


        /// <summary>
        /// Copied from article https://faithlife.codes/blog/2010/04/a_truly_lazy_orderby_in_linq/
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// May improve speed significantly depending on Framework version. It seems latest .Net standard uses the same technique.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/> whose elements are sorted according to a key.</returns>
        /// <remarks>This method only sorts as much of <paramref name="source"/> as is
        /// necessary to yield the elements that are requested from the return value. It performs an
        /// unstable sort, and can't be chained to Enumerable.ThenBy.</remarks>
        public static IEnumerable<TSource> LazyOrderBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            // convert the entire input to an array, so we can sort it in place
            TSource[] array = source.ToArray();

            // track index of the last (sorted) item that was output
            int index = 0;

            // use a stack to turn a recursive algorithm into an iterative one
            Stack<Range> stack = new Stack<Range>();

            // start by processing the entire array
            stack.Push(new Range(0, array.Length - 1));
            while (stack.Count > 0)
            {
                // get current range to sort
                Range currentRange = stack.Pop();

                if (currentRange.Last - currentRange.First == 0)
                {
                    // base case: we only have one item; it must be sorted
                    yield return array[index];
                    index++;
                }
                else
                {
                    // recursive case: partition the array into two halves and "recurse" on each half
                    int pivotIndex = Partition(array, currentRange.First, currentRange.Last, keySelector, comparer);

                    // pushing the second half of the range first means that it will be sorted last
                    stack.Push(new Range(pivotIndex + 1, currentRange.Last));
                    stack.Push(new Range(currentRange.First, pivotIndex));
                }
            }
        }

        // Partitions an array into two halves around a pivot, returning the index of the pivot element.
        // This algorithm is taken from Introduction to Algorithms (MIT Press), p154.
        private static int Partition<TSource, TKey>(TSource[] source, int first, int last, Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            // naively select the first item as the pivot
            TKey pivot = keySelector(source[first]);

            // walk the array, moving items into the correct position
            int left = first - 1;
            int right = last + 1;
            while (true)
            {
                do
                {
                    right--;
                } while (comparer.Compare(keySelector(source[right]), pivot) > 0);
                do
                {
                    left++;
                } while (comparer.Compare(keySelector(source[left]), pivot) < 0);

                if (left < right)
                {
                    TSource temp = source[left];
                    source[left] = source[right];
                    source[right] = temp;
                }
                else
                {
                    return right;
                }
            }
        }

        // Range represents an inclusive range of indexes into the array being sorted.
        private struct Range
        {
            public Range(int first, int last)
            {
                m_first = first;
                m_last = last;
            }

            public int First { get { return m_first; } }
            public int Last { get { return m_last; } }

            readonly int m_first;
            readonly int m_last;
        }



    }
}