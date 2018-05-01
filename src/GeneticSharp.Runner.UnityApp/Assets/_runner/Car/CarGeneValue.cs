using UnityEngine;
using System.Collections;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public struct CarGeneValue
    {
        public CarGeneValue(float vectorSize, float vectorAngle, int wheelIndex, float wheelRadius)
        {
            this.VectorSize = vectorSize;
            this.VectorAngle = vectorAngle;
            this.WheelIndex = wheelIndex;
            this.WheelRadius = wheelRadius;
        }

        public CarGeneValue(float vectorSize, float vectorAngle)
            : this(vectorSize, vectorAngle, 0, 0)
        {
        }

        public float VectorSize { get; private set; }
        public float VectorAngle { get; private set; }
        public int WheelIndex { get; private set; }
        public float WheelRadius { get; private set; }
    }

}