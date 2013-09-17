using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private void Start () {
		Camera.main.transform.position += Vector3.up * GAController.Instance.BoardSize;
	}
}
