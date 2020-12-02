using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace GeneticSharp.Infrastructure.Framework.Texts
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


        /// <summary>
        /// Computes the hamming distance between 2 strings, that is, the number of distinct chars
        /// </summary>
        public static int HammingDistance(this string s, string t)
        {
            if (s.Length != t.Length)
            {
                throw new Exception("Strings must be equal length");
            }

            int distance =
                s.ToCharArray()
                    .Zip(t.ToCharArray(), (c1, c2) => new { c1, c2 })
                    .Count(m => m.c1 != m.c2);

            return distance;
        }


        /// <summary>
        /// Computes the Levenshtein distance between two strings.
        /// </summary>
        public static int LevenshteinDistance(this string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        /// <summary>
        /// Computes the Damerau Levenshtein distance between two strings.
        /// </summary>
        public static int DamerauLevenshteinDistance(this string s, string t)
        {
            var bounds = new { Height = s.Length + 1, Width = t.Length + 1 };

            int[,] matrix = new int[bounds.Height, bounds.Width];

            for (int height = 0; height < bounds.Height; height++) { matrix[height, 0] = height; };
            for (int width = 0; width < bounds.Width; width++) { matrix[0, width] = width; };

            for (int height = 1; height < bounds.Height; height++)
            {
                for (int width = 1; width < bounds.Width; width++)
                {
                    int cost = (s[height - 1] == t[width - 1]) ? 0 : 1;
                    int insertion = matrix[height, width - 1] + 1;
                    int deletion = matrix[height - 1, width] + 1;
                    int substitution = matrix[height - 1, width - 1] + cost;

                    int distance = Math.Min(insertion, Math.Min(deletion, substitution));

                    if (height > 1 && width > 1 && s[height - 1] == t[width - 2] && s[height - 2] == t[width - 1])
                    {
                        distance = Math.Min(distance, matrix[height - 2, width - 2] + cost);
                    }

                    matrix[height, width] = distance;
                }
            }

            return matrix[bounds.Height - 1, bounds.Width - 1];
        }


    }
}