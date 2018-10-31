using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Extensions.Checkers
{
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
 
    /// <summary>
    /// Checkers board.
    /// </summary>
    public class CheckersBoard
    {
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member", Justification = "Better to checkers problem")]
        private readonly CheckersSquare[,] m_squares;
 

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
    
        /// <summary>
        /// Reset the board to initial state (player one and two with pieces in start positions).
        /// </summary>
        public void Reset()
        {
            // Creates the two lists of pieces por player one and two.ß
            PlayerOnePieces = new List<CheckersPiece>();
            PlayerTwoPieces = new List<CheckersPiece>();

            for (int c = 0; c < Size; c++)
            {
                for (int r = 0; r < Size; r++)
                {
                    // For each combinatino of collumn and row of the board
                    // Is create a new CheckersSquare.
                    var square = new CheckersSquare(c, r);

                    // If the sqaure is free.
                    if (square.State == CheckersSquareState.Free)
                    {
                        // If the actual line index is lower than 3, 
                        // then is a square for player one.
                        if (r < 3)
                        {
                            var piece = new CheckersPiece(CheckersPlayer.PlayerOne);
                            PlayerOnePieces.Add(piece);
                            square.PutPiece(piece);
                        }
                        /// fi the actual line index is bigger than max lines index -3, 
                        /// then it is a square for player two.
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
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }

            if (!IsValidIndex(rowIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
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
            ExceptionHelper.ThrowIfNull(nameof(move), move);

            // Gets the piece's actual position and movement kind.
            bool moved = false;
            var from = GetSquare(move.Piece.CurrentSquare.ColumnIndex, move.Piece.CurrentSquare.RowIndex);
            var moveKind = GetMoveKind(move);

            // Se the movement kind is invalid between From e To positions.
            if (moveKind != CheckersMoveKind.Invalid)
            {
                // Moves the piece to 'To' position.
                var to = GetSquare(move.ToSquare.ColumnIndex, move.ToSquare.RowIndex);
                to.PutPiece(from.CurrentPiece);

                // Geets the current indexModifier.
                var indexModifier = to.State == CheckersSquareState.OccupiedByPlayerOne ? 1 : -1;
               
                // Removes the piece "From" position.
                from.RemovePiece();

                moved = true;

                // Capture move.
                if (moveKind == CheckersMoveKind.Capture)
                {
                    // Here is checked if needs to capture a piece between To and From.
                    if (to.ColumnIndex == from.ColumnIndex + (2 * indexModifier))
                    {
                        GetSquare(from.ColumnIndex + (1 * indexModifier), from.RowIndex + (1 * indexModifier)).RemovePiece();
                    }
                    else if (to.ColumnIndex == from.ColumnIndex - (2 * indexModifier))
                    {
                        GetSquare(from.ColumnIndex - (1 * indexModifier), from.RowIndex + (1 * indexModifier)).RemovePiece();
                    }
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
                    var opponentState = from.State == CheckersSquareState.OccupiedByPlayerOne
                        ? CheckersSquareState.OccupiedByPlayerTwo
                        : CheckersSquareState.OccupiedByPlayerOne;

                    // Forward move.
                    if (to.RowIndex == from.RowIndex + (1 * indexModifier)
                        && (to.ColumnIndex == from.ColumnIndex - (1 * indexModifier) || to.ColumnIndex == from.ColumnIndex + (1 * indexModifier)))
                    {
                        kind = CheckersMoveKind.Forward;
                    }
                    else if (CanCapture(to, from, opponentState, indexModifier))
                    {
                        kind = CheckersMoveKind.Capture;
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
                    capturedCount += CountIfEnemyPiece(enemyPieceRowIndex, enemyToSquareRowIndex, enemyLeftColumnIndex, enemyRightColumnIndex);
                    capturedCount += CountIfEnemyPiece(enemyPieceRowIndex, enemyToSquareRowIndex, enemyRightColumnIndex, enemyLeftColumnIndex);
                }
            }

            return capturedCount;
        }

        private int CountIfEnemyPiece(int enemyFromRowIndex, int enemyToRowIndex, int enemyFromColumnIndex, int enemyToColumnIndex)
        {
            var enemyPiece = GetSquare(enemyFromColumnIndex, enemyFromRowIndex).CurrentPiece;

            if (enemyPiece != null)
            {
                return GetMoveKind(new CheckersMove(enemyPiece, GetSquare(enemyToColumnIndex, enemyToRowIndex))) == CheckersMoveKind.Capture ? 1 : 0;
            }

            return 0;
        }
       

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

        private bool CanCapture(CheckersSquare to, CheckersSquare from, CheckersSquareState opponentState, int indexModifier)
        {
            if (to.RowIndex == from.RowIndex + (2 * indexModifier))
            {
                // To right or To left?
                if (to.ColumnIndex == from.ColumnIndex + (2 * indexModifier)
                && GetSquare(from.ColumnIndex + (1 * indexModifier), from.RowIndex + (1 * indexModifier)).State == opponentState)
                {
                    return true;
                }
                else if (to.ColumnIndex == from.ColumnIndex - (2 * indexModifier)
                && GetSquare(from.ColumnIndex - (1 * indexModifier), from.RowIndex + (1 * indexModifier)).State == opponentState)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}