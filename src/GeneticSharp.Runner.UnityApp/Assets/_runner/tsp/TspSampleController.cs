using System.ComponentModel;
using System.Threading;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Framework.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace GeneticSharp.Runner.UnityApp.Tsp
{
    /// <summary>
    /// TSP (Travelling Salesman Problem) sample controller.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class TspSampleController : MonoBehaviour
    {
        private TspFitness m_fitness;
        private Generation m_currentGeneration;
       
        private bool m_showIndexes = true;
        private GeneticAlgorithm m_ga;
        private LineRenderer m_lr;
        public Object CityPrefab;
        public Canvas Canvas;
        public int m_numberOfCities = 50;
        public int FitnessStagnation = 1000;
        public Text GenerationText;
        public Text FitnessText;
       
        void Start()
        {
            CreateGA();

            m_lr = GetComponent<LineRenderer>();
            m_lr.positionCount = m_numberOfCities + 1;

            DrawCities();
           
            new Thread(new ThreadStart(delegate {
                Thread.Sleep(1000);
                m_ga.Start();

            })).Start();
        }

        void CreateGA()
        {
            var r = Canvas.pixelRect;
            var size = (int)Camera.main.orthographicSize - 1;
            m_fitness = new TspFitness(m_numberOfCities, -size, size, -size, size);
            var chromosome = new TspChromosome(m_numberOfCities);
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();
            var selection = new RouletteWheelSelection();
            var population = new Population(50, 100, chromosome);
            m_ga = new GeneticAlgorithm(population, m_fitness, selection, crossover, mutation);
            m_ga.Termination = new FitnessStagnationTermination(FitnessStagnation);
            m_ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = 100,
                MaxThreads = 200
            };
        }

		private void Update()
		{
            var c = m_ga.BestChromosome as TspChromosome;

            if (c != null)
            {
                GenerationText.text = "Generation: " + m_ga.GenerationsNumber;
                FitnessText.text = $"Distance: {c.Distance:N2}";
                DrawPath(m_currentGeneration ?? m_ga.Population.CurrentGeneration);
            }
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

        public void ChangeCurrentGeneration(float index)
        {
            m_currentGeneration = m_ga.Population.Generations[(int)index];
        }

        public void ShuffleCities()
        {
            foreach(var c in m_fitness.Cities)
            {
                var p = m_fitness.GetCityRandomPosition();
                c.X = p.x;
                c.Y = p.y;
            }
        }
    }
}