using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowChromosomeCam : MonoBehaviour {

    public GameObject Target;
    public Vector3 Offset = new Vector3(0, 0, -1);
    public float Speed = 1f;

    void LateUpdate() 
    {
        transform.position = Vector3.Lerp(transform.position, Target.transform.position + Offset, Time.deltaTime * Speed);
    }
}
