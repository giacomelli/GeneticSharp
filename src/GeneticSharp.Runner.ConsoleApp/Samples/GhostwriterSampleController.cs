using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Extensions.Ghostwriter;
using GeneticSharp.Runner.ConsoleApp.Samples.Resources;
using HelperSharp;
using Newtonsoft.Json;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    public class GhostwriterSampleController : ISampleController
    {
        private List<string> m_quotes;
        private List<string> m_words;

        public GhostwriterSampleController()
        {            
            var json = JsonConvert.DeserializeObject<dynamic>(SamplesResource.GhostwriterQuoteJson);
            m_quotes = new List<string>();
            m_words = new List<string>();

            for (int i = 0; i < json.value.Count; i++)
            {
                var quote = HttpUtility.HtmlDecode(json.value[i].joke.Value) as string;
                m_quotes.Add(quote);
                
                m_words.AddRange(quote.Split(' '));
            }

            m_words = m_words.Select(w => w.RemovePontuactions()).Distinct().OrderBy(w => w).ToList();
        }

        public IFitness CreateFitness()
        {           

            var f = new GhostwriterFitness();

            f.EvaluateFunc = (text) =>
            {
                var minDistance = m_quotes.Min(q => LevenshteinDistance(q, text));

                return 1 - (minDistance/100f);
            };

            return f;
        }

        int LevenshteinDistance(string s, string t)
        {
            // degenerate cases
            if (s == t) return 0;
            if (s.Length == 0) return t.Length;
            if (t.Length == 0) return s.Length;

            // create two work vectors of integer distances
            int[] v0 = new int[t.Length + 1];
            int[] v1 = new int[t.Length + 1];

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (int i = 0; i < v0.Length; i++)
                v0[i] = i;

            for (int i = 0; i < s.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (int j = 0; j < t.Length; j++)
                {
                    var cost = (s[i] == t[j]) ? 0 : 1;
                    v1[j + 1] = Math.Min(Math.Min(v1[j] + 1, v0[j + 1] + 1), v0[j] + cost);
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                for (int j = 0; j < v0.Length; j++)
                    v0[j] = v1[j];
            }

            return v1[t.Length];
        }

        public IChromosome CreateChromosome()
        {
            return new GhostwriterChromosome(5, m_words);
        }

        public void Draw(IChromosome bestChromosome)
        {
            var c = bestChromosome as GhostwriterChromosome;
            Console.WriteLine("Text: {0}", c.GetText());
        }
    }
}
