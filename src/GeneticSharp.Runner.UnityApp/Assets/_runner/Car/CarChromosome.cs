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
        private float m_angle;

        public CarChromosome(int vectorsCount, float vectorSize) : base(vectorsCount)
        {
            m_vectorsCount = vectorsCount;
            m_vectorSize = vectorSize;
            m_angle = 360f / vectorsCount;

            for (int i = 0; i < vectorsCount; i++)
            {
                ReplaceGene(i, new Gene(GetRandomRadius()));
            }
        }

        public string ID { get; } = System.Guid.NewGuid().ToString();

        public bool Evaluated { get; set; }
        public float Distance { get; set; }

        public override IChromosome CreateNew()
        {
            return new CarChromosome(m_vectorsCount, m_vectorSize);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(GetRandomRadius());
        }

        public Vector2[] GetVectors()
        {
            return GetGenes().Select((g, i) => GetVector(i, (float)g.Value)).ToArray();
        }


        float GetRandomRadius()
        {
            return RandomizationProvider.Current.GetFloat(0, m_vectorSize);
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