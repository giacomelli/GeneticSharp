using System;
using System.Collections.Generic;

namespace GeneticSharp
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
    }
}