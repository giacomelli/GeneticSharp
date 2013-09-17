using UnityEngine;
using System.Collections;
using GeneticSharp.Extensions.Checkers;

public class PieceController : MonoBehaviour {
	
	public Material m_PlayerOneMaterial;
	public Material m_PlayerTwoMaterial;
	
	public SquareController CurrentSquare { get; set; }
	
	private void Start ()
	{
		UpdateMaterial ();
			
		GAController.Instance.Ran += delegate {
			UpdateMaterial ();
		};
	}
	
	private void UpdateMaterial ()
	{
		if (CurrentSquare.Model.State == CheckersSquareState.OccupiedByPlayerOne) {
			renderer.material = m_PlayerOneMaterial;
		} else {
			renderer.material = m_PlayerTwoMaterial;
		}	
	}
	
	public void Select ()
	{
		renderer.material.color = Color.red;		
	}
	
	public void Unselect ()
	{
		renderer.material.color = Color.white;		
	}
}