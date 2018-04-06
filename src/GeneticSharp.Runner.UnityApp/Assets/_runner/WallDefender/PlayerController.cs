using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Object BulletPrefab;
    public Vector3 FireForce = Vector3.forward;
    public bool AutoFire = true;

    public static int BulletsFiredCount;

	void Update()
    {
        if(AutoFire || Input.GetMouseButton(0))
        {
            var pos = Input.mousePosition;
            var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity) as GameObject;

            var direction = new Vector3(
                (Screen.width / 2 - pos.x) * -1,
                (Screen.height / 2 - pos.y) * -1,
                1);
            
            var force = Vector3.Scale(FireForce, direction);

            bullet.GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
            BulletsFiredCount++;
        }
    }

	private void OnDrawGizmos()
	{
        Gizmos.DrawSphere(transform.position, 1);
	}
}
