using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowChromosomeCam : MonoBehaviour {

    private Color m_originalBackgroundColor;
    private bool m_isFollowing;
    private GameObject m_target;

    public Vector3 Offset = new Vector3(0, 0, -1);
    public float Speed = 1f;
    public Color NoFollowingColor = Color.gray;

    public Camera Camera { get; private set; }

	private void Awake()
	{
        Camera = GetComponent<Camera>();
        m_originalBackgroundColor = Camera.backgroundColor;
	}

	void LateUpdate() 
    {
        if(m_isFollowing)
            transform.position = Vector3.Lerp(transform.position, m_target.transform.position + Offset, Time.deltaTime * Speed);
    }

    public void StartFollowing(GameObject target)
    {
        if (target == null)
            Debug.LogError("Target cannot be null");
        
        if (!m_isFollowing)
        {
            transform.position = target.transform.position; 
            m_target = target;
            Camera.backgroundColor = m_originalBackgroundColor;
            m_isFollowing = true;
        }
        else
            Debug.LogWarning("Trying to start an already started camera");
    }

    public void StopFollowing()
    {
        if (m_isFollowing)
        {
            m_isFollowing = false;


            if (Camera != null)
                Camera.backgroundColor = NoFollowingColor;
        }
        else
            Debug.LogWarning("Trying to stop an already stopped camera");
    }
}
