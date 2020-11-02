using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using GeneticSharp.Infrastructure.Framework.Reflection;
using GeneticSharp.Domain.Randomizations;


namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Mutation service.
    /// </summary>
    public static class MutationService
    {
        #region Methods
        /// <summary>
        /// Gets available mutation types.
        /// </summary>
        /// <returns>All available mutation types.</returns>
        public static IList<Type> GetMutationTypes()
        {
            return TypeHelper.GetTypesByInterface<IMutation>();
        }

        /// <summary>
        /// Gets the available mutation names.
        /// </summary>
        /// <returns>The mutation names.</returns>
        public static IList<string> GetMutationNames()
        {
            return TypeHelper.GetDisplayNamesByInterface<IMutation>();
        }

        /// <summary>
        /// Creates the IMutation's implementation with the specified name.
        /// </summary>
        /// <returns>The IMutation's implementation instance.</returns>
        /// <param name="name">The IMutation name.</param>
        /// <param name="constructorArgs">Constructor arguments.</param>
        public static IMutation CreateMutationByName(string name, params object[] constructorArgs)
        {
            return TypeHelper.CreateInstanceByName<IMutation>(name, constructorArgs);
        }

        /// <summary>
        /// Gets the mutation type by the name.
        /// </summary>
        /// <returns>The mutation type.</returns>
        /// <param name="name">The name of mutation.</param>
        public static Type GetMutationTypeByName(string name)
        {
            return TypeHelper.GetTypeByName<IMutation>(name);
        }

        /// <summary>
        /// Shuffle sequence.
        /// </summary>
        /// <returns>The shuffled sequence.</returns>
        /// <param name="source">Source of sequence.</param>
        /// <param name="randomization">Random number generator to select next index to shuffle.</param>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, IRandomization randomization)
        {
            var elements = source.ToArray();
            int swapIndex;

            for (int i = elements.Length - 1; i >= 0; i--)
            {
                swapIndex = randomization.GetInt(0, i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        /// <summary>
        /// Shift sequence to left.
        /// </summary>
        /// <returns>The sequence shifted to left.</returns>
        /// <param name="source">source of sequence</param>
        /// <param name="valueToShift">count of units to be shifted</param>
        public static IEnumerable<T> LeftShift<T>(this IEnumerable<T> source, int valueToShift)
        {
            // all elements except for the first one... and at the end, the first one.
            return source
                        .Skip(valueToShift)
                        .Concat(source.Take(valueToShift));
        }

        /// <summary>
        /// Shift sequence to right.
        /// </summary>
        /// <returns>The sequence shifted to right.</returns>
        /// <param name="source">source of sequence</param>
        /// <param name="valueToShift">count of units to be shifted</param>
        public static IEnumerable<T> RightShift<T>(this IEnumerable<T> source, int valueToShift)
        {
            // the last element (because we're skipping all but one)... then all but the last one.
            var skipCount = source.Count() - valueToShift;

            return source
                .Skip(skipCount)
                .Concat(source.Take(skipCount));
        }
        #endregion
    }
}