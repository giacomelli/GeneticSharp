using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HelperSharp;

namespace GeneticSharp.Extensions.Checkers
{
    #region Enums
    /// <summary>
    /// The checkers player.
    /// </summary>
    public enum CheckersPlayer
    {
        /// <summary>
        /// The player one.
        /// </summary>
        PlayerOne,

        /// <summary>
        /// The player two.
        /// </summary>
        PlayerTwo
    }
    #endregion

    /// <summary>
    /// Checkers board.
    /// </summary>
    public class CheckersBoard
    {
        #region Fields
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member", Justification = "Better to checkers problem")]
        private CheckersSquare[,] m_squares;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Checkers.CheckersBoard"/> class.
        /// </summary>
        /// <param name="size">The board size in number of squares of each side.</param>
        public CheckersBoard(int size)
        {
            if (size < 8)
            {
                throw new ArgumentException("The minimum valid size is 8.");
            }

            Size = size;
            m_squares = new CheckersSquare[size, size];
            Reset();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the size.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Gets the player one's pieces.
        /// </summary>
        public IList<CheckersPiece> PlayerOnePieces { get; private set; }

        /// <summary>
        /// Gets the player two's pieces.
        /// </summary>
        public IList<CheckersPiece> PlayerTwoPieces { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Reset the board to initial state (player one and two with pieces in start positions).
        /// </summary>
        public void Reset()
        {
            PlayerOnePieces = new List<CheckersPiece>();
            PlayerTwoPieces = new List<CheckersPiece>();

            for (int c = 0; c < Size; c++)
            {
                for (int r = 0; r < Size; r++)
                {
                    var square = new CheckersSquare(c, r);

                    if (square.State == CheckersSquareState.Free)
                    {
                        if (r < 3)
                        {
                            var piece = new CheckersPiece(CheckersPlayer.PlayerOne);
                            PlayerOnePieces.Add(piece);
                            square.PutPiece(piece);
                        }
                        else if (r >= Size - 3)
                        {
                            var piece = new CheckersPiece(CheckersPlayer.PlayerTwo);
                            PlayerTwoPieces.Add(piece);
                            square.PutPiece(piece);
                        }
                    }

                    m_squares[c, r] = square;
                }
            }
        }

        /// <summary>
        /// Gets the square from column index and row index specified.
        /// </summary>
        /// <returns>The square.</returns>
        /// <param name="columnIndex">Column index.</param>
        /// <param name="rowIndex">Row index.</param>
        public CheckersSquare GetSquare(int columnIndex, int rowIndex)
        {
            if (!IsValidIndex(columnIndex))
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            if (!IsValidIndex(rowIndex))
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            return m_squares[columnIndex, rowIndex];
        }

        /// <summary>
        /// Move a piece using the specified move.
        /// </summary>
        /// <param name="move">The move to perform.</param>
        /// <returns>True if move was performed, otherwise false.</returns>
        public bool MovePiece(CheckersMove move)
        {
            ExceptionHelper.ThrowIfNull("move", move);

            bool moved = false;
            var from = GetSquare(move.Piece.CurrentSquare.ColumnIndex, move.Piece.CurrentSquare.RowIndex);
            var moveKind = GetMoveKind(move);

            if (moveKind != CheckersMoveKind.Invalid)
            {
                var to = GetSquare(move.ToSquare.ColumnIndex, move.ToSquare.RowIndex);
                to.PutPiece(from.CurrentPiece);

                var indexModifier = to.State == CheckersSquareState.OccupiedByPlayerOne ? 1 : -1;
                from.RemovePiece();

                moved = true;

                // Capture move.
                if (moveKind == CheckersMoveKind.Capture)
                {
                    if (to.ColumnIndex == from.ColumnIndex + (2 * indexModifier))
                    {
                        GetSquare(from.ColumnIndex + (1 * indexModifier), from.RowIndex + (1 * indexModifier)).RemovePiece();
                    }
                    else if (to.ColumnIndex == from.ColumnIndex - (2 * indexModifier))
                    {
                        GetSquare(from.ColumnIndex - (1 * indexModifier), from.RowIndex + (1 * indexModifier)).RemovePiece();
                    }

                    moved = true;
                }
            }

            return moved;
        }

        /// <summary>
        /// Gets the kind of the move.
        /// </summary>
        /// <returns>The move kind.</returns>
        /// <param name="move">The move.</param>
        public CheckersMoveKind GetMoveKind(CheckersMove move)
        {
            var kind = CheckersMoveKind.Invalid;
            var player = move.Piece.Player;
            var currentSquareState = move.Piece.CurrentSquare.State;

            if (currentSquareState == CheckersSquareState.OccupiedByPlayerOne || currentSquareState == CheckersSquareState.OccupiedByPlayerTwo)
            {
                var from = GetSquare(move.Piece.CurrentSquare.ColumnIndex, move.Piece.CurrentSquare.RowIndex);
                var to = GetSquare(move.ToSquare.ColumnIndex, move.ToSquare.RowIndex);

                // From is square of the AI player and To is a free square.
                if (from.State == currentSquareState && to.State == CheckersSquareState.Free)
                {
                    int indexModifier = GetIndexModifier(player);
                    CheckersSquareState opponentState;

                    if (from.State == CheckersSquareState.OccupiedByPlayerOne)
                    {
                        opponentState = CheckersSquareState.OccupiedByPlayerTwo;
                    }
                    else
                    {
                        opponentState = CheckersSquareState.OccupiedByPlayerOne;
                    }

                    // Forward move.
                    if (to.RowIndex == from.RowIndex + (1 * indexModifier)
                        && (to.ColumnIndex == from.ColumnIndex - (1 * indexModifier) || to.ColumnIndex == from.ColumnIndex + (1 * indexModifier)))
                    {
                        kind = CheckersMoveKind.Forward;
                    }
                    else
                    if (to.RowIndex == from.RowIndex + (2 * indexModifier))
                    {
                        // Capture move.

                        // To right or To left?
                        if (to.ColumnIndex == from.ColumnIndex + (2 * indexModifier)
                        && GetSquare(from.ColumnIndex + (1 * indexModifier), from.RowIndex + (1 * indexModifier)).State == opponentState)
                        {
                            kind = CheckersMoveKind.Capture;
                        }
                        else
                        if (to.ColumnIndex == from.ColumnIndex - (2 * indexModifier)
                        && GetSquare(from.ColumnIndex - (1 * indexModifier), from.RowIndex + (1 * indexModifier)).State == opponentState)
                        {
                            kind = CheckersMoveKind.Capture;
                        }
                    }
                }
            }

            return kind;
        }

        /// <summary>
        /// Counts the number of pieces catchable by specified piece.
        /// </summary>
        /// <param name="piece">The piece which can capture another ones.</param>
        /// <returns>The number of catchable pieces.</returns>
        public int CountCatchableByPiece(CheckersPiece piece)
        {
            ExceptionHelper.ThrowIfNull("piece", piece);

            var capturableCount = 0;
            var square = piece.CurrentSquare;
            var newRowIndex = square.RowIndex + (2 * GetIndexModifier(piece.Player));

            if (IsValidIndex(newRowIndex))
            {
                var columnIndex = square.ColumnIndex;
                var newColumnToLeftIndex = columnIndex - 2;
                var newColumnToRightIndex = columnIndex + 2;

                if (IsValidIndex(newColumnToLeftIndex))
                {
                    capturableCount += GetMoveKind(new CheckersMove(piece, GetSquare(newColumnToLeftIndex, newRowIndex))) == CheckersMoveKind.Capture ? 1 : 0;
                }

                if (IsValidIndex(newColumnToRightIndex))
                {
                    capturableCount += GetMoveKind(new CheckersMove(piece, GetSquare(newColumnToRightIndex, newRowIndex))) == CheckersMoveKind.Capture ? 1 : 0;
                }
            }

            return capturableCount;
        }

        /// <summary>
        /// Counts the number of chances the specified piece to be captured.
        /// </summary>
        /// <param name="piece">The piece which can be captured.</param>
        /// <returns>The number of changes of piece be captured.</returns>
        public int CountPieceChancesToBeCaptured(CheckersPiece piece)
        {
            ExceptionHelper.ThrowIfNull("piece", piece);

            var capturedCount = 0;
            var square = piece.CurrentSquare;

            var indexModifier = GetIndexModifier(piece.Player);
            var enemyPieceRowIndex = square.RowIndex + indexModifier;
            var enemyToSquareRowIndex = square.RowIndex - indexModifier;

            if (IsValidIndex(enemyPieceRowIndex) && IsValidIndex(enemyToSquareRowIndex))
            {
                var columnIndex = square.ColumnIndex;
                var enemyLeftColumnIndex = columnIndex - 1;
                var enemyRightColumnIndex = columnIndex + 1;

                if (IsValidIndex(enemyLeftColumnIndex) && IsValidIndex(enemyRightColumnIndex))
                {
                    var enemyPiece = GetSquare(enemyLeftColumnIndex, enemyPieceRowIndex).CurrentPiece;

                    if (enemyPiece != null)
                    {
                        capturedCount += GetMoveKind(new CheckersMove(enemyPiece, GetSquare(enemyRightColumnIndex, enemyToSquareRowIndex))) == CheckersMoveKind.Capture ? 1 : 0;
                    }

                    enemyPiece = GetSquare(enemyRightColumnIndex, enemyPieceRowIndex).CurrentPiece;

                    if (enemyPiece != null)
                    {
                        capturedCount += GetMoveKind(new CheckersMove(enemyPiece, GetSquare(enemyLeftColumnIndex, enemyToSquareRowIndex))) == CheckersMoveKind.Capture ? 1 : 0;
                    }
                }
            }

            return capturedCount;
        }
        #endregion

        #region Helpers
        private static int GetIndexModifier(CheckersPlayer player)
        {
            int indexModifier;

            if (player == CheckersPlayer.PlayerOne)
            {
                indexModifier = 1;
            }
            else
            {
                indexModifier = -1;
            }

            return indexModifier;
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < Size;
        }
        #endregion
    }
}