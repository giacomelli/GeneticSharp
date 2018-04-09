using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeneticSharp.Domain;
using System.Threading;

public abstract class SampleControllerBase : MonoBehaviour {

    private Thread m_gaThread;

    protected Canvas Canvas { get; private set; }
    protected Text GenerationText { get; private set; }
    protected Text FitnessText { get; private set;  }
    protected GeneticAlgorithm GA { get; private set; }

	private void Start()
	{
        Canvas = GameObject.Find("Menu").GetComponent<Canvas>();
        GenerationText = GameObject.Find("GenerationText").GetComponent<Text>();
        FitnessText = GameObject.Find("FitnessText").GetComponent<Text>();

        GenerationText.text = string.Empty;
        FitnessText.text = string.Empty;

        GA = CreateGA();

        StartSample();

        m_gaThread = new Thread(new ThreadStart(delegate
        {
            Thread.Sleep(1000);
            GA.Start();
        }));
        m_gaThread.Start();
	}

    void Update()
    {
        GenerationText.text = $"Generation: {GA.GenerationsNumber}";

        if (GA.BestChromosome != null)
        {
            FitnessText.text = $"Fitness: {GA.BestChromosome.Fitness.Value:N2}";
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
