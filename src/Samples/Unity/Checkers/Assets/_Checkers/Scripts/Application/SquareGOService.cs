using UnityEngine;
using GeneticSharp.Extensions.Checkers;

namespace Checkers.Application
{
	public static class SquareGOService
	{
		#region Fields
		private static Object s_squarePrefab = Resources.Load("SquarePrefab");
		#endregion
		
		#region Methods
		public static GameObject CreateGameObject (CheckersSquare model)
		{
			var go = (GameObject)GameObject.Instantiate (s_squarePrefab);
			go.GetComponent<SquareController> ().Model = model;
			
			return go;
		}
		#endregion
	}
}

