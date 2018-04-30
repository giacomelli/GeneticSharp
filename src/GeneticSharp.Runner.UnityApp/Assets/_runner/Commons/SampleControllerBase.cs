using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeneticSharp.Domain;
using System.Threading;
using System;
using System.Linq;

public abstract class SampleControllerBase : MonoBehaviour {

    private Thread m_gaThread;
    private double m_bestFitness;
    private double m_averageFitness;
    private bool m_shouldUpdateInfo;

    protected Text GenerationText { get; private set; }
    protected Text FitnessText { get; private set;  }
    protected GeneticAlgorithm GA { get; private set; }
    protected bool ChromosomesCleanupEnabled { get; set; }
    public Rect Area { get; private set; }

	private void Start()
	{
        Application.runInBackground = true;
        var sampleArea = GameObject.Find("SampleArea");
        Area = sampleArea == null
            ? Camera.main.rect
            : sampleArea.GetComponent<RectTransform>().rect;
   
        GenerationText = GameObject.Find("GenerationText")?.GetComponent<Text>();
        FitnessText = GameObject.Find("FitnessText")?.GetComponent<Text>();

        if (GenerationText != null)
        {
            GenerationText.text = string.Empty;
            FitnessText.text = string.Empty;
        }

        GA = CreateGA();
        GA.GenerationRan += delegate {
            m_bestFitness = GA.BestChromosome.Fitness.Value;
            m_averageFitness = GA.Population.CurrentGeneration.Chromosomes.Average(c => c.Fitness.Value);
            Debug.Log($"Generation: {GA.GenerationsNumber} - Best: ${m_bestFitness} - Average: ${m_averageFitness}");

            m_shouldUpdateInfo = true;

            if (ChromosomesCleanupEnabled)
            {
                foreach (var c in GA.Population.CurrentGeneration.Chromosomes)
                {
                    c.Fitness = null;
                }
            }
        };
        StartSample();

        m_gaThread = new Thread(() =>
        {
            try
            {
                Thread.Sleep(1000);
                GA.Start();
            }
            catch(Exception ex)
            {
                Debug.LogError($"GA thread error: {ex.Message}");
            }
        });
        m_gaThread.Start();
	}

    void Update()
    {
        if (GenerationText != null && m_shouldUpdateInfo)
        {
            GenerationText.text = $"Generation: {GA.GenerationsNumber}";
            FitnessText.text = $"Best: {m_bestFitness:N2}\nAverage: {m_averageFitness:N2}";
            m_shouldUpdateInfo = false;
        }

        UpdateSample();
    }

	private void OnDestroy()
	{
        GA.Stop();
        m_gaThread.Abort();
	}

	protected virtual void StartSample() 
    {
        
    }

    protected abstract GeneticAlgorithm CreateGA();

    protected virtual void UpdateSample()
    {
        
    }
}
