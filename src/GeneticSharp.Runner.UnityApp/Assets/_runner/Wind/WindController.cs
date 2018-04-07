using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Wind
{
    public class WindController : MonoBehaviour
    {
        public int WindTurbineVertices = 10;
        public Vector3 MinPosition;
        public Vector3 MaxPosition;
        public Object WindTurbinePrefab;
        public int SecondsForEvaluation = 5;
        public float TimeScale = 1;
        public int NumberOfSimultaneousEvaluations = 100;
        public Vector3 EvaluationDistance = new Vector3(0, 0, 2);

        private GeneticAlgorithm m_ga;
        private WindFitness m_fitness;
        private Vector3 m_lastPosition = Vector3.zero;

        private void Start()
        {
            Time.timeScale = TimeScale;
            CreateGA();

            new Thread(new ThreadStart(delegate
            {
                Thread.Sleep(1000);
                m_ga.Start();

            })).Start();
        }

        private void Update()
        {
            // end evaluation.
            while (m_fitness.ChromosomesToEndEvaluation.Count > 0)
            {
                WindChromosome c;
                m_fitness.ChromosomesToEndEvaluation.TryTake(out c);
                var turbine = GameObject.Find(c.ID);
                GameObject.Destroy(turbine);
                c.Evaluated = true;
            }

            // in evaluation.
            while (m_fitness.ChromosomesToBeginEvaluation.Count > 0)
            {
                WindChromosome c;
                m_fitness.ChromosomesToBeginEvaluation.TryTake(out c);
                c.Evaluated = false;
                c.Turns = 0;

                var turbine = Object.Instantiate(WindTurbinePrefab, m_lastPosition, Quaternion.identity) as GameObject;
                turbine.name = c.ID;
                m_lastPosition += EvaluationDistance;
                var vertices = c.GetVertices();

                var pc = turbine.GetComponent<PolygonCollider2D>();
                pc.points = vertices;

                var lr = turbine.GetComponent<LineRenderer>();
                lr.SetPositions(vertices.Select(v => new Vector3(v.x, v.y, turbine.transform.position.z)).ToArray());
            }
        }

        void CreateGA()
        {
            m_fitness = new WindFitness(SecondsForEvaluation / TimeScale);
            var chromosome = new WindChromosome(WindTurbineVertices, MinPosition, MaxPosition);
            var crossover = new UniformCrossover();
            var mutation = new UniformMutation(true);
            var selection = new EliteSelection();
            var population = new Population(NumberOfSimultaneousEvaluations, NumberOfSimultaneousEvaluations, chromosome);
            m_ga = new GeneticAlgorithm(population, m_fitness, selection, crossover, mutation);
            m_ga.Termination = new FitnessStagnationTermination(100000);
            m_ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = population.MinSize,
                MaxThreads = population.MaxSize * 2
            };
            m_ga.GenerationRan += delegate
            {
                Debug.Log($"Generation: {m_ga.GenerationsNumber} - Best: ${m_ga.BestChromosome.Fitness}");
                m_lastPosition = Vector3.zero;
            };
        }
    }
}