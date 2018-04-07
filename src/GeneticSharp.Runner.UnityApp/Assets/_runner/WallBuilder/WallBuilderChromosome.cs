using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;
using UnityEngine;
using GeneticSharp.Domain.Randomizations;
using System.Linq;

public class WallBuilderChromosome : ChromosomeBase
{
   private Vector3 m_minPosition, m_maxPosition;

    public WallBuilderChromosome(int bricksCount, Vector3 minPosition, Vector3 maxPosition) : base(bricksCount)
    {
        m_minPosition = minPosition;
        m_maxPosition = maxPosition;


        for (int i = 0; i < bricksCount; i++)
        {
            ReplaceGene(i, new Gene(GetRandomPosition()));
        }
    }

    public string ID { get; } = System.Guid.NewGuid().ToString();

    public bool Evaluated { get; set;  }
    public int FloorHits { get; set; }

    public override IChromosome CreateNew()
    {
        return new WallBuilderChromosome(Length, m_minPosition, m_maxPosition);
    }

    public override Gene GenerateGene(int geneIndex)
    {
        return new Gene(GetRandomPosition());
    }

    public Vector3[] GetBricksPositions()
    {
        return GetGenes().Select(g => (Vector3)g.Value).ToArray();
    }


    Vector3 GetRandomPosition()
    {
        var rnd = RandomizationProvider.Current;

        return new Vector3(
            rnd.GetFloat(m_minPosition.x, m_maxPosition.x),
            rnd.GetFloat(m_minPosition.y, m_maxPosition.y),
            rnd.GetFloat(m_minPosition.z, m_maxPosition.z));
    }
}
