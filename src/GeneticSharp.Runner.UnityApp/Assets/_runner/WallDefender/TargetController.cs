using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour {

    public static int HitsCount;

    private void OnCollisionEnter(Collision collision)
	{
        if(collision.other.tag == "Bullet") 
        {
            HitsCount++;
        }
	}
}
