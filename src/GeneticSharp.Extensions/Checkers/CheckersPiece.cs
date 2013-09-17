using System;

namespace GeneticSharp.Extensions.Checkers
{	
	public class CheckersPiece
    {
        #region Fields
        private CheckersSquare m_currentSquare;
        #endregion

        #region Contructors
        public CheckersPiece(CheckersPlayer player)
		{
			Player = player;
		}
		#endregion

		#region Properties
		public CheckersPlayer Player { get; private set; }
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