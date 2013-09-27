using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Extensions.Tsp;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    public class TspSampleController : ISampleController
    {
        private int m_numberOfCities;
        private TspFitness m_fitness;

        public TspSampleController(int numberOfCities)
        {
            m_numberOfCities = numberOfCities;
        }

        public IFitness CreateFitness()
        {
            m_fitness = new TspFitness(m_numberOfCities, 0, 1000, 0, 1000);
            
            return m_fitness;
        }

        public IChromosome CreateChromosome()
        {
            return new TspChromosome(m_numberOfCities);
        }

        /// <summary>
        /// Draws the sample;
        /// </summary>
        public void Draw(IChromosome bestChromosome)
        {
            var c = bestChromosome as TspChromosome;
            Console.WriteLine("Distance: {0:n2}", c.Distance);

			//var cities = bestChromosome.GetGenes ().Select (g => g.Value.ToString ()).ToArray ();
            //Console.WriteLine("City tour: {0}", String.Join(", ", cities));
        }
    }
}
