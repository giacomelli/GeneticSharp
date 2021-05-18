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
        /// Computes the hamming distance between 2 strings of equal lengths, that is, the number of distinct chars
        /// </summary>
        public static int HammingDistance(this string s, string t)
        {
            if (s.Length != t.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(t), "Strings must be equal length");
            }

            int distance =
                s.ToCharArray()
                    .Zip(t.ToCharArray(), (c1, c2) => new { c1, c2 })
                    .Count(m => m.c1 != m.c2);

            return distance;
        }


        /// <summary>
        /// Computes and returns the Levenshtein edit distance between two strings, i.e. the
        /// number of insertion, deletion, and sustitution edits required to transform one
        /// string to the other. This value will be >= 0, where 0 indicates identical strings.
        /// Comparisons are case sensitive, so for example, "Fred" and "fred" will have a 
        /// distance of 1.
        /// http://blog.softwx.net/2014/12/optimizing-levenshtein-algorithm-in-c.html
        /// </summary>
        /// <remarks>See http://en.wikipedia.org/wiki/Levenshtein_distance
        /// This is based on Sten Hjelmqvist's "Fast, memory efficient" algorithm, described
        /// at http://www.codeproject.com/Articles/13525/Fast-memory-efficient-Levenshtein-algorithm, 
        /// with some additional optimizations.
        /// </remarks>
        /// <param name="s">String being compared for distance.</param>
        /// <param name="t">String being compared against other string.</param>
        /// <returns>int edit distance, >= 0 representing the number of edits required to transform one string to the other.</returns>
        public static int LevenshteinDistance(this string s, string t)
        {
            if (String.IsNullOrEmpty(s)) return (t ?? "").Length;
            if (String.IsNullOrEmpty(t)) return s.Length;

            // if strings of different lengths, ensure shorter string is in s. This can result in a little
            // faster speed by spending more time spinning just the inner loop during the main processing.
            if (s.Length > t.Length)
            {
                var temp = s; s = t; t = temp; // swap s and t
            }
            int sLen = s.Length; // this is also the minimun length of the two strings
            int tLen = t.Length;

            // suffix common to both strings can be ignored
            while (sLen > 0 && s[sLen - 1] == t[tLen - 1]) { sLen--; tLen--; }

            int start = 0;
            if (s[0] == t[0] || sLen == 0)
            { // if there's a shared prefix, or all s matches t's suffix
              // prefix common to both strings can be ignored
                while (start < sLen && s[start] == t[start]) start++;
                sLen -= start; // length of the part excluding common prefix and suffix
                tLen -= start;

                // if all of shorter string matches prefix and/or suffix of longer string, then
                // edit distance is just the delete of additional characters present in longer string
                if (sLen == 0) return tLen;

                t = t.Substring(start, tLen); // faster than t[start+j] in inner loop below
            }
            var v0 = new int[tLen];
            for (int j = 0; j < tLen; j++) v0[j] = j + 1;

            int current = 0;
            for (int i = 0; i < sLen; i++)
            {
                char sChar = s[start + i];
                int left = current = i;
                for (int j = 0; j < tLen; j++)
                {
                    int above = current;
                    current = left; // cost on diagonal (substitution)
                    left = v0[j];
                    if (sChar != t[j])
                    {
                        current++;              // substitution
                        int insDel = above + 1; // deletion
                        if (insDel < current) current = insDel;
                        insDel = left + 1;      // insertion
                        if (insDel < current) current = insDel;
                    }
                    v0[j] = current;
                }
            }
            return current;
        }


        /// <summary>
        ///  Computes and returns the Damerau-Levenshtein edit distance between two strings, 
        /// i.e. the number of insertion, deletion, sustitution, and transposition edits
        /// required to transform one string to the other. This value will be >= 0, where 0
        /// indicates identical strings. Comparisons are case sensitive, so for example, 
        /// "Fred" and "fred" will have a distance of 1. This algorithm is basically the
        /// Levenshtein algorithm with a modification that considers transposition of two
        /// adjacent characters as a single edit.
        /// http://blog.softwx.net/2015/01/optimizing-damerau-levenshtein_15.html
        /// </summary>
        /// <remarks>See http://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
        /// Note that this is based on Sten Hjelmqvist's "Fast, memory efficient" algorithm, described
        /// at http://www.codeproject.com/Articles/13525/Fast-memory-efficient-Levenshtein-algorithm.
        /// This version differs by including some optimizations, and extending it to the Damerau-
        /// Levenshtein algorithm.
        /// Note that this is the simpler and faster optimal string alignment (aka restricted edit) distance
        /// that difers slightly from the classic Damerau-Levenshtein algorithm by imposing the restriction
        /// that no substring is edited more than once. So for example, "CA" to "ABC" has an edit distance
        /// of 2 by a complete application of Damerau-Levenshtein, but a distance of 3 by this method that
        /// uses the optimal string alignment algorithm. See wikipedia article for more detail on this
        /// distinction.
        /// </remarks>
        /// <param name="s">String being compared for distance.</param>
        /// <param name="t">String being compared against other string.</param>
        /// <returns>int edit distance, >= 0 representing the number of edits required
        /// to transform one string to the other.</returns>
        public static int DamerauLevenshteinDistance(this string s, string t)
        {
            if (String.IsNullOrEmpty(s)) return (t ?? "").Length;
            if (String.IsNullOrEmpty(t)) return s.Length;

            // if strings of different lengths, ensure shorter string is in s. This can result in a little
            // faster speed by spending more time spinning just the inner loop during the main processing.
            if (s.Length > t.Length)
            {
                var temp = s; s = t; t = temp; // swap s and t
            }
            int sLen = s.Length; // this is also the minimun length of the two strings
            int tLen = t.Length;

            // suffix common to both strings can be ignored
            while (sLen > 0 && s[sLen - 1] == t[tLen - 1]) { sLen--; tLen--; }

            int start = 0;
            if (s[0] == t[0] || sLen == 0)
            { // if there's a shared prefix, or all s matches t's suffix
              // prefix common to both strings can be ignored
                while (start < sLen && s[start] == t[start]) start++;
                sLen -= start; // length of the part excluding common prefix and suffix
                tLen -= start;

                // if all of shorter string matches prefix and/or suffix of longer string, then
                // edit distance is just the delete of additional characters present in longer string
                if (sLen == 0) return tLen;

                t = t.Substring(start, tLen); // faster than t[start+j] in inner loop below
            }

            var v0 = new int[tLen];
            var v2 = new int[tLen]; // stores one level further back (offset by +1 position)
            for (int j = 0; j < tLen; j++) v0[j] = j + 1;

            char sChar = s[0];
            int current = 0;
            for (int i = 0; i < sLen; i++)
            {
                char prevsChar = sChar;
                sChar = s[start + i];
                char tChar = t[0];
                int left = i;
                current = i + 1;
                int nextTransCost = 0;
                for (int j = 0; j < tLen; j++)
                {
                    int above = current;
                    int thisTransCost = nextTransCost;
                    nextTransCost = v2[j];
                    v2[j] = current = left; // cost of diagonal (substitution)
                    left = v0[j];    // left now equals current cost (which will be diagonal at next iteration)
                    char prevtChar = tChar;
                    tChar = t[j];
                    if (sChar != tChar)
                    {
                        if (left < current) current = left;   // insertion
                        if (above < current) current = above; // deletion
                        current++;
                        if (i != 0 && j != 0
                                   && sChar == prevtChar
                                   && prevsChar == tChar)
                        {
                            thisTransCost++;
                            if (thisTransCost < current) current = thisTransCost; // transposition
                        }
                    }
                    v0[j] = current;
                }
            }
            return current;
        }
        ///<summary>
        /// Computes and returns the Damerau-Levenshtein edit distance between two strings, 
        /// i.e. the number of insertion, deletion, sustitution, and transposition edits
        /// required to transform one string to the other. This value will be >= 0, where 0
        /// indicates identical strings. Comparisons are case sensitive, so for example, 
        /// "Fred" and "fred" will have a distance of 1. This algorithm is basically the
        /// Levenshtein algorithm with a modification that considers transposition of two
        /// adjacent characters as a single edit.
        /// This version takes an additional parameter that lets you specify a maximum distance. Specifying a maximum distance allows important optimizations.
        /// http://blog.softwx.net/2015/01/optimizing-damerau-levenshtein_15.html
        /// </summary>
        /// <remarks>See http://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
        /// Note that this is based on Sten Hjelmqvist's "Fast, memory efficient" algorithm, described
        /// at http://www.codeproject.com/Articles/13525/Fast-memory-efficient-Levenshtein-algorithm.
        /// This version differs by including some optimizations, and extending it to the Damerau-
        /// Levenshtein algorithm.
        /// Note that this is the simpler and faster optimal string alignment (aka restricted edit) distance
        /// that difers slightly from the classic Damerau-Levenshtein algorithm by imposing the restriction
        /// that no substring is edited more than once. So for example, "CA" to "ABC" has an edit distance
        /// of 2 by a complete application of Damerau-Levenshtein, but a distance of 3 by this method that
        /// uses the optimal string alignment algorithm. See wikipedia article for more detail on this
        /// distinction.
        /// </remarks>
        /// <param name="s">String being compared for distance.</param>
        /// <param name="t">String being compared against other string.</param>
        /// <param name="maxDistance">The maximum edit distance of interest.</param>
        /// <returns>int edit distance, >= 0 representing the number of edits required
        /// to transform one string to the other, or -1 if the distance is greater than the specified maxDistance.</returns>
        public static int DamerauLevenshteinDistance(this string s, string t, int maxDistance)
        {
            if (String.IsNullOrEmpty(s)) return (t ?? "").Length <= maxDistance ? (t ?? "").Length : -1;
            if (String.IsNullOrEmpty(t)) return s.Length <= maxDistance ? s.Length : -1;

            // if strings of different lengths, ensure shorter string is in s. This can result in a little
            // faster speed by spending more time spinning just the inner loop during the main processing.
            if (s.Length > t.Length)
            {
                var temp = s; s = t; t = temp; // swap s and t
            }
            int sLen = s.Length; // this is also the minimun length of the two strings
            int tLen = t.Length;

            // suffix common to both strings can be ignored
            while (sLen > 0 && s[sLen - 1] == t[tLen - 1]) { sLen--; tLen--; }

            int start = 0;
            if (s[0] == t[0] || sLen == 0)
            { // if there's a shared prefix, or all s matches t's suffix
              // prefix common to both strings can be ignored
                while (start < sLen && s[start] == t[start]) start++;
                sLen -= start; // length of the part excluding common prefix and suffix
                tLen -= start;

                // if all of shorter string matches prefix and/or suffix of longer string, then
                // edit distance is just the delete of additional characters present in longer string
                if (sLen == 0) return tLen <= maxDistance ? tLen : -1;

                t = t.Substring(start, tLen); // faster than t[start+j] in inner loop below
            }
            int lenDiff = tLen - sLen;
            if (maxDistance < 0 || maxDistance > tLen)
            {
                maxDistance = tLen;
            }
            else if (lenDiff > maxDistance) return -1;

            var v0 = new int[tLen];
            var v2 = new int[tLen]; // stores one level further back (offset by +1 position)
            int j;
            for (j = 0; j < maxDistance; j++) v0[j] = j + 1;
            for (; j < tLen; j++) v0[j] = maxDistance + 1;

            int jStartOffset = maxDistance - (tLen - sLen);
            bool haveMax = maxDistance < tLen;
            int jStart = 0;
            int jEnd = maxDistance;
            char sChar = s[0];
            int current = 0;
            for (int i = 0; i < sLen; i++)
            {
                char prevsChar = sChar;
                sChar = s[start + i];
                char tChar = t[0];
                int left = i;
                current = left + 1;
                int nextTransCost = 0;
                // no need to look beyond window of lower right diagonal - maxDistance cells (lower right diag is i - lenDiff)
                // and the upper left diagonal + maxDistance cells (upper left is i)
                jStart += i > jStartOffset ? 1 : 0;
                jEnd += jEnd < tLen ? 1 : 0;
                for (j = jStart; j < jEnd; j++)
                {
                    int above = current;
                    int thisTransCost = nextTransCost;
                    nextTransCost = v2[j];
                    v2[j] = current = left; // cost of diagonal (substitution)
                    left = v0[j];    // left now equals current cost (which will be diagonal at next iteration)
                    char prevtChar = tChar;
                    tChar = t[j];
                    if (sChar != tChar)
                    {
                        if (left < current) current = left;   // insertion
                        if (above < current) current = above; // deletion
                        current++;
                        if (i != 0 && j != 0
                                   && sChar == prevtChar
                                   && prevsChar == tChar)
                        {
                            thisTransCost++;
                            if (thisTransCost < current) current = thisTransCost; // transposition
                        }
                    }
                    v0[j] = current;
                }
                if (haveMax && v0[i + lenDiff] > maxDistance) return -1;
            }
            return current <= maxDistance ? current : -1;
        }




       

        public static string ToStringLookup(this int value)
        {
            // See if the integer is in range of the lookup table.
            // ... If it is present, return the string literal element.
            if (value >= 0 &&
                value < _top)
            {
                return _cache[value];
            }
            // Fall back to ToString method.
            return value.ToString(CultureInfo.InvariantCulture);
        }

        // Lookup table.
        private static readonly string[] _cache = Enumerable.Range(0, 10000).Select(i => i.ToString(CultureInfo.InvariantCulture))
            .ToArray();


        // Lookup table last index.
        private static readonly int _top = _cache.Length;

    }

  

}