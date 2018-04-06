using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;
using System.Threading;
using UnityEngine;

public class DefenderFitness : IFitness
{
    private int m_secondsForEvaluation = 5;

    public DefenderFitness(int secondsForEvaluation)
    {
        m_secondsForEvaluation = secondsForEvaluation;    
    }

    public DefenderChromosome CurrentChromosomeInEvaluation { get; set; }

    public double Evaluate(IChromosome chromosome)
    {
        var previousHitsCount = TargetController.HitsCount;
        CurrentChromosomeInEvaluation = chromosome as DefenderChromosome;

        Thread.Sleep(m_secondsForEvaluation * 1000);
        var hits = TargetController.HitsCount - previousHitsCount;

        Debug.Log($"Chromosome allowed {hits} hits");

        return hits * -1;
    }

}
