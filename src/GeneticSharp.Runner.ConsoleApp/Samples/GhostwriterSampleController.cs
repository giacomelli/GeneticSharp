using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Extensions.Ghostwriter;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    public class GhostwriterSampleController : ISampleController
    {
        private Dictionary<string, int> m_wordDefitionsCache = new Dictionary<string, int>();

        public IFitness CreateFitness()
        {
            var db = "";

            foreach (var file in Directory.GetFiles(@"C:\Users\giacomelli\Downloads\wn3.1.dict\dict\dbfiles", "*.*"))
            {
                db += File.ReadAllText(file).ToUpperInvariant();
            }
            
            var f = new GhostwriterFitness();

            f.EvaluateFunc = (text) =>
            {
                //Console.WriteLine("Evaluating text: {0}", text);
                var words = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var points = 0.0;
                var wordsNotFound = 0.0;

                foreach (var word in words)
                {
                    try
                    {
                        if (db.Contains(word))
                        {
                            points += word.Length;
                        }
                        else 
                        {
                            wordsNotFound += word.Length * 2;
                        }
                    }
                    catch
                    {
                        points--;
                        continue;
                    }
                }

                if (points > 100)
                {
                    points = 100;
                }
                else if (points < 0)
                {
                    points = 0;
                }

                var fitness = (points / wordsNotFound) / 100.0;

                //onsole.WriteLine("Fitness: {0}", fitness);

                return fitness;
            };

            return f;
        }

        public IChromosome CreateChromosome()
        {
            return new GhostwriterChromosome();
        }

        public void Draw(IChromosome bestChromosome)
        {
            var c = bestChromosome as GhostwriterChromosome;
            Console.WriteLine("Text: {0}", c.GetText());
        }
    }
}
