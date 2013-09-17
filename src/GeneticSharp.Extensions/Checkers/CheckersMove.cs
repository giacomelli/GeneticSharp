using System;
using System.Diagnostics;
using HelperSharp;

namespace GeneticSharp.Extensions.Checkers
{
    #region Enums
	/// <summary>
	///  Moove kind.
	/// </summary>
    public enum CheckersMoveKind
    {
		/// <summary>
		/// The move is invalid.
		/// </summary>
        Invalid,

		/// <summary>
		/// A forward move.
		/// </summary>
        Forward,

		/// <summary>
		/// A capture move.
		/// </summary>
        Capture
    }
    #endregion

    /// <summary>
	/// Checkers move.
	/// </summary>
    [DebuggerDisplay("({From.ColumnIndex}, {From.RowIndex}) -> ({To.ColumnIndex}, {To.RowIndex})")]
    public class CheckersMove
    {        
        #region Constructors
        /// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Checkers.CheckersMove"/> class.
		/// </summary>
		/// <param name="piece">The piece which will be moved.</param>
		/// <param name="toSquare">The target square.</param>
        public CheckersMove(CheckersPiece piece, CheckersSquare toSquare)
        {
            ExceptionHelper.ThrowIfNull("piece", piece);

            if (piece.CurrentSquare == null)
            {
                throw new ArgumentException("A piece for a move should have a current square defined.");
            }

            ExceptionHelper.ThrowIfNull("toSquare", toSquare);
            
            Piece = piece;
            ToSquare = toSquare;
        }
        #endregion

        #region Properties
		/// <summary>
		/// Gets or sets move from square.
		/// </summary>
        public CheckersPiece Piece { get; set; }

		/// <summary>
		/// Gets or sets move to square.
		/// </summary>
        public CheckersSquare ToSquare { get; set; }
        #endregion
    }
}

