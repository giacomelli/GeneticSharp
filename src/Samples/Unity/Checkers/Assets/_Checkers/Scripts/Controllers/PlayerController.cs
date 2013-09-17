using UnityEngine;
using System.Collections;
using GeneticSharp.Extensions.Checkers;

public class PlayerController : MonoBehaviour {
	
	#region Fields
	private SquareController m_lastSquareSelected;
	#endregion
	
	private void Start ()
	{
		GAController.Instance.Ran += delegate {
			if (m_lastSquareSelected != null) {
				m_lastSquareSelected.Unselect ();
			}
		};	
	}
	
	private void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast (ray, out hit)) {
				var piece = hit.collider.gameObject.GetComponent<PieceController> ();
				
				if (m_lastSquareSelected != null) {
					m_lastSquareSelected.Unselect ();
				}
				
				if (piece == null) {
					m_lastSquareSelected = hit.collider.gameObject.GetComponent<SquareController> ();
					m_lastSquareSelected.Select ();
					BoardController.Instance.MoveCurrentPieceTo (m_lastSquareSelected);
				} else {
					BoardController.Instance.SelectPiece (piece);
				}
			}
		}
	}
}
