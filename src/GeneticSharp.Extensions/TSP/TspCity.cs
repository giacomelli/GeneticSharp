using System;
using System.Drawing;

namespace GeneticSharp.Extensions.Tsp
{
    public class TspCity
    {
        public TspCity(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}

