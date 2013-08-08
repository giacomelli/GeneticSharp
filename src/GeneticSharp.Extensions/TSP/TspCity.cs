using System;
using System.Drawing;

namespace GeneticSharp.Extensions.Tsp
{
	public class TspCity
	{
        public TspCity(int x, int y)
        {
            Location = new Point(x, y);
        }

		public Point Location { get; set; }
	}
}

