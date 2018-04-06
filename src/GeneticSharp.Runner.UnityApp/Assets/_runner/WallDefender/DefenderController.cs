using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using UnityEngine;

public class DefenderController : MonoBehaviour {

    public int BricksCount = 10;
    public Vector3 MinPosition;
    public Vector3 MaxPosition;
    public Object BrickPrefab;
    public int SecondsForEvaluation = 5;

    private GeneticAlgorithm m_ga;
    private DefenderFitness m_fitness;
	
    private void Start()
	{
        CreateGA();

        new Thread(new ThreadStart(delegate {
            Thread.Sleep(1000);
            m_ga.Start();

        })).Start();
	}

	private void Update()
	{
        if (m_fitness.CurrentChromosomeInEvaluation != null)
        {
            ClearPreviousWall();

            var bricksPositions = m_fitness.CurrentChromosomeInEvaluation.GetBricksPositions();

            foreach (var p in bricksPositions)
            {
                var brick = Object.Instantiate(BrickPrefab, p, Quaternion.identity) as GameObject;
                brick.transform.parent = transform;
            }

            m_fitness.CurrentChromosomeInEvaluation = null;
        }
	}


    private void ClearPreviousWall()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

	void CreateGA()
    {
        m_fitness = new DefenderFitness(SecondsForEvaluation);
        var chromosome = new DefenderChromosome(BricksCount, MinPosition, MaxPosition);
        var crossover = new UniformCrossover();
        var mutation = new UniformMutation(true);
        var selection = new EliteSelection();
        var population = new Population(10, 10, chromosome);
        m_ga = new GeneticAlgorithm(population, m_fitness, selection, crossover, mutation);
        m_ga.Termination = new FitnessStagnationTermination(100000);
        m_ga.TaskExecutor = new LinearTaskExecutor();
        m_ga.GenerationRan += delegate {
            Debug.Log($"Generation: {m_ga.GenerationsNumber} - Best: ${m_ga.BestChromosome.Fitness}");
        
        };
    }
}
