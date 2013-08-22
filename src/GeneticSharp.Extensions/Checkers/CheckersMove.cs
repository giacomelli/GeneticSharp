using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticSharp.Extensions.Checkers
{
	/// <summary>
	/// Checkers move.
	/// </summary>
    public class CheckersMove
    {
        #region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Checkers.CheckersMove"/> class.
		/// </summary>
		/// <param name="from">Move from square.</param>
		/// <param name="to">Move to square.</param>
        public CheckersMove(CheckersSquare from, CheckersSquare to)
        {
            From = from;
            To = to;
        }
        #endregion

        #region Properties
		/// <summary>
		/// Gets or sets move from square.
		/// </summary>
        public CheckersSquare From { get; set; }

		/// <summary>
		/// Gets or sets move to square.
		/// </summary>
        public CheckersSquare To { get; set; }
        #endregion
    }
}

