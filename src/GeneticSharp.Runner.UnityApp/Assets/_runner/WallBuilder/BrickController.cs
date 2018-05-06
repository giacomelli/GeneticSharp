using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.WallBuilder
{
    public class BrickController : MonoBehaviour
    {
        public bool HitFloor;
        public int HitBricksCount;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Floor")
            {
                HitFloor = true;
            }
            else 
            {
                HitBricksCount++;
            }
        }

        private void OnGetFromPool()
        {
            gameObject.SetActive(true);
            HitFloor = false;
            HitBricksCount = 0;
        }

        private void OnRelaseToPool()
        {
            gameObject.SetActive(false);
        }
    }
}