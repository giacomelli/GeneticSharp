using System;
using System.Collections.Generic;
using System.Diagnostics;
using GeneticSharp.Infrastructure.Framework.Collections;
using GeneticSharp.Infrastructure.Framework.Texts;
using NUnit.Framework;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Texts
{
    [TestFixture]
    public class StringExtensionsTest
    {
        [Test()]
        public void RemovePunctuations_Punctuations_CleanString()
        {
            Assert.AreEqual("`1234567890-=qwertyuiop\\asdfghjklzxcvbnm/", "`1234567890-=q!wer?tyuiop,[]\\asdfghjkl;\'zxcvbnm,./".RemovePunctuations());
        }

        [Test()]
        public void With_SourceAndArgs_Formatted()
        {
            Assert.AreEqual("A1b2", "A{0}b{1}".With(1, 2));
        }


        [Test()]
        public void HammingDistance_EqualLength_CorrectValue()
        {
            Assert.Catch(() => "abc".HammingDistance("ab"));

            var tests = new List<(string s1, string s2, int distance)>();
            tests.Add(("karolin", "kathrin", 3));
            tests.Add(("kathrin", "kerstin", 4));
            tests.Add(("1011101", "1001001", 2));
            tests.Add(("2173896", "2233796", 3));

            tests.Each(tuple => Assert.AreEqual(tuple.distance, tuple.s1.LevenshteinDistance(tuple.s2)));

        }



        [Test()]
        public void LevenshteinDistance_CorrectValue()
        {
            var tests = new List<(string s1, string s2, int distance)>();
            tests.Add(("niche", "chien", 4));
            tests.Add(("GeneticSharp", "SharpGenetic", 10));
            tests.Add(("vladimir", "levenshtein", 9));

            tests.Each(tuple => Assert.AreEqual(tuple.distance, tuple.s1.LevenshteinDistance(tuple.s2)));

          
        }

        [Test()]
        public void DamerauLevenshteinDistance_CorrectValue()
        {

            var tests = new List<(string s1, string s2, int distance)>();
            tests.Add(("climax", "volmax", 3));//value where true DamerauLevenshtein should be 2
            tests.Add(("Ram", "Rom", 1));
            tests.Add(("jellyifhs", "jellyfish", 3));//value where true DamerauLevenshtein should be 2

            tests.Each(tuple => Assert.AreEqual(tuple.distance, tuple.s1.LevenshteinDistance(tuple.s2)));

        }


        [Test()]
        public void DamerauLevenshteinDistanceBoundedVersion_Faster()
        {
            
            var str1 = "abcdefghijklmnopqrstuvwxyzjellyfishabcdefghijklmnopqrstuvwxyz";
            var str2 = "efghijklmnopqrstuvwxyzjellyifshabcdefghijklmnopqrstuvwxyzabcd";

            int d1 = 0, d2 = 0;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                d1 = str1.DamerauLevenshteinDistance(str2);
            }
            var firstDuration = sw.Elapsed;
            sw.Restart();
            for (int i = 0; i < 10000; i++)
            {
                d2 = str1.DamerauLevenshteinDistance(str2, 9);
            }
            var fasterVersionDuration = sw.Elapsed;
            Assert.AreEqual(d1,d2);

            
            Assert.Greater(firstDuration.Ticks, fasterVersionDuration.Ticks * 2);

        }



    }
}
