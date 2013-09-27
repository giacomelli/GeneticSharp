using System;
using System.Drawing;

namespace GeneticSharp.Extensions.Tsp
{
	/// <summary>
	/// Travelling Salesman city.
	/// </summary>
    public class TspCity
    {
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Tsp.TspCity"/> class.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
        public TspCity(int x, int y)
        {
            X = x; 
            Y = y;
        }
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the x.
		/// </summary>
		/// <value>The x.</value>
        public int X { get; set; }

		/// <summary>
		/// Gets or sets the y.
		/// </summary>
		/// <value>The y.</value>
        public int Y { get; set; }
		#endregion
    }
}