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
        private float m_maxWheelRadius;
        private float m_angle;

        public CarChromosome(int vectorsCount, float vectorSize, int wheelsCount, float maxWheelRadius) : base(vectorsCount + wheelsCount + wheelsCount)
        {
            m_vectorsCount = vectorsCount;
            m_vectorSize = vectorSize;
            m_wheelsCount = wheelsCount;
            m_maxWheelRadius = maxWheelRadius;
            m_angle = 360f / vectorsCount;

            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        public string ID { get; } = System.Guid.NewGuid().ToString();

        public bool Evaluated { get; set; }
        public float MaxDistance { get; set; }

        public override IChromosome CreateNew()
        {
            return new CarChromosome(m_vectorsCount, m_vectorSize, m_wheelsCount, m_maxWheelRadius);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            if (geneIndex < m_vectorsCount)
            {
                return new Gene(GetRandomVectorSize());
            }
           
            if (geneIndex < m_vectorsCount + m_wheelsCount)
            {
                return new Gene(GetRandomWheelIndex());
            }


            return new Gene(GetRandomWheelRadius());
        }

        public Vector2[] GetVectors()
        {
            return GetGenes().Take(m_vectorsCount).Select((g, i) => GetVector(i, (float)g.Value)).ToArray();
        }

        public int[] GetWheelsIndexes()
        {
            return GetGenes().Skip(m_vectorsCount).Take(m_wheelsCount).Select(g => (int)g.Value).ToArray();
        }

        public float[] GetWheelsRadius()
        {
            return GetGenes().Skip(m_vectorsCount + m_wheelsCount).Select(g => (float)g.Value).ToArray();
        }

        float GetRandomVectorSize()
        {
            return RandomizationProvider.Current.GetFloat(0, m_vectorSize);
        }

        int GetRandomWheelIndex()
        {
            return RandomizationProvider.Current.GetInt(0, m_vectorsCount);
        }

        float GetRandomWheelRadius()
        {
            return RandomizationProvider.Current.GetFloat(-m_maxWheelRadius, m_maxWheelRadius);
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