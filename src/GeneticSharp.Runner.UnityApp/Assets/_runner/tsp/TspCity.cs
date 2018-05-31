using UnityEngine;

namespace GeneticSharp.Extensions.Tsp
{
    public class TspCity
    {
        public TspCity(float x, float y)
        {
            X = x;
            Y = y;
        }
  
        public float X { get; set; }
        public float Y { get; set; }
     
    }
}