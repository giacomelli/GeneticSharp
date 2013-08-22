using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Extensions.Checkers
{
	/// <summary>
	/// Checkers fitness.
	/// </summary>
    public class CheckersFitness : IFitness
    {
        #region Fields
        private int m_boardSize;
        #endregion

        #region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Checkers.CheckersFitness"/> class.
		/// </summary>
		/// <param name="boardSize">Board size.</param>
        public CheckersFitness(int boardSize)
        {
            m_boardSize = boardSize;
            Board = new CheckersSquare[boardSize, boardSize];

            for (int c = 0; c < boardSize; c++)
            {
                for (int r = 0; r < boardSize; r++)
                {
                    var square = new CheckersSquare(c, r);                    

                    if(square.State == CheckersSquareState.Free)
                    {
                        if (r < 3)
                        {
                            square.State = CheckersSquareState.OccupiedByPlayerOne;
                        }
                        else if (r >= boardSize - 3)
                        {
                            square.State = CheckersSquareState.OccupiedByPlayerTwo;
                        }
                    }

                    Board[c, r] = square;
                }
            }
        }
        #endregion

        #region Properties
		/// <summary>
		/// Gets the board.
		/// </summary>
		/// <value>The board.</value>
		public CheckersSquare[,] Board { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="GeneticSharp.Extensions.Checkers.CheckersFitness"/> supports parallel.
		/// </summary>
		/// <value><c>true</c> if supports parallel; otherwise, <c>false</c>.</value>
        public bool SupportsParallel
        {
            get { return false; }
        }
        #endregion

        #region Methods
		/// <summary>
		/// Performs the evaluation against the specified chromosome.
		/// </summary>
		/// <param name="chromosome">The chromosome to be evaluated.</param>
		/// <returns>The fitness of the chromosome.</returns>
        public double Evaluate(IChromosome chromosome)
        {
            double fitness = 0;
            var c = chromosome as CheckersChromosome;
            double movesAhead = c.Moves.Count;

            var nextMovementFitness = EvaluateMove(c.Moves.First());

            if (nextMovementFitness > 0)
            {
                fitness += nextMovementFitness / movesAhead;

                foreach (var move in c.Moves.Skip(1))
                {
                    fitness += EvaluateMove(move) / movesAhead;
                }

            }
            return fitness;
        }

		/// <summary>
		/// Evaluates the move.
		/// </summary>
		/// <returns>The move.</returns>
		/// <param name="move">Move.</param>
        private double EvaluateMove(CheckersMove move)
        {
            double moveFitness = 0;

            var from = Board[move.From.ColumnIndex, move.From.RowIndex];
            var to = Board[move.To.ColumnIndex, move.To.RowIndex];

            // From is square of the AI player and To is a free square.
            if (from.State == CheckersSquareState.OccupiedByPlayerOne && to.State == CheckersSquareState.Free)
            {
                // Simple move.
                if (to.RowIndex == from.RowIndex + 1 && (to.ColumnIndex == from.ColumnIndex - 1 || to.ColumnIndex == from.ColumnIndex + 1))
                {
                    moveFitness = 0.5;
                }
                else if (to.RowIndex == from.RowIndex + 2) // 'Eat' move.
                {
                    
                    if (from.ColumnIndex + 1 <m_boardSize && 
                        from.RowIndex + 1 < m_boardSize && 
                        to.ColumnIndex == from.ColumnIndex + 2 && 
                        Board[from.ColumnIndex + 1, from.RowIndex + 1].State == CheckersSquareState.OccupiedByPlayerTwo)
                    {
                        moveFitness = 1;
                    }
                    else if (from.ColumnIndex > 0 && 
                        from.RowIndex > 0 && 
                        to.ColumnIndex == from.ColumnIndex - 2 && 
                        Board[from.ColumnIndex - 1, from.RowIndex - 1].State == CheckersSquareState.OccupiedByPlayerTwo)
                    {
                        moveFitness = 1;
                    }                                      
                }
            }

            return moveFitness;
        }

		/// <summary>
		/// Update the specified checkersChromosome.
		/// </summary>
		/// <param name="checkersChromosome">Checkers chromosome.</param>
        public void Update(CheckersChromosome checkersChromosome)
        {
            if (checkersChromosome.Fitness > 0)
            {
                var move = checkersChromosome.Moves.First();

                var from = Board[move.From.ColumnIndex, move.From.RowIndex];
                from.State = CheckersSquareState.Free;

                var to = Board[move.To.ColumnIndex, move.To.RowIndex];
                to.State = CheckersSquareState.OccupiedByPlayerOne;

                if (to.RowIndex == from.RowIndex + 2) // 'Eat' move.
                {
                    if (to.ColumnIndex == from.ColumnIndex + 2)
                    {
                        Board[from.ColumnIndex + 1, from.RowIndex + 1].State = CheckersSquareState.Free;
                    }
                    else if (to.ColumnIndex == from.ColumnIndex - 2)
                    {
                        Board[from.ColumnIndex - 1, from.RowIndex - 1].State = CheckersSquareState.Free;
                    }
                }
            }
        }
        #endregion        
    }
}
