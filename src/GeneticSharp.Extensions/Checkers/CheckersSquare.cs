using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [DebuggerDisplay("({ColumnIndex}, {RowIndex}): {State}")]
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
            State = IsNotPlayableSquare(columnIndex, rowIndex) ? CheckersSquareState.NotPlayable : CheckersSquareState.Free;            
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
        public CheckersSquareState State { get; private set; }

		/// <summary>
		/// Gets the current piece.
		/// </summary>
		/// <value>The current piece.</value>
		public CheckersPiece CurrentPiece { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Put the specified piece above this square.
        /// </summary>
        /// <param name="piece">The piece.</param>
        /// <returns>True if square was free and could receive the piece, otherwise false.</returns>
		public bool PutPiece(CheckersPiece piece)
		{
			if (State == CheckersSquareState.Free) {
				CurrentPiece = piece;
				State = piece.Player == CheckersPlayer.PlayerOne ? CheckersSquareState.OccupiedByPlayerOne : CheckersSquareState.OccupiedByPlayerTwo;
				piece.CurrentSquare = this;
				return true;
            }
            else if (State == CheckersSquareState.NotPlayable)
            {
                throw new ArgumentException("Attempt to put a piece in a not playable square.");
            }

			return false;
		}

        /// <summary>
        /// Remove the current piece.
        /// </summary>
        /// <returns>True if has a piece to be removed, otherwise false.</returns>
		public bool RemovePiece()
		{
			if (CurrentPiece != null) {
				if (CurrentPiece.CurrentSquare == this) {
					CurrentPiece.CurrentSquare = null;
				}

				CurrentPiece = null;
				State = CheckersSquareState.Free;

				return true;
			}

			return false;
		}

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as CheckersSquare;

            if (other == null)
            {
                return false;
            }

            return ColumnIndex == other.ColumnIndex
                && RowIndex == other.RowIndex
                && State == other.State;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
 			int hash = 17;
			hash = hash * 23 + ColumnIndex.GetHashCode();
			hash = hash * 23 + RowIndex.GetHashCode();
			hash = hash * 23 + State.GetHashCode();

			return hash;
        }

        /// <summary>
        /// Verifies if the column and row index specified are coordinates of not playable square.
        /// </summary>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>True if it is not playable square.</returns>
        public static bool IsNotPlayableSquare(int columnIndex, int rowIndex)
        {
            return !((columnIndex % 2 == 0 && rowIndex % 2 != 0) || (columnIndex % 2 != 0 && rowIndex % 2 == 0));
        }
        #endregion
    }
}

