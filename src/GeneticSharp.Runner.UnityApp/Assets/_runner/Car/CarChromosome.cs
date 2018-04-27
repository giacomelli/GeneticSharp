using GeneticSharp.Domain.Chromosomes;
using UnityEngine;
using GeneticSharp.Domain.Randomizations;
using System.Linq;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarChromosome : ChromosomeBase
    {
        private CarSampleConfig m_config;
        private float m_angle;

        public CarChromosome(CarSampleConfig config) : base(config.VectorsCount)
        {
            m_config = config;
            m_angle = 360f / config.VectorsCount;

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
            return new CarChromosome(m_config);
        }

        public override Gene GenerateGene(int geneIndex)
        {

            CarGeneValue value;

            if (geneIndex < m_config.WheelsCount)
            {
                value = new CarGeneValue(
                    GetRandomVectorSize(), 
                    GetRandomWheelIndex(), 
                    GetRandomWheelRadius(),
                    GetRandomWheelSpeed());
            }
            else
            {
                value = new CarGeneValue(GetRandomVectorSize());
            }


            return new Gene(value);
   
        }

        public CarGeneValue[] GetGenesValues()
        {
            return GetGenes().Select(g => (CarGeneValue)g.Value).ToArray();
        }

        float GetRandomVectorSize()
        {
            return RandomizationProvider.Current.GetFloat(0, m_config.VectorSize);
        }

        int GetRandomWheelIndex()
        {
            return RandomizationProvider.Current.GetInt(0, m_config.VectorsCount);
        }

        float GetRandomWheelRadius()
        {
            return RandomizationProvider.Current.GetFloat(-m_config.MaxWheelRadius, m_config.MaxWheelRadius);
        }

        float GetRandomWheelSpeed()
        {
            return RandomizationProvider.Current.GetFloat(m_config.MinWheelSpeed, m_config.MaxWheelSpeed);
        }

        public Vector2 GetVector(int geneIndex, float vectorSize)
        {
            var rnd = RandomizationProvider.Current;
            var angle = m_angle * geneIndex;

            // GeneValue is the radius.
            var offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * vectorSize;
            return Vector2.zero + offset;
        }
    }
}