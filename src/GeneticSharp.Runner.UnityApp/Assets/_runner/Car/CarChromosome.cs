using GeneticSharp.Domain.Chromosomes;
using UnityEngine;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using System;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarChromosome : StringChromosome
    {
        private CarSampleConfig m_config;
        private string m_originalValueStringRepresentation;

        public const int VectorSizeBits = 7;
        public const int VectorAngleBits = 9;
        public const int WheelIndexBits = 7;
        public const int WheelRadiusBits = 4;
        public const int PhenotypeSize = VectorSizeBits + VectorAngleBits + WheelIndexBits + WheelRadiusBits;

        public CarChromosome(CarSampleConfig config)
            : base(config.VectorsCount * PhenotypeSize)
        {
            m_config = config;

            var originalValues = new double[Length];
            var totalBits = new int[Length];
            var fractionBits = new int[Length];

            for (int i = 0; i < originalValues.Length; i += 4)
            {
                originalValues[i] = GetRandomVectorSize();
                originalValues[i + 1] = GetRandomVectorAngle();
                originalValues[i + 2] = GetRandomWheelIndex();
                originalValues[i + 3] = GetRandomWheelRadius();

                totalBits[i] = VectorSizeBits;
                totalBits[i + 1] = VectorAngleBits;
                totalBits[i + 2] = WheelIndexBits;
                totalBits[i + 3] = WheelRadiusBits;
            };

            m_originalValueStringRepresentation = String.Join(
                "",
                BinaryStringRepresentation.ToRepresentation(
                originalValues,
                    totalBits,
                    fractionBits));
            
            CreateGenes();
        }

        public string ID { get; } = System.Guid.NewGuid().ToString();

        public bool Evaluated { get; set; }
        public float MaxDistance { get; set; }
        public float MaxDistanceTime { get; set; }
      
        public override IChromosome CreateNew()
        {
            return new CarChromosome(m_config);
        }
	
		public CarGeneValue[] GetGenesValues()
        {
            var genes = GetGenes();
            var phenotype = new CarGeneValue[genes.Length / PhenotypeSize];
            var phenotypeIndex = 0;

            for (int i = 0; i < genes.Length; i += PhenotypeSize)
            {
                phenotype[phenotypeIndex] = new CarGeneValue(m_config, phenotypeIndex, genes.Select(g => (int)g.Value).Skip(i).Take(PhenotypeSize));
                phenotypeIndex++;                        
            }

            return phenotype;
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(Convert.ToInt32(m_originalValueStringRepresentation[geneIndex].ToString()));
        }

        float GetRandomVectorSize()
        {
            return RandomizationProvider.Current.GetFloat(1, m_config.MaxVectorSize);
        }

        float GetRandomVectorAngle()
        {
            return RandomizationProvider.Current.GetFloat(0, 360);
        }

        int GetRandomWheelIndex()
        {
            return RandomizationProvider.Current.GetInt(0, m_config.VectorsCount);
        }

        float GetRandomWheelRadius()
        {
            // The range is form negative to positive MaxWheelRadius, because we want the same probability to have a 
            // point with a wheel and without one.
            var radius =  RandomizationProvider.Current.GetFloat(-m_config.MaxWheelRadius, m_config.MaxWheelRadius);

            return radius > 0 ? radius : 0;
        }

        public Vector2 GetVector(CarGeneValue geneValue)
        {
            var rnd = RandomizationProvider.Current;
        
            // GeneValue is the radius.
            var offset = new Vector2(Mathf.Sin(geneValue.VectorAngle), Mathf.Cos(geneValue.VectorAngle)) * geneValue.VectorSize;
            return Vector2.zero + offset;
        }
    }
}