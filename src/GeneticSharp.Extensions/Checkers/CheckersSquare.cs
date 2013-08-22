using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticSharp.Extensions.Checkers
{
    #region Enums
	/// <summary>
	/// Checkers square state.
	/// </summary>
    public enum CheckersSquareState
    {
		/// <summary>
		/// Square is free.
		/// </summary>
        Free,

		/// <summary>
		/// Square is not playble (white one).
		/// </summary>
        NotPlayable,

		/// <summary>
		/// Square is occupied by player one.
		/// </summary>
        OccupiedByPlayerOne,

		/// <summary>
		/// Square is occupied by playe two.
		/// </summary>
        OccupiedByPlayerTwo
    }
    #endregion

	/// <summary>
	/// Checkers square.
	/// </summary>
    public sealed class CheckersSquare
    {
        #region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Checkers.CheckersSquare"/> class.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <param name="rowIndex">Row index.</param>
        public CheckersSquare(int columnIndex, int rowIndex)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;

            if((ColumnIndex % 2 == 0 && RowIndex % 2 != 0) || (ColumnIndex % 2 != 0 && RowIndex % 2 == 0))
            {
                State = CheckersSquareState.Free;
            }
            else {
                State = CheckersSquareState.NotPlayable;
            }
        }
        #endregion

        #region Properties
		/// <summary>
		/// Gets or sets the index of the column.
		/// </summary>
		/// <value>The index of the column.</value>
        public int ColumnIndex { get; set; }

		/// <summary>
		/// Gets or sets the index of the row.
		/// </summary>
		/// <value>The index of the row.</value>
        public int RowIndex { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>The state.</value>
        public CheckersSquareState State { get; set; }
        #endregion
    }
}

