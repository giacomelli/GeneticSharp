using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSlider : MonoBehaviour {

    public Camera LeftCamera;
    public Camera RightCamera;

    void OnMouseDrag()
    {
        var pos = Input.mousePosition;
        var x = pos.x / Screen.width;
        LeftCamera.rect = new Rect(0, 0, 1f - x, 1);
        RightCamera.rect = new Rect(1f - x, 0, x, 1);
    }

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LeftCamera.rect = new Rect(0, 0, 1, 1);
            RightCamera.rect = new Rect(0, 0, 0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            LeftCamera.rect = new Rect(0, 0, .5f, 1);
            RightCamera.rect = new Rect(0, .5f, .5f, 1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LeftCamera.rect = new Rect(0, 0, 0, 0);
            RightCamera.rect = new Rect(0, 0, 1, 1);
        }
	}
}
