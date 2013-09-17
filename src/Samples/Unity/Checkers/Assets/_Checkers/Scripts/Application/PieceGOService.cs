using UnityEngine;

namespace Checkers.Application
{
	public static class PieceGOService
	{
		#region Fields
		private static Object s_piecePrefab = Resources.Load("PiecePrefab");
		#endregion
		
		#region Methods
		public static GameObject CreateGameObject()
		{
			return (GameObject) GameObject.Instantiate (s_piecePrefab);
		}
		#endregion
	}
}

