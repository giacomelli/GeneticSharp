using System.Threading;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using UnityEngine;

public class GAController : MonoBehaviour
{
    private GeneticAlgorithm m_ga;
    private Thread m_gaThread;
    private LineRenderer m_lr;

    public Object CityPrefab;
    public int m_numberOfCities = 20;

    private void Awake()
    {
        m_lr = GetComponent<LineRenderer>();
        m_lr.positionCount = m_numberOfCities + 1;
    }
 
    private void Start()
    {
        var fitness = new TspFitness(m_numberOfCities);
        var chromosome = new TspChromosome(m_numberOfCities);

        // This operators are classic genetic algorithm operators that lead to a good solution on TSP,
        // but you can try others combinations and see what result you get.
        var crossover = new OrderedCrossover();
        var mutation = new ReverseSequenceMutation();
        var selection = new RouletteWheelSelection();
        var population = new Population(50, 100, chromosome);
       
        m_ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
        m_ga.Termination = new TimeEvolvingTermination(System.TimeSpan.FromHours(1));

        // The fitness evaluation of whole population will be running on parallel.
        m_ga.TaskExecutor = new ParallelTaskExecutor
        {
            MinThreads = 100,
            MaxThreads = 200
        };

        // Everty time a generation ends, we log the best solution.
        m_ga.GenerationRan += delegate
        {
            var distance = ((TspChromosome)m_ga.BestChromosome).Distance;
            Debug.Log($"Generation: {m_ga.GenerationsNumber} - Distance: ${distance}");
        };

        DrawCities();

        // Starts the genetic algorithm in a separate thread.
        m_gaThread = new Thread(() => m_ga.Start());
        m_gaThread.Start();
    }

    private void Update()
    {
        DrawRoute();
    }

    private void OnDestroy()
    {
        // When the script is destroyed we stop the genetic algorithm and abort its thread too.
        m_ga.Stop();
        m_gaThread.Abort();
    }

    void DrawCities()
    {
        var cities = ((TspFitness)m_ga.Fitness).Cities;

        for (int i = 0; i < m_numberOfCities; i++)
        {
            var city = cities[i];
            var go = Instantiate(CityPrefab, city.Position, Quaternion.identity) as GameObject;
            go.name = "City " + i;
            go.GetComponent<CityController>().Data = city;
        }
    }

    void DrawRoute()
    {
        var c = m_ga.Population.CurrentGeneration.BestChromosome as TspChromosome;

        if (c != null)
        {
            var genes = c.GetGenes();
            var cities = ((TspFitness)m_ga.Fitness).Cities;

            for (int i = 0; i < genes.Length; i++)
            {
                var city = cities[(int)genes[i].Value];
                m_lr.SetPosition(i, city.Position);
            }

            var firstCity = cities[(int)genes[0].Value];
            m_lr.SetPosition(m_numberOfCities, firstCity.Position);
        }
    }
}