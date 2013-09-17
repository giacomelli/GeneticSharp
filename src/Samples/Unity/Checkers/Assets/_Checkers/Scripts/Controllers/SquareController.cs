using UnityEngine;
using System.Collections;
using GeneticSharp.Extensions.Checkers;
using Checkers.Application;

public class SquareController : MonoBehaviour {
	
	#region Fields
	private GameObject m_piece;
	#endregion
	
	#region Properties
	public CheckersSquare Model { get; set; }
	#endregion
	
	#region Methods
	private void Start ()
	{
		Unselect();
	
		m_piece = transform.FindChild ("PiecePrefab").gameObject;
		m_piece.GetComponent<PieceController>().CurrentSquare = this;
	}
	
	
	private void Update ()
	{
		if (Model.State == CheckersSquareState.OccupiedByPlayerOne) {
			m_piece.SetActive (true);
		} else if (Model.State == CheckersSquareState.OccupiedByPlayerTwo) {
			m_piece.SetActive (true);
		} else {
			m_piece.SetActive (false);
		}
	}
	
	public void Select ()
	{
		if (Model.State == CheckersSquareState.Free) {
			renderer.material.color = Color.red;	
		}
	}
	
	public void Unselect ()
	{
		if (Model.State == CheckersSquareState.NotPlayable) {
			renderer.material.color = Color.white;
		} else {
			renderer.material.color = Color.black;	
		}
	}
	
	#endregion
}
