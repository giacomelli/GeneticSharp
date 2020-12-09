using System;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Framework.Threading;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    [DisplayName("TSP (Travelling Salesman Problem)")]
    public class TspSampleController : SampleControllerBase
    {
        #region Fields
        private readonly int m_numberOfCities;
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
        public override void ConfigGA(GeneticAlgorithm ga)
        {
            ga.TaskExecutor = new LinearTaskExecutor();
            base.ConfigGA(ga);
        }

        public override ITermination CreateTermination()
        {
            return new OrTermination(new TimeEvolvingTermination(TimeSpan.FromMinutes(1)), new FitnessStagnationTermination(500));
        }       

        public override IFitness CreateFitness()
        {
            return new TspFitness(m_numberOfCities, 0, 1000, 0, 1000);

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
            Console.WriteLine($@"Cities: {c.Length:n0}");
            Console.WriteLine($@"Distance: {c.Distance:n2}");

            var cities = bestChromosome.GetGenes().Select(g => g.Value.ToString()).ToArray();
            Console.WriteLine($@"City tour: {string.Join(", ", cities)}");
        }
        #endregion
    }
}
