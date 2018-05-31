using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraExpander : MonoBehaviour {

    private Camera m_camera;
    private Rect m_originalRect;
    private float m_originalDepth;

    private BoxCollider2D m_collider;
    private Vector2 m_originalSize;

    private bool m_expanded;

	private void Awake()
	{
        m_camera = GetComponentInParent<Camera>();
        m_collider = GetComponent<BoxCollider2D>();
 	}

	void OnMouseDown()
    {
        if (m_expanded)
        {
            m_camera.pixelRect = m_originalRect;
            m_camera.depth = m_originalDepth;
            m_collider.size = m_originalSize;
        }
        else 
        {
            m_originalRect = m_camera.pixelRect;
            m_originalDepth = m_camera.depth;
            m_originalSize = m_collider.size;

            m_camera.pixelRect = new Rect(0, 0, Screen.width, Screen.height);
            m_camera.depth = float.MaxValue;
            m_collider.size = new Vector2(m_camera.pixelWidth, m_camera.pixelHeight);
        }

        m_expanded = !m_expanded;
    }
}
