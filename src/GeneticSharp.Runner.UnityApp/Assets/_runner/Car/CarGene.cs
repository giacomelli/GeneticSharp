using UnityEngine;
using System.Collections;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public struct CarGene
    {
        public CarGene(float vectorSize, int wheelIndex, float wheelRadius)
        {
            this.VectorSize = vectorSize;
            this.WheelIndex = wheelIndex;
            this.WheelRadius = wheelRadius;
        }

        public float VectorSize { get; private set; }
        public int WheelIndex { get; private set; }
        public float WheelRadius { get; private set; }
    }

}