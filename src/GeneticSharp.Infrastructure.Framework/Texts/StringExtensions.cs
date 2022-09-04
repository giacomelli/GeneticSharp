using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GeneticSharp
{
    /// <summary>
    /// String extensions.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Regex s_removePunctuationsRegex = new Regex(@"[!\(\)\[\]{}\:;\.,?'""]*", RegexOptions.Compiled);

        /// <summary>
        /// Replaces the format items in the string with the string representation of
        /// corresponding objects in the specified array.
        /// </summary>
        /// <remarks>
        /// An invariant culture is used to format.
        /// </remarks>
        /// <returns>The formatted string..</returns>
        /// <param name="value">The string.</param>
        /// <param name="args">The arguments..</param>
        public static string With(this string value, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, value, args);
        }

        /// <summary>
        /// Removes the punctuations.
        /// </summary>
        /// <returns>The clean string.</returns>
        /// <param name="source">The source string.</param>
        public static string RemovePunctuations(this string source)
        {
            return s_removePunctuationsRegex.Replace(source, String.Empty);
        }
    }
}