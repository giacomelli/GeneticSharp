using GeneticSharp.Domain.Chromosomes;
using UnityEngine;
using GeneticSharp.Domain.Randomizations;
using System.Linq;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarChromosome : ChromosomeBase
    {
        private int m_vectorsCount;
        private float m_vectorSize;
        private int m_wheelsCount;
        private float m_angle;

        public CarChromosome(int vectorsCount, float vectorSize, int wheelsCount) : base(vectorsCount + wheelsCount)
        {
            m_vectorsCount = vectorsCount;
            m_vectorSize = vectorSize;
            m_wheelsCount = wheelsCount;
            m_angle = 360f / vectorsCount;

            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        public string ID { get; } = System.Guid.NewGuid().ToString();

        public bool Evaluated { get; set; }
        public float Distance { get; set; }

        public override IChromosome CreateNew()
        {
            return new CarChromosome(m_vectorsCount, m_vectorSize, m_wheelsCount);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            if (geneIndex < m_vectorsCount)
            {
                return new Gene(GetRandomRadius());
            }

            return new Gene(GetRandomWheelIndex());
        }

        public Vector2[] GetVectors()
        {
            return GetGenes().Take(m_vectorsCount).Select((g, i) => GetVector(i, (float)g.Value)).ToArray();
        }

        public int[] GetWheelsIndexes()
        {
            return GetGenes().Skip(m_vectorsCount).Select(g => (int)g.Value).ToArray();
        }


        float GetRandomRadius()
        {
            return RandomizationProvider.Current.GetFloat(0, m_vectorSize);
        }

        int GetRandomWheelIndex()
        {
            return RandomizationProvider.Current.GetInt(0, m_vectorsCount);
        }

        Vector2 GetVector(int geneIndex, float geneValue)
        {
            var rnd = RandomizationProvider.Current;
            var angle = m_angle * geneIndex;

            // GeneValue is the radius.
            var offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * geneValue;
            return Vector2.zero + offset;
        }
    }
}