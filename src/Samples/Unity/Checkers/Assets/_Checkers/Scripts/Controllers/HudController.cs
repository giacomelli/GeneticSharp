using UnityEngine;
using System.Collections;

public class HudController : MonoBehaviour {
	
	public static bool IsThinking { get; set; }
	public static bool IsInvalidMove { get; set; }
	public static bool IsGameOver { get; set; }
	
	private void OnGUI ()
	{
		if (IsThinking) {
			GUILayout.Label ("Thinking...", GUILayout.Width (100));
		}
		
		if (IsInvalidMove) {
			GUILayout.Label ("Invalid move!", GUILayout.Width (100));
		}
		
		if (IsGameOver) {
			GUILayout.Label ("GAME OVER!", GUILayout.Width (100));
		}
	}
}
