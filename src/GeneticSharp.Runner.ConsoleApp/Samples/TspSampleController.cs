using System;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    [DisplayName("TSP (Travelling Salesman Problem)")]
    public class TspSampleController : SampleControllerBase
    {
        #region Fields
        private int m_numberOfCities;
        private TspFitness m_fitness;
        #endregion

        #region Constructors
        public TspSampleController() : this(20)
        {
        }

        public TspSampleController(int numberOfCities)
        {
            m_numberOfCities = numberOfCities;
        }
        #endregion

        #region Methods
        public override ITermination CreateTermination()
        {
            return new OrTermination(new TimeEvolvingTermination(TimeSpan.FromMinutes(1)), new FitnessStagnationTermination(500));
        }       

        public override IFitness CreateFitness()
        {
            m_fitness = new TspFitness(m_numberOfCities, 0, 1000, 0, 1000);

            return m_fitness;
        }

        public override IChromosome CreateChromosome()
        {
            return new TspChromosome(m_numberOfCities);
        }

        public override ICrossover CreateCrossover()
        {
            return new OrderedCrossover();
        }

        public override IMutation CreateMutation()
        {
            return new ReverseSequenceMutation();
        }

        /// <summary>
        /// Draws the sample.
        /// </summary>
        /// <param name="bestChromosome">The current best chromosome</param>
        public override void Draw(IChromosome bestChromosome)
        {
            var c = bestChromosome as TspChromosome;
            Console.WriteLine("Cities: {0:n0}", c.Length);
            Console.WriteLine("Distance: {0:n2}", c.Distance);

            var cities = bestChromosome.GetGenes().Select(g => g.Value.ToString()).ToArray();
            Console.WriteLine("City tour: {0}", string.Join(", ", cities));
        }
        #endregion
    }
}
