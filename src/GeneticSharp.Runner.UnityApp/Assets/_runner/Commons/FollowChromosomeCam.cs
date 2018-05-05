using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowChromosomeCam : MonoBehaviour {

    private Color m_originalBackgroundColor;
    private bool m_isFollowing = true;

    public GameObject Target;
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
        if(m_isFollowing && Target != null)
            transform.position = Vector3.Lerp(transform.position, Target.transform.position + Offset, Time.deltaTime * Speed);
    }

    public void StartFollowing()
    {
        if (!m_isFollowing)
        {
            Camera.backgroundColor = m_originalBackgroundColor;
            m_isFollowing = true;
        }
    }

    public void StopFollowing()
    {
        if (m_isFollowing)
        {
            m_isFollowing = false;
            Camera.backgroundColor = NoFollowingColor;
        }
    }
}
