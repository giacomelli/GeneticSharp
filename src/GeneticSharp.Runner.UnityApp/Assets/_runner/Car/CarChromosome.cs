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

        public CarChromosome(int vectorsCount, float vectorSize, int wheelsCount, float maxWheelRadius) : base(vectorsCount)
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

            CarGene value;

            if (geneIndex < m_wheelsCount)
            {
                value = new CarGene(GetRandomVectorSize(), GetRandomWheelIndex(), GetRandomWheelRadius());
            }
            else
            {
                value = new CarGene(GetRandomVectorSize(), 0, 0);
            }


            return new Gene(value);
            //if (geneIndex < m_vectorsCount)
            //{
            //    return new Gene(GetRandomVectorSize());
            //}
           
            //if (geneIndex < m_vectorsCount + m_wheelsCount)
            //{
            //    return new Gene(GetRandomWheelIndex());
            //}


            //return new Gene(GetRandomWheelRadius());
        }

        public Vector2[] GetVectors()
        {
            return GetGenes().Select((g, i) => GetVector(i, ((CarGene)g.Value).VectorSize)).ToArray();
        }

        public int[] GetWheelsIndexes()
        {
            return GetGenes().Select(g => ((CarGene)g.Value).WheelIndex).ToArray();
        }

        public float[] GetWheelsRadius()
        {
            return GetGenes().Select(g => ((CarGene)g.Value).WheelRadius).ToArray();
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

        Vector2 GetVector(int geneIndex, float vectorSize)
        {
            var rnd = RandomizationProvider.Current;
            var angle = m_angle * geneIndex;

            // GeneValue is the radius.
            var offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * vectorSize;
            return Vector2.zero + offset;
        }
    }
}