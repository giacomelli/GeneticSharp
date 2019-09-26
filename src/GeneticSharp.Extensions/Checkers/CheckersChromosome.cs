using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Checkers
{
    /// <summary>
    /// Checkers chromosome.
    /// </summary>
    public sealed class CheckersChromosome : ChromosomeBase
    {
        #region Fields
        private readonly int m_boardSize;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Checkers.CheckersChromosome"/> class.
        /// </summary>
        /// <param name="movesAhead">Moves ahead.</param>
        /// <param name="boardSize">Board size.</param>
        public CheckersChromosome(int movesAhead, int boardSize)
            : base(movesAhead)
        {
            m_boardSize = boardSize;
            Moves = new List<CheckersMove>();

            for (int i = 0; i < movesAhead; i++)
            {
                Moves.Add(null);
                ReplaceGene(i, GenerateGene(i));
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the moves.
        /// </summary>
        /// <value>The moves.</value>
        public IList<CheckersMove> Moves { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Generates the gene for the specified index.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override Gene GenerateGene(int geneIndex)
        {
            var from = FindPlayableSquare();
            from.PutPiece(new CheckersPiece(CheckersPlayer.PlayerOne));

            var to = FindPlayableSquare();
            var move = new CheckersMove(from.CurrentPiece, to);

            Moves[geneIndex] = move;

            return new Gene(move);
        }

        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        public override IChromosome CreateNew()
        {
            return new CheckersChromosome(Length, m_boardSize);
        }

        /// <summary>
        /// Creates a clone.
        /// </summary>
        /// <returns>The chromosome clone.</returns>
        public override IChromosome Clone()
        {
            var clone = base.Clone() as CheckersChromosome;
            clone.Moves = Moves;

            return clone;
        }

        private CheckersSquare FindPlayableSquare()
        {
            CheckersSquare square;
            var rnd = RandomizationProvider.Current;

            do
            {
                var columnIndex = rnd.GetInt(0, m_boardSize);
                var rowIndex = columnIndex % 2 == 0 ? rnd.GetOddInt(0, m_boardSize) : rnd.GetEvenInt(0, m_boardSize);
                square = new CheckersSquare(columnIndex, rowIndex);
            }
            while (square.State == CheckersSquareState.NotPlayable);

            return square;
        }
        #endregion
    }
}
