using UnityEngine;
using System.Collections;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;
using GeneticSharp.Infrastructure.Framework.Commons;
using System.Linq;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public struct CarGeneValue
    {

		//public CarGeneValue(float vectorSize, float vectorAngle, int wheelIndex, float wheelRadius)
        //{
        //    this.VectorSize = vectorSize;
        //    this.VectorAngle = vectorAngle;
        //    this.WheelIndex = wheelIndex;
        //    this.WheelRadius = wheelRadius;
        //}

        //public CarGeneValue(float vectorSize, float vectorAngle)
        //    : this(vectorSize, vectorAngle, 0, 0)
        //{
        //}

		public CarGeneValue(CarSampleConfig config,  int phenotypeIndex, IEnumerable<int> genes) : this()
		{
            var skip = 0;
            VectorSize =  GetFloat(genes, skip, CarChromosome.VectorSizeBits, 1, config.MaxVectorSize);
            skip += CarChromosome.VectorSizeBits;

            VectorAngle = GetFloat(genes, skip, CarChromosome.VectorAngleBits, 0, 359);
            skip += CarChromosome.VectorAngleBits;

            WheelIndex = phenotypeIndex < config.WheelsCount ? GetInt(genes, skip, CarChromosome.WheelIndexBits, 0, config.WheelsCount - 1) : 0;
            skip += CarChromosome.WheelIndexBits;

            WheelRadius = phenotypeIndex < config.WheelsCount ? GetFloat(genes, skip, CarChromosome.WheelRadiusBits, 0, config.MaxWheelRadius) : 0;
        }

		public float VectorSize { get; private set; }
        public float VectorAngle { get; private set; }
        public int WheelIndex { get; private set; }
        public float WheelRadius { get; private set; }

        private float GetFloat(IEnumerable<int> genes, int skip, int take, float min, float max)
        {
            var representation = string.Join("", genes.Skip(skip).Take(take));
            var value = (float)BinaryStringRepresentation.ToDouble(representation, 0);

            // Debug.Log($"{representation} = {value}");

            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        private int GetInt(IEnumerable<int> genes, int skip, int take, int min, int max)
        {
            var value = (int)BinaryStringRepresentation.ToInt64(string.Join("", genes.Skip(skip).Take(take)));

            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }
    }

}