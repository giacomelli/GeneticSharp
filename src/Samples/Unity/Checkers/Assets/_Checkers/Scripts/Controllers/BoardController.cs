using UnityEngine;
using System.Collections;
using GeneticSharp.Extensions.Checkers;
using Checkers.Application;

public class BoardController : MonoBehaviour {
	
	#region Fields
	private PieceController m_lastSelectedPiece;
	#endregion
	
	#region Constructors
	public BoardController()
	{
		Instance = this;
	}
	#endregion
	
	#region Editor properties
	public int m_squareSize = 1;
	#endregion
	
	#region Properties
	public static BoardController Instance { get; private set; }
	#endregion
	
	#region Methods
	private void Start ()
	{
		var fitness = GAController.Instance.Fitness;
		var boardSize = GAController.Instance.BoardSize;
		var posModifier = (boardSize / 2) * m_squareSize;
	
		// Draw board.
		for (int c = 0; c < boardSize; c++) {
			for (int r = 0; r < boardSize; r++) {
				var squareModel = fitness.Board.GetSquare(c, r);
				var x = (c * m_squareSize) - posModifier;
				var z = (r * m_squareSize) - posModifier;

				// Draw square.
				var square = SquareGOService.CreateGameObject (squareModel);
				square.transform.position = new Vector3 (x, 0, z);
				square.transform.localScale = new Vector3 (m_squareSize, m_squareSize, m_squareSize);			
			}
		}		
	}
	
	public void SelectPiece (PieceController piece)
	{
		if (m_lastSelectedPiece != null) {
			m_lastSelectedPiece.Unselect ();
		}
		
		if (piece != null) {
			piece.Select ();
		}
		
		m_lastSelectedPiece = piece;
	}
	
	public void MoveCurrentPieceTo (SquareController square)
	{
		if (m_lastSelectedPiece != null && m_lastSelectedPiece.CurrentSquare.Model.CurrentPiece != null) {
			var move = new CheckersMove (m_lastSelectedPiece.CurrentSquare.Model.CurrentPiece, square.Model);
			var fitness = GAController.Instance.Fitness;
			
			if (fitness.Board.GetMoveKind (move) == CheckersMoveKind.Invalid) {
				HudController.IsInvalidMove = true; 
			} else {
				HudController.IsInvalidMove = false;
				fitness.Board.MovePiece (move);
				GAController.Instance.MovePiece ();
			}
		}
	}
	
	#endregion
}
