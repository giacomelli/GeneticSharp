using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Extensions.Checkers
{
    /// <summary>
    /// Checkers fitness.
    /// </summary>
    public class CheckersFitness : IFitness
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Checkers.CheckersFitness"/> class.
        /// </summary>
        /// <param name="board">The checkers board.</param>
        public CheckersFitness(CheckersBoard board)
        {
            Board = board;
            Reset();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the board.
        /// </summary>
        /// <value>The board.</value>
        public CheckersBoard Board { get; private set; }
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
        /// <returns>The move fitness.</returns>
        /// <param name="move">The move.</param>
        public double EvaluateMove(CheckersMove move)
        {
            double moveFitness = 0;

            // Evals the move kind.
            var moveKind = Board.GetMoveKind(move);

            switch (moveKind)
            {
                case CheckersMoveKind.Forward:
                    moveFitness = 0.5;
                    break;

                case CheckersMoveKind.Capture:
                    moveFitness = 1;
                    break;

                case CheckersMoveKind.Invalid:
                    moveFitness = 0;
                    break;
            }

            if (moveFitness > 0)
            {
                var futurePiece = new CheckersPiece(move.Piece.Player) { CurrentSquare = move.ToSquare };

                // Evals the possibilities to capture anothers pieces.
                moveFitness += Board.CountCatchableByPiece(move.Piece);

                // Evals the possibilities to be captured by another pieces.
                moveFitness -= Board.CountPieceChancesToBeCaptured(futurePiece);
            }

            return moveFitness;
        }

        /// <summary>
        /// Resets the fitness.
        /// </summary>
        public void Reset()
        {
            Board.Reset();
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
                Board.MovePiece(move);
            }
        }
        #endregion        
    }
}
