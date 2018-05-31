using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Framework.Threading;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Tsp
{
    /// <summary>
    /// TSP (Travelling Salesman Problem) sample controller.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class TspController : SampleControllerBase
    {
        private TspFitness m_fitness;
       
        private LineRenderer m_lr;
        public Object CityPrefab;
        public int m_numberOfCities = 50;
      
        protected override GeneticAlgorithm CreateGA()
        {
            var size = (int)Camera.main.orthographicSize - 1;
            m_fitness = new TspFitness(m_numberOfCities, -size, size, -size, size);
            var chromosome = new TspChromosome(m_numberOfCities);
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();
            var selection = new RouletteWheelSelection();
            var population = new Population(50, 100, chromosome);
            var ga = new GeneticAlgorithm(population, m_fitness, selection, crossover, mutation);
            ga.Termination = new TimeEvolvingTermination(System.TimeSpan.FromDays(1));
            ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = 100,
                MaxThreads = 200
            };

            return ga;
        }

		protected override void StartSample()
		{
            m_lr = GetComponent<LineRenderer>();
            m_lr.positionCount = m_numberOfCities + 1;
            ShowPreviousInfoEnabled = false;
            DrawCities();
		}

		protected override void UpdateSample()
		{
            if (GA.Population.CurrentGeneration == null)
                return;

            var c = GA.BestChromosome as TspChromosome;

            if (c != null)
            {
                SetFitnessText($"Distance: {c.Distance}");
            }
           
            DrawPath(GA.Population.CurrentGeneration);
       	}

		void DrawCities()
        {
            for (int i = 0; i < m_numberOfCities; i++)
            {
                var city = m_fitness.Cities[i];
                var go = Instantiate(CityPrefab, new Vector2(city.X, city.Y), Quaternion.identity) as GameObject;
                go.name = "City " + i;
                go.GetComponent<CityController>().Data = city;
            }
        }

        void DrawPath(Generation generation)
        {
            var c = generation.BestChromosome as TspChromosome;

            if (c != null)
            {
                var genes = c.GetGenes();

                for (int i = 0; i < genes.Length; i++)
                {
                    var city = m_fitness.Cities[(int)genes[i].Value];
                    m_lr.SetPosition(i, new Vector2(city.X, city.Y));
                }

                var firstCity = m_fitness.Cities[(int)genes[0].Value];
                m_lr.SetPosition(m_numberOfCities, new Vector2(firstCity.X, firstCity.Y));
            }
        }
       
    }
}