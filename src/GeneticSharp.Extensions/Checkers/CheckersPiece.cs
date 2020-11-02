namespace GeneticSharp.Extensions.Checkers
{
    /// <summary>
    /// A checkers' piece.
    /// </summary>
    public class CheckersPiece
    {
        #region Fields
        private CheckersSquare m_currentSquare;
        #endregion

        #region Contructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckersPiece"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public CheckersPiece(CheckersPlayer player)
        {
            Player = player;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the player.
        /// </summary>        
        public CheckersPlayer Player { get; private set; }

        /// <summary>
        /// Gets or sets the current square.
        /// </summary>
        public CheckersSquare CurrentSquare
        {
            get
            {
                return m_currentSquare;
            }

            set
            {
                m_currentSquare = value;

                if (m_currentSquare != null && m_currentSquare.CurrentPiece != this)
                {
                    m_currentSquare.PutPiece(this);
                }
            }
        }
        #endregion
    }
}